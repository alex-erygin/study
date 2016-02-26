using System;
using System.Security.Cryptography.X509Certificates;

namespace Tests
{
    public class CertificateVerifier
    {
        /// <summary>
        ///     Проверка цепочки сертификата.
        /// </summary>
        public bool VerifyCertificateChain(X509Certificate2 certificate)
        {
            X509Chain certificateChain = new X509Chain
            {
                ChainPolicy =
                {
                    VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid,
                    RevocationMode = X509RevocationMode.NoCheck
                }
            };

            return certificateChain.Build(certificate);
        }

        /// <summary>
        ///     Проверка срока действия сертификата.
        /// </summary>
        public bool VerifyCertificateExpiration(X509Certificate2 certificate)
        {
            return (certificate.NotAfter >= DateTime.Now) && (certificate.NotBefore <= DateTime.Now);
        }

        /// <summary>
        ///     Проверка, не отозван ли сертификат.
        /// </summary>
        public bool VerifyCertificateForRevocation(X509Certificate2 certificate)
        {
            X509Chain chain = new X509Chain
            {
                ChainPolicy =
                {
                    RevocationMode = X509RevocationMode.Online,
                    VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid,
                    RevocationFlag = X509RevocationFlag.ExcludeRoot
                }
            };

            return chain.Build(certificate);
        }
    }
}