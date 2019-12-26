using System;
using System.Runtime.InteropServices;

namespace dedupe
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll")]
        internal static extern bool FindClose(IntPtr hFindFile);
    }
}
