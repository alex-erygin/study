using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Tests
{
    public class CertificateVerifierTests : TestBase
    {
        [Fact]
        public void VerifyCertificateChain_ValidCert_ReturnsTrue()
        {
            X509Certificate2 certificate = GetCertificate();
            var verifier = new CertificateVerifier();
            Assert.True(verifier.VerifyCertificateChain(certificate));
        }

        [Fact]
        public void VerifyCertificateExpiration_ValidCert_ReturnsTrue()
        {
            X509Certificate2 certificate = GetCertificate();
            var verifier = new CertificateVerifier();
            Assert.True(verifier.VerifyCertificateExpiration(certificate));
        }

        [Fact]
        public void VerifyCertificateForRevocation_ValidCert_ReturnsTrue()
        {
            X509Certificate2 certificate = GetCertificate();
            var verifier = new CertificateVerifier();
            Assert.True(verifier.VerifyCertificateForRevocation(certificate));
        }
    }
}