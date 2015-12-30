using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using P7Lib;
using Xunit;

namespace UnitTests
{
    public class SignerTests
    {
        [Fact]
        void SignTest()
        {
            var signer = new P7Wrapper();
            signer.Sign(@"data\source.png", @"data\target.dat", GetCertificateWithPrivateKey());
        }

        private static X509Certificate2 GetCertificateWithPrivateKey()
        {
            var password = new SecureString();
            password.AppendChar('1');
            password.AppendChar('2');
            password.AppendChar('3');
            password.AppendChar('1');
            password.AppendChar('2');
            password.AppendChar('3');
            var cert = new X509Certificate2(File.ReadAllBytes(@"certificates\CertWithPrivateKey.pfx"), password);
            return cert;
        }
    }
}