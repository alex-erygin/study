using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    /// <summary>
    ///     SafeHandle for a native BCRYPT_KEY_HANDLE.
    /// </summary>
    public sealed class SafeBCryptKeyHandle : SafeHandleWithBuffer
    {
        [DllImport("bcrypt.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern ErrorCode BCryptDestroyKey(IntPtr hKey);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseNativeHandle()
        {
            return BCryptDestroyKey(handle) == ErrorCode.Success;
        }
    }
}