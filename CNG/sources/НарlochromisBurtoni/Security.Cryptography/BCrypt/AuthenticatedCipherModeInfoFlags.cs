using System;

namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    /// <summary>
    ///     Flags for use with the BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO structure
    /// </summary>
    [Flags]
    internal enum AuthenticatedCipherModeInfoFlags
    {
        None = 0x00000000,
        ChainCalls = 0x00000001,                           // BCRYPT_AUTH_MODE_CHAIN_CALLS_FLAG
        InProgress = 0x00000002,                           // BCRYPT_AUTH_MODE_IN_PROGRESS_FLAG
    }
}