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
        internal static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dedupe <folder> <output>");
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

            foreach (var entry in entries)
            {
                var key = (entry.Filename, entry.Size);

                if (!entryList.ContainsKey(key))
                {
                    entryList.Add(key, new List<FileSystemEntry> { entry });
                    continue;
                }

                entryList[key].Add(entry);
            }

            // Only list files where there are duplicates
            foreach (var entry in entryList.Where(kvp => kvp.Value.Count > 1))
            {
                for (var i = 0; i < entry.Value.Count; i++)
                {
                    if (i == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(entry.Value[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("    DUPLICATE: " + entry.Value[i]);
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
