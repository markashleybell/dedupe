using System;
using System.Collections.Generic;
using System.Linq;

namespace dedupe
{
    public struct FileSystemEntry : IEquatable<FileSystemEntry>
    {
        public FileSystemEntry(
            string path,
            DateTime dateCreated,
            DateTime dateModified,
            int size)
            : this(
                path,
                dateCreated,
                dateModified,
                size,
                false,
                Enumerable.Empty<FileSystemEntry>())
        {
        }

        public FileSystemEntry(
            string path,
            IEnumerable<FileSystemEntry> children)
            : this(
                path,
                default,
                default,
                0,
                true,
                children)
        {
        }

        private FileSystemEntry(
            string path,
            DateTime dateCreated,
            DateTime dateModified,
            int size,
            bool isDirectory,
            IEnumerable<FileSystemEntry> children)
        {
            if (!IsValid(path))
            {
                throw new Exception("Path cannot be null");
            }

            Path = path;
            Filename = System.IO.Path.GetFileName(path);
            DateCreated = dateCreated;
            DateModified = dateModified;
            Size = size;
            IsDirectory = isDirectory;
            Children = children ?? Enumerable.Empty<FileSystemEntry>();
        }

        public string Path { get; }

        public string Filename { get; }

        public DateTime DateCreated { get; }

        public DateTime DateModified { get; }

        public int Size { get; set; }

        public bool IsDirectory { get; }

        public IEnumerable<FileSystemEntry> Children { get; }

        public static bool operator ==(FileSystemEntry a, FileSystemEntry b) =>
            a.Equals(b);

        public static bool operator !=(FileSystemEntry a, FileSystemEntry b) =>
            !a.Equals(b);

        public bool Equals(FileSystemEntry other) =>
            Path == other.Path;

        public override bool Equals(object obj) =>
            obj is FileSystemEntry && Equals((FileSystemEntry)obj);

        public override int GetHashCode() =>
            Path.GetHashCode();

        public override string ToString() =>
            Path;

        private static bool IsValid(string path) =>
            path != null;
    }
}
