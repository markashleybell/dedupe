using System.IO;
using System.Runtime.InteropServices;

namespace dedupe
{
    // PLEASE NOTE: the *order* of the fields in this struct MUST NOT CHANGE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WIN32_FIND_DATAW
    {
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1202 // 'public' members should come before 'internal' members
        public FileAttributes dwFileAttributes;
        internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
#pragma warning disable SA1202 // 'public' members should come before 'internal' members
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
    }
}
