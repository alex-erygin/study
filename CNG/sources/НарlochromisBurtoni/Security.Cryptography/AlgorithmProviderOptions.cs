using System;

namespace НарlochromisBurtoni.Security.Cryptography
{
    /// <summary>
    ///     Flags for BCryptOpenAlgorithmProvider
    /// </summary>
    [Flags]
    internal enum AlgorithmProviderOptions
    {
        None = 0x00000000,
        HmacAlgorithm = 0x00000008,                           // BCRYPT_ALG_HANDLE_HMAC_FLAG
    }
}