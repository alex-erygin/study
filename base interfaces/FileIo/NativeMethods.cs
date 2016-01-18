namespace FileIo
{
    internal static class NativeMethods
    {
        public static extern int CreateSigner(IFile inputFile, IFile outputFile, string CN, string serialNumber);
    }
}