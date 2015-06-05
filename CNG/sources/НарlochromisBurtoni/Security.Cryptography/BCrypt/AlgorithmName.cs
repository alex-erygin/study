namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    /// <summary>
    ///     Well known algorithm names
    /// </summary>
    public static class AlgorithmName
    {
        public const string Aes = "AES";                          // BCRYPT_AES_ALGORITHM
        public const string Rng = "RNG";                          // BCRYPT_RNG_ALGORITHM
        public const string Rsa = "RSA";                          // BCRYPT_RSA_ALGORITHM
        public const string TripleDes = "3DES";                   // BCRYPT_3DES_ALOGORITHM
        public const string Sha1 = "SHA1";                        // BCRYPT_SHA1_ALGORITHM
        public const string Sha256 = "SHA256";                    // BCRYPT_SHA256_ALGORITHM
        public const string Sha384 = "SHA384";                    // BCRYPT_SHA384_ALGORITHM
        public const string Sha512 = "SHA512";                    // BCRYPT_SHA512_ALGORITHM
        public const string Pbkdf2 = "PBKDF2";                    // BCRYPT_PBKDF2_ALGORITHM     
        public const string ECDH_P256 = "ECDH_P256";
    }
}
