using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;

namespace Tests
{
    public class CmsTests : TestBase
    {
        [Fact]
        public void SignAttached_CertificateAndData_NotEmptySign()
        {
            X509Certificate2 certificate = GetCertificate();
            byte[] dataToSign = Encoding.UTF8.GetBytes("Sign me sign me");
            Cms cms = new Cms();
            Assert.NotEmpty(cms.SignAttached(certificate, dataToSign));
        }

        [Fact]
        public void SignDetached_CertificateAndData_NotEmptySign()
        {
            X509Certificate2 certificate = GetCertificate();
            byte[] dataToSign = Encoding.UTF8.GetBytes("Sign me sign me");
            Cms cms = new Cms();
            Assert.NotEmpty(cms.SignDetached(certificate, dataToSign));
        }

        [Fact]
        public void GetOriginalData_SignedData_OriginalData()
        {
            X509Certificate2 certificate = GetCertificate();
            byte[] dataToSign = Encoding.UTF8.GetBytes("Sign me sign me");
            Cms cms = new Cms();
            var attachedSignature = cms.SignAttached(certificate, dataToSign);
            var originalData = cms.GetOriginalData(attachedSignature);

            Assert.Equal(dataToSign, originalData);
        }

        [Fact]
        public void VerifyAttached_ValidAttachedSignature_ReturnsTrue()
        {
            X509Certificate2 certificate = GetCertificate();
            byte[] dataToSign = Encoding.UTF8.GetBytes("Sign me sign me");
            Cms cms = new Cms();
            var attachedSignature = cms.SignAttached(certificate, dataToSign);
            Assert.True(cms.VerifyAttached(attachedSignature));
        }

        [Fact]
        public void VerifyAttached_Trash_ReturnsFalse()
        {
            X509Certificate2 certificate = GetCertificate();
            byte[] dataToSign = Encoding.UTF8.GetBytes("Sign me sign me");
            Cms cms = new Cms();
            var attachedSignature = new byte[] {1, 2, 3, 4, 4, 5, 6};
            Assert.False(cms.VerifyAttached(attachedSignature));
        }

        [Fact]
        public void Encrypt_SomeContentAndRecipientsCertificare_EncryptedContent()
        {
            var cms = new Cms();
            var content = Encoding.UTF8.GetBytes("Zag-Zag!");
            var encrypted = cms.Encrypt(content, GetCertificate());
            Assert.NotEmpty(encrypted);
            Assert.NotEqual(content, encrypted);
        }

        [Fact]
        public void Decrypt_EncryptedData_DecryptedData()
        {
            var cms = new Cms();
            var original = Encoding.UTF8.GetBytes("Zag-Zag!");
            var encrypted = cms.Encrypt(original, GetCertificate());
            var decrypted = cms.Decrypt(encrypted);
            Assert.Equal(original, decrypted);
        }
    }


    public class Cms
    {
        /// <summary>
        ///     Прикрепленная подпись.
        /// </summary>
        public byte[] SignAttached(X509Certificate2 certificate, byte[] dataToSign)
        {
            ContentInfo contentInfo = new ContentInfo(dataToSign);
            SignedCms cms = new SignedCms(contentInfo, false);
            CmsSigner signer = new CmsSigner(certificate);
            cms.ComputeSignature(signer, false);
            return cms.Encode();
        }

        /// <summary>
        ///     Открепленная подпись.
        /// </summary>
        public byte[] SignDetached(X509Certificate2 certificate, byte[] dataToSign)
        {
            ContentInfo contentInfo = new ContentInfo(dataToSign);
            SignedCms cms = new SignedCms(contentInfo, true);
            CmsSigner signer = new CmsSigner(certificate);
            cms.ComputeSignature(signer, false);
            return cms.Encode();
        }

        /// <summary>
        /// Декодирование подписанных данных из сообщения с прикрепленной подписью.
        /// </summary>    
        public byte[] GetOriginalData(byte[] signedCms)
        {
            var cms = new SignedCms();
            cms.Decode(signedCms);
            return cms.ContentInfo.Content;
        }

        /// <summary>
        /// Проверка прикрепленной подписи.
        /// </summary>
        public bool VerifyAttached(byte[] dataToVerify)
        {
            try
            {
                var cms = new SignedCms();
                cms.Decode(dataToVerify);

                foreach (var signer in cms.SignerInfos)
                {
                    signer.CheckSignature(true);
                }
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        /// <summary>
        /// Шифрование.
        /// </summary>    
        public byte[] Encrypt(byte[] dataToEncrypt, params X509Certificate2[] recepients)
        {
            var contentInfo = new ContentInfo(dataToEncrypt);
            var recipientsCertificates = new X509Certificate2Collection(recepients);
            var recipients = new CmsRecipientCollection(SubjectIdentifierType.IssuerAndSerialNumber, recipientsCertificates);
            var cms = new EnvelopedCms(contentInfo);
            cms.Encrypt(recipients);
            return cms.Encode();
        }

        /// <summary>
        /// Расшифрование.
        /// </summary>
        public byte[] Decrypt(byte[] encryptedData)
        {
            var envelopedCms = new EnvelopedCms();
            envelopedCms.Decode(encryptedData);
            RecipientInfo recipientInfo = envelopedCms.RecipientInfos.Cast<RecipientInfo>().FirstOrDefault();
            envelopedCms.Decrypt(recipientInfo);
            return envelopedCms.ContentInfo.Content;
        }
    }
}