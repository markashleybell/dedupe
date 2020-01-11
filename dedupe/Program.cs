using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static dedupe.InteropFunctions;

[assembly: InternalsVisibleTo("LINQPadQuery")]

namespace dedupe
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dedupe <folder> <output>");

                return -1;
            }

            var folder = args[0];
            var output = args[1];

            var fileCount = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Count();

            Console.WriteLine($"Found {fileCount} files");

            var originals = new Dictionary<string, FileSystemEntry>();

            var tree = ScanDirectory(folder);

            var entries = FlattenTree(tree)
                .OrderBy(f => f.DateCreated)
                .ThenBy(f => f.Path);

            var entryList = new Dictionary<(string, int), List<FileSystemEntry>>();

            // First, we add all files to a dictionary keyed on file name and size
            // If a file has the same name *and* size, it's almost definitely a duplicate
            foreach (var entry in entries)
            {
                var key = (entry.Filename, entry.Size);

                if (!entryList.ContainsKey(key))
                {
                    entryList.Add(key, new List<FileSystemEntry>());
                }

                entryList[key].Add(entry);
            }

            string getDestinationPathFor(FileSystemEntry f, string basePath) =>
                $@"{basePath}\{Path.GetFileNameWithoutExtension(f.Filename)}_DUPLICATE_FROM_{new DirectoryInfo(Path.GetDirectoryName(f.Path)).Name}{Path.GetExtension(f.Filename)}";

            // Now we can figure out which files there are multiple copies of
            foreach (var entry in entryList)
            {
                // This *should* be the original
                var oldest = entry.Value[0];

                var destinationPath = $@"{output}\{oldest.Filename}";

                if (File.Exists(destinationPath))
                {
                    // We've already copied a file with this name to the output folder,
                    // so there must be multiple files with the same name but different
                    // content. In this case, we just copy it to a different file name.
                    WriteError($"    DUPLICATE (DIFFERENT CONTENT): {oldest}");

                    File.Copy(oldest.Path, getDestinationPathFor(oldest, output));
                }
                else
                {
                    WriteSuccess(oldest);

                    File.Copy(oldest.Path, destinationPath);
                }

                foreach (var file in entry.Value.Skip(1))
                {
                    // Anything after the first file with this size/name key is a duplicate
                    WriteError($"    DUPLICATE (SAME NAME AND SIZE): {file}");

                    var duplicateFolder = $@"{output}\{entry.Key.Item1}_DUPLICATES";

                    if (!Directory.Exists(duplicateFolder))
                    {
                        Directory.CreateDirectory(duplicateFolder);
                    }

                    File.Copy(file.Path, getDestinationPathFor(file, duplicateFolder));
                }
            }

            return 0;
        }

        private static void WriteSuccess(object o)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(o);
            Console.ResetColor();
        }

        private static void WriteError(object o)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(o);
            Console.ResetColor();
        }
    }
}
