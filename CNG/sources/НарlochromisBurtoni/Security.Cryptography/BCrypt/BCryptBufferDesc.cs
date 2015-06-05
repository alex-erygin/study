using System;
using System.Runtime.InteropServices;

namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BCryptBufferDesc
    {
        internal int ulVersion;
        internal int cBuffers;
        internal IntPtr pBuffers;       // PBCryptBuffer
    }
}
