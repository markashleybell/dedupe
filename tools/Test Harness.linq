<Query Kind="Program">
  <Reference Relative="..\dedupe\bin\Debug\net472\dedupe.exe">C:\Src\dedupe\dedupe\bin\Debug\net472\dedupe.exe</Reference>
  <Namespace>dedupe</Namespace>
  <Namespace>static dedupe.InteropFunctions</Namespace>
</Query>

void Main()
{
    // var tree = ScanDirectory(@"C:\Temp\dedupetest");
    // var tree = ScanDirectory(@"C:\Users\me\Pictures");
    var tree = ScanDirectory(@"C:\Webselect");

    var entries = FlattenTree(tree)
        .OrderBy(f => f.DateCreated)
        .ThenBy(f => f.Path);
        
    // entries.Dump();
    // entries.Count().Dump();
    
    var files = new Dictionary<(string, int), List<FileSystemEntry>>();
    
    foreach (var entry in entries)
    {
        var key = (entry.Filename, entry.Size);
        
        if (!files.ContainsKey(key))
        {
            files.Add(key, new List<FileSystemEntry> { entry });
            continue;
        }

        files[key].Add(entry);
    }
    
    // files.Dump();
    // files.Count.Dump();
    
    var filesWithDuplicates = files
        .Where(kvp => kvp.Value.Count > 1);
    
    filesWithDuplicates.Dump();
    // filesWithDuplicates.Count().Dump();
}

public static class Functions
{
    public static byte[] ReadFileBytes(this string path, int count)
    {
        // var start = 0;
        var bytes = new byte[count];
        
        using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
        {
            // reader.BaseStream.Seek(start, SeekOrigin.Begin);
            reader.Read(bytes, 0, count);
        }
        
        return bytes;
    }
}