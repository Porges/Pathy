using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Pathy
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll",
            CallingConvention = CallingConvention.Winapi,
            SetLastError = true,
            CharSet = CharSet.Unicode,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateHardLink(
            string fileName,
            string existingFileName,
            IntPtr reserved = default(IntPtr));

        public static void CreateHardLinkChecked(string fileName, string existingFileName)
        {
            if (!CreateHardLink(fileName, existingFileName))
            {
                var ex = new Win32Exception();
                throw new IOException("Error creating hard link", ex);
            }
        }

        [DllImport("Shlwapi.dll",
            CallingConvention = CallingConvention.StdCall,
            SetLastError = true,
            CharSet = CharSet.Unicode,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PathCompactPathEx(
            StringBuilder buffer,
            string input,
            uint bufferSize,
            uint reserved = 0
            );

        public static string CompactPathChecked(string input, int width)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "width must be >= 0");
            }

            var buffer = new StringBuilder(width + 1);
            if (!PathCompactPathEx(buffer, input, (uint)width + 1))
            {
                throw new Win32Exception();
            }

            return buffer.ToString();
        }

        [DllImport("Shlwapi.dll",
            CallingConvention = CallingConvention.StdCall,
            SetLastError = false,
            CharSet = CharSet.Unicode,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true)]
        public static extern int StrCmpLogicalW(string left, string right);

    }
}
