using Xunit;

namespace Tests
{
    public class SignerTests
    {
        [Fact]
        void Sign_NoException()
        {
            var signer = new SignLib.Signer();
            var cert = TestHelper.GetCertWithPrivateKey();

            signer.Sign(cert, @"f:\_VM\tls.cpp", "fdfdf");
        }
    }
}