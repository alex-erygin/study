using Xunit;

namespace Tests
{
    public class SignerTests
    {
        [Fact]
        void Sign_NoException()
        {
            var signer = new SignLib.Signer();
            signer.Sign("Хлюп", "Хлюп", "Хлюп");
        }
    }
}