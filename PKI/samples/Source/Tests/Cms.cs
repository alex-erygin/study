using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Tests
{
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

            X509Store store = new X509Store("My");
            store.Open(OpenFlags.ReadOnly);

            //Типов идентификаторов можнт быть несколько.
            //В даннои примере обрабатываются только идентификаторы типа SubjectIdentifierType.IssuerAndSerialNumber.
            RecipientInfo recipientInfo = envelopedCms.RecipientInfos.Cast<RecipientInfo>()
                .FirstOrDefault(x => FindCertificate((X509IssuerSerial)x.RecipientIdentifier.Value) != null);

            if (recipientInfo == null)
            {
                throw new InvalidOperationException("Не найден сертификат для расшифрования.");
            }

            envelopedCms.Decrypt(recipientInfo);
            return envelopedCms.ContentInfo.Content;
        }

        private X509Certificate2 FindCertificate(X509IssuerSerial issuerSerial)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            try
            {
                return store.Certificates
                    .Find(X509FindType.FindByIssuerDistinguishedName, issuerSerial.IssuerName, false)
                    .Find(X509FindType.FindBySerialNumber, issuerSerial.SerialNumber, false)
                    .Cast<X509Certificate2>()
                    .FirstOrDefault();
            }
            finally
            {
                store.Close();
            }
        }
    }
}