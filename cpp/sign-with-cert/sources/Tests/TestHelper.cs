using System.Security.Cryptography.X509Certificates;

namespace Tests
{
    /// <summary>
    /// Вспомогательный класс для тестов.
    /// </summary>
    public static class TestHelper
    {
        private static readonly string knownCertificateWithPrivateKeyThumbprint =
            "a5 60 9f 3b ab 06 da 9c 1b 19 ac 17 a1 7f 4c 96 70 ec 04 7a";

        /// <summary>
        /// Получить сертификат с закрытым ключом.
        /// </summary>
        /// <returns>Сертификат с закрытым ключом.</returns>
        public static X509Certificate2 GetCertWithPrivateKey()
        {
            X509Certificate2 certificate = FindCertificate(
                knownCertificateWithPrivateKeyThumbprint, StoreName.My, StoreLocation.CurrentUser);
            return certificate;
        }

        /// <summary>
        /// Получить сертификат с новым ГОСТ алгоритмом.
        /// </summary>
        /// <returns>Сертификат.</returns>
        public static X509Certificate2 GetNewGostAlgCert()
        {
            return FindCertificate(
                "ee bc c1 15 8c 19 d9 8f d8 24 9f 50 55 cd 75 c0 2f 3a 7e 86", StoreName.My, StoreLocation.CurrentUser);
        }

        private static X509Certificate2 FindCertificate(
            string certThumb, StoreName storeName, StoreLocation storeLocation)
        {
            var storeMy = new X509Store(storeName, storeLocation);
            storeMy.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2Collection certColl = storeMy.Certificates.Find(
                    X509FindType.FindByThumbprint, certThumb, false);
                return certColl[0];
            }
            finally
            {
                storeMy.Close();
            }
        }
    }
}