using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dedupe
{
    internal static class InteropFunctions
    {
        private const string Current = ".";
        private const string Parent = "..";

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        internal static FileSystemEntry ScanDirectory(string directory, string replacePrefix = null)
        {
            var findHandle = INVALID_HANDLE_VALUE;

            var children = new List<FileSystemEntry>();

            try
            {
                findHandle = NativeMethods.FindFirstFileW($@"{directory}\*", out var findData);

                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    do
                    {
                        if (findData.cFileName == Current || findData.cFileName == Parent)
                        {
                            continue;
                        }

                        var filePath = $@"{directory}\{findData.cFileName}";

                        if ((findData.dwFileAttributes & FileAttributes.Directory) != 0)
                        {
                            children.Add(ScanDirectory(filePath, replacePrefix));
                        }
                        else
                        {
                            var fpath = replacePrefix is object
                                ? filePath.Replace(replacePrefix, string.Empty)
                                : filePath;

                            var entry = new FileSystemEntry(
                                fpath,
                                findData.ftCreationTime.ToDateTime(),
                                findData.ftLastWriteTime.ToDateTime(),
                                findData.nFileSizeLow
                            );

                            children.Add(entry);
                        }
                    }
                    while (NativeMethods.FindNextFile(findHandle, out findData));
                }
            }
            finally
            {
                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    NativeMethods.FindClose(findHandle);
                }
            }

            var dpath = replacePrefix is object
                ? directory.Replace(replacePrefix, string.Empty)
                : directory;

            return new FileSystemEntry(dpath, children);
        }

        internal static IEnumerable<FileSystemEntry> FlattenTree(FileSystemEntry f)
        {
            var paths = new List<FileSystemEntry>();

            foreach (var dir in f.Children.Where(c => c.IsDirectory))
            {
                paths.AddRange(FlattenTree(dir));
            }

            var files = f.Children.Where(c => !c.IsDirectory);

            paths.AddRange(files);

            return paths;
        }

        internal static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME time)
        {
            var high = (ulong)time.dwHighDateTime;
            var low = (uint)time.dwLowDateTime;
            var fileTime = (long)((high << 32) + low);

            try
            {
                return DateTime.FromFileTimeUtc(fileTime);
            }
            catch
            {
                return DateTime.FromFileTimeUtc(0xFFFFFFFF);
            }
        }
    }
}
