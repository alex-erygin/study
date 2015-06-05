using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    /// <summary>
    ///     SafeHandle for a BCRYPT_HASH_HANDLE.
    /// </summary>
    internal sealed class SafeBCryptHashHandle : SafeHandleWithBuffer
    {
        [DllImport("bcrypt.dll")]
        [SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern ErrorCode BCryptDestroyHash(IntPtr hHash);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseNativeHandle()
        {
            return BCryptDestroyHash(handle) == ErrorCode.Success;
        }
    }
}
