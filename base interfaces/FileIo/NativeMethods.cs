using System.Runtime.InteropServices;

namespace FileIo
{
    public static class SignerFactory
    {
        [DllImport("Cyclope", CallingConvention = CallingConvention.StdCall)]
        public static extern int CreateSigner(
            [MarshalAs(UnmanagedType.Interface)]IFile inputFile, 
            [MarshalAs(UnmanagedType.Interface)]IFile outputFile, 
            [MarshalAs(UnmanagedType.LPStr)] string CN,
            [MarshalAs(UnmanagedType.LPStr)] string serialNumber,
            [MarshalAs(UnmanagedType.Interface)] out ISigner signer);
    }
}