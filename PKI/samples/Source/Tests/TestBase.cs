using System.Security.Cryptography.X509Certificates;

namespace Tests
{
    public class TestBase
    {
        protected X509Certificate2 GetCertificate()
        {
            X509Store store = new X509Store("My");
            store.Open(OpenFlags.ReadOnly);

            const string GostCertSerial = "01D158DA80087DD00000160B10E10DC1";

            foreach (var certificate in store.Certificates)
            {
                if (certificate.SerialNumber == GostCertSerial)
                {
                    return certificate;
                }
            }

            return null;
        }
    }
}