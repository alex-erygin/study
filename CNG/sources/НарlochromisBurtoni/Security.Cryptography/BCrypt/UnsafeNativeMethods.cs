using System;
using System.Runtime.InteropServices;
using System.Security;

namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptCreateHash(SafeBCryptAlgorithmHandle hAlgorithm,
                                                          [Out] out SafeBCryptHashHandle hHash,
                                                          IntPtr pbHashObject,              // byte *
                                                          int cbHashObject,
                                                          [In, MarshalAs(UnmanagedType.LPArray)]byte[] pbSecret,
                                                          int cbSecret,
                                                          int dwFlags);

        [DllImport("bcrypt.dll")]
        public static extern ErrorCode BCryptImportKeyPair(
            SafeBCryptAlgorithmHandle algorithmHandle,
            IntPtr hImportKey,
            [MarshalAs(UnmanagedType.LPWStr)] string pszBlobType,
            [Out] out SafeBCryptKeyHandle phKey,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] keyBlob,
            int keyBlobLength,
            int dwFlags
            );

        [DllImport("bcrypt.dll")]
        public static extern ErrorCode BCryptGenerateKeyPair(
            SafeBCryptAlgorithmHandle algorithmHandle,
            [Out] out SafeBCryptKeyHandle keyPair,
            [In] int keyLength,
            int flags);

        [DllImport("bcrypt.dll")]
        public static extern ErrorCode BCryptFinalizeKeyPair(
            [In] [Out] SafeBCryptKeyHandle keyPair,
            int flags);


        // Overload of BCryptDecrypt for use in standard decryption
        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptDecrypt(SafeBCryptKeyHandle hKey,
                                                       [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                       int cbInput,
                                                       IntPtr pPaddingInfo,
                                                       [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbIV,
                                                       int cbIV,
                                                       [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                       int cbOutput,
                                                       [Out] out int pcbResult,
                                                       int dwFlags);

        // Overload of BCryptDecrypt for use with authenticated decryption
        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptDecrypt(SafeBCryptKeyHandle hKey,
                                                       [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                       int cbInput,
                                                       [In, Out] ref BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO pPaddingInfo,
                                                       [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbIV,
                                                       int cbIV,
                                                       [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                       int cbOutput,
                                                       [Out] out int pcbResult,
                                                       int dwFlags);

        // Overload of BCryptEncrypt for use in standard encryption
        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptEncrypt(SafeBCryptKeyHandle hKey,
                                                       [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                       int cbInput,
                                                       IntPtr pPaddingInfo,
                                                       [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbIV,
                                                       int cbIV,
                                                       [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                       int cbOutput,
                                                       [Out] out int pcbResult,
                                                       int dwFlags);

        // Overload of BCryptEncrypt for use with authenticated encryption
        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptEncrypt(SafeBCryptKeyHandle hKey,
                                                       [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                       int cbInput,
                                                       [In, Out] ref BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO pPaddingInfo,
                                                       [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbIV,
                                                       int cbIV,
                                                       [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                       int cbOutput,
                                                       [Out] out int pcbResult,
                                                       int dwFlags);

        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptFinishHash(SafeBCryptHashHandle hHash,
                                                          [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                          int cbOutput,
                                                          int dwFlags);

        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptGenRandom(SafeBCryptAlgorithmHandle hAlgorithm,
                                                         [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbBuffer,
                                                         int cbBuffer,
                                                         int dwFlags);

        [DllImport("bcrypt.dll", EntryPoint = "BCryptGetProperty")]
        internal static extern ErrorCode BCryptGetAlgorithmProperty(SafeBCryptAlgorithmHandle hObject,
                                                                    [MarshalAs(UnmanagedType.LPWStr)] string pszProperty,
                                                                    [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                                    int cbOutput,
                                                                    [In, Out] ref int pcbResult,
                                                                    int flags);

        [DllImport("bcrypt.dll", EntryPoint = "BCryptGetProperty")]
        internal static extern ErrorCode BCryptGetHashProperty(SafeBCryptHashHandle hObject,
                                                               [MarshalAs(UnmanagedType.LPWStr)] string pszProperty,
                                                               [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
                                                               int cbOutput,
                                                               [In, Out] ref int pcbResult,
                                                               int flags);

        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptHashData(SafeBCryptHashHandle hHash,
                                                        [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                        int cbInput,
                                                        int dwFlags);

        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptImportKey(SafeBCryptAlgorithmHandle hAlgorithm,
                                                         SafeBCryptKeyHandle hImportKey,
                                                         [MarshalAs(UnmanagedType.LPWStr)] string pszBlobType,
                                                         [Out] out SafeBCryptKeyHandle phKey,
                                                         [In, Out] IntPtr pbKeyObject,          // BYTE *
                                                         int cbKeyObject,
                                                         [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                         int cbInput,
                                                         int dwFlags);

        [DllImport("bcrypt.dll")]
        internal static extern ErrorCode BCryptOpenAlgorithmProvider([Out] out SafeBCryptAlgorithmHandle phAlgorithm,
                                                                     [MarshalAs(UnmanagedType.LPWStr)] string pszAlgId,
                                                                     [MarshalAs(UnmanagedType.LPWStr)] string pszImplementation,
                                                                     AlgorithmProviderOptions dwFlags);

        [DllImport("bcrypt.dll", EntryPoint = "BCryptSetProperty")]
        internal static extern ErrorCode BCryptSetAlgorithmProperty(SafeBCryptAlgorithmHandle hObject,
                                                                    [MarshalAs(UnmanagedType.LPWStr)] string pszProperty,
                                                                    [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                                    int cbInput,
                                                                    int dwFlags);

        [DllImport("bcrypt.dll", EntryPoint = "BCryptSetProperty")]
        internal static extern ErrorCode BCryptSetHashProperty(SafeBCryptHashHandle hObject,
                                                               [MarshalAs(UnmanagedType.LPWStr)] string pszProperty,
                                                               [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
                                                               int cbInput,
                                                               int dwFlags);

        [DllImport("bcrypt.dll", EntryPoint = "BCryptKeyDerivation")]
        internal static extern ErrorCode BCryptKeyDerivation(SafeBCryptKeyHandle hKey,
                                                             [In] ref BCryptBufferDesc pParamList,
                                                             [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbDerivedKey,
                                                             int cbDerivedKey,
                                                             [In, Out] ref int pcbResult,
                                                             int dwFlags);

        [DllImport("bcrypt.dll", EntryPoint = "BCryptGenerateSymmetricKey")]
        internal static extern ErrorCode BCryptGenerateSymmetricKey(SafeBCryptAlgorithmHandle hAlgorithm,
                                                                    [Out] out SafeBCryptKeyHandle phKey,
                                                                    [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbKeyObjectOptional,
                                                                    int cbKeyObject,
                                                                    [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbSecret,
                                                                    int cbSecret,
                                                                    int dwFlags);

        [DllImport("bcrypt.dll", EntryPoint = "BCryptDeriveKeyPBKDF2")]
        internal static extern ErrorCode BCryptDeriveKeyPBKDF2(SafeBCryptAlgorithmHandle hPrf,
                                                               [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbPassword,
                                                               int cbPassword,
                                                               [In, MarshalAs(UnmanagedType.LPArray)] byte[] pbSalt,
                                                               int cbSalt,
                                                               ulong cIterations,
                                                               [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbDerivedKey,
                                                               int cbDerivedKey,
                                                               int dwFlags);

    }
}
