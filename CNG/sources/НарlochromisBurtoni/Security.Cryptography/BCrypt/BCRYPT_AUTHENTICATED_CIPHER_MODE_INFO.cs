using System;
using System.Runtime.InteropServices;

namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO
    {
        internal int cbSize;
        internal int dwInfoVersion;

        
        internal IntPtr pbNonce;            // byte *
        internal int cbNonce;

        
        internal IntPtr pbAuthData;         // byte *
        internal int cbAuthData;

        
        internal IntPtr pbTag;              // byte *
        internal int cbTag;

        
        internal IntPtr pbMacContext;       // byte *
        internal int cbMacContext;

        internal int cbAAD;
        internal long cbData;
        internal AuthenticatedCipherModeInfoFlags dwFlags;
    }
}
