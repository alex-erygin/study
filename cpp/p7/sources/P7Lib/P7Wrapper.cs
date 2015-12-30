using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace P7Lib
{
    public class P7Wrapper
    {
        public void Sign(string sourceFile, string targetFile, X509Certificate2 certificate)
        {
            NativeMethods.SignFile(sourceFile, targetFile, certificate.Handle);
        }
    }

    internal static class NativeMethods
    {
        [DllImport(@"c:\Projects\Study\cpp\p7\sources\P7Lib\bin\Debug\MyLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SignFile(string sourceFile, string targetFile, IntPtr certificateContext);
    }
}