using System.IO;
using Xunit;

namespace Tests
{
    public class SignerTests
    {
        [Fact(Timeout = 1200000)]
        void Sign_NoException()
        {
            var signer = new SignLib.Signer();
            var cert = TestHelper.GetCertificateWithPrivateKey();
            const string OutputPath = @"f:\_VM\output.bin";
            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
            signer.Sign(cert, @"f:\_VM\tls.cpp", OutputPath);
        }
    }
}