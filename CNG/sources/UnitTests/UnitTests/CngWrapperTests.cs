using System.Linq;
using NUnit.Framework;
using НарlochromisBurtoni;
using НарlochromisBurtoni.Security.Cryptography;
using НарlochromisBurtoni.Security.Cryptography.BCrypt;

namespace UnitTests
{
    [TestFixture]
    public class CngWrapperTests
    {
        private const int KeySizeInBytes = 16;

        [Test]
        public void GenerateSymmetricKey_ValidArguments_ValidKeyHandle()
        {
            var wrapper = new CngWrapper();
            using (var key = wrapper.GenerateSymmetricKey(CngProvider2.MicrosoftPrimitiveAlgorithmProvider, AlgorithmName.Aes, 16))
            {
                Assert.NotNull(key);
                Assert.IsFalse(key.IsClosed);
                Assert.IsFalse(key.IsInvalid);
            }
        }

        [Test]
        public void GenerateRandomBytes_GeneratesRandomBytes()
        {
            var wrapper = new CngWrapper();
            byte[] array1 = wrapper.GenerateRandomBytes(CngProvider2.MicrosoftPrimitiveAlgorithmProvider, 10);
            byte[] array2 = wrapper.GenerateRandomBytes(CngProvider2.MicrosoftPrimitiveAlgorithmProvider, 10);

            Assert.AreNotEqual(array1, array2);
        }

        [Test]
        public void ExportSymmetricKey_ValidKeys_ExportedKey()
        {
            var wrapper = new CngWrapper();
            using (
                var protectionKey = wrapper.GenerateSymmetricKey(CngProvider2.MicrosoftPrimitiveAlgorithmProvider,
                                                                 AlgorithmName.Aes, KeySizeInBytes))
            {
                byte[] exportedKey = GetExportedKey(protectionKey);
                Assert.IsNotEmpty(exportedKey);
            }
        }


        [Test]
        public void ExportSymmetricKey_PublicKeyToExport_NotEmptyExportedKey()
        {
            var wrapper = new CngWrapper();
            using (var algorithm = wrapper.OpenAlgorithm(AlgorithmName.ECDH_P256, CngProvider2.MicrosoftPrimitiveAlgorithmProvider.CngProvider))
            {
                using (var keyPair = wrapper.GenerateKeyPair(algorithm, 256))
                {
                    using (var protectionKey = new SafeBCryptKeyHandle())
                    {
                        byte[] exportedKey = wrapper.ExportSymmetricKey(keyPair, protectionKey, BlobTypes.PublicKeyBlobType);
                        Assert.IsNotEmpty(exportedKey);
                    }
                }
            }
        }


        [Test]
        public void ImportPublicKey_ExportedPublicKey_ValidKey()
        {
            var wrapper = new CngWrapper();
            using (var algorithm = wrapper.OpenAlgorithm(AlgorithmName.ECDH_P256, CngProvider2.MicrosoftPrimitiveAlgorithmProvider.CngProvider))
            {
                using (var keyPair = wrapper.GenerateKeyPair(algorithm, 256))
                {
                    using (var protectionKey = new SafeBCryptKeyHandle())
                    {
                        byte[] exportedKey = wrapper.ExportSymmetricKey(keyPair, protectionKey, BlobTypes.PublicKeyBlobType);

                        var importedKey = wrapper.ImportPublicKey(exportedKey,
                                                                  CngProvider2.MicrosoftPrimitiveAlgorithmProvider,
                                                                  AlgorithmName.ECDH_P256);

                        Assert.IsFalse(importedKey.IsClosed);
                        Assert.IsFalse(importedKey.IsInvalid);
                    }
                }
            }
        }


        [Test]
        public void ImportSymmetricKey_ValidExportedKey_ValidImportedKeyHandle()
        {
            var wrapper = new CngWrapper();
            using (
                var protectionKey = wrapper.GenerateSymmetricKey(CngProvider2.MicrosoftPrimitiveAlgorithmProvider,
                                                                 AlgorithmName.Aes, KeySizeInBytes))
            {
                byte[] exportedKey = GetExportedKey(protectionKey);
                using (SafeBCryptKeyHandle importedKey = wrapper.ImportSymmetricKey(
                    AlgorithmName.Aes,
                    CngProvider2.MicrosoftPrimitiveAlgorithmProvider,
                    protectionKey,
                    exportedKey,
                    BlobTypes.AesWrapKeyBlobType))
                {
                    Assert.IsFalse(importedKey.IsClosed);
                    Assert.IsFalse(importedKey.IsInvalid);
                }
            }
        }

        
        [Test]
        public void SymmetrycEncrypt_ValidParameters_EncryptedData()
        {
            var wrapper = new CngWrapper();
            var key = wrapper.GenerateSymmetricKey(CngProvider2.MicrosoftPrimitiveAlgorithmProvider, AlgorithmName.Aes,
                                                   KeySizeInBytes);

            var data = new byte[32];
            byte[] encrypted = wrapper.SymmetrycEncrypt(key, new byte[16], data);
            Assert.AreNotEqual(data, encrypted);
            Assert.IsNotEmpty(encrypted);
        }
         

        [Test]
        public void SymmetrycDecrypt_ValidParameters_DecryptedData()
        {
            var wrapper = new CngWrapper();
            var key = wrapper.GenerateSymmetricKey(CngProvider2.MicrosoftPrimitiveAlgorithmProvider, AlgorithmName.Aes,
                                                   KeySizeInBytes);

            var data = Enumerable.Range(1, 32).Select(x=>(byte)x).ToArray();
            
            byte[] encrypted = wrapper.SymmetrycEncrypt(key, new byte[16], data);
            byte[] decrypted = wrapper.SymmetrycDecrypt(key, new byte[16], encrypted);

            Assert.AreEqual(data, decrypted);
        }


        [Test]
        public void ComputeHash_NotEmptyByteArray_Hash()
        {
            var wrapper = new CngWrapper();
            var data = Enumerable.Range(1, 32).Select(x => (byte)x).ToArray();
            var hash = wrapper.ComputeHash(data);
            Assert.IsNotEmpty(hash);
        }

        [Test]
        public void ComputeHash_DamageData_HashChanged()
        {
            var wrapper = new CngWrapper();
            var data = Enumerable.Range(1, 32).Select(x => (byte)x).ToArray();
            var hash = wrapper.ComputeHash(data);
            data[0] = data[0] += 50;
            var newHash = wrapper.ComputeHash(data);
            Assert.AreNotEqual(hash, newHash);
        }

        [Test]
        public void GenerateKeyPair_ValidParameters_ValidKeyHandle()
        {
            var wrapper = new CngWrapper();
            using (var algorithm = wrapper.OpenAlgorithm(AlgorithmName.ECDH_P256, CngProvider2.MicrosoftPrimitiveAlgorithmProvider.CngProvider))
            {
                using (var keyPair = wrapper.GenerateKeyPair(algorithm, 256))
                {
                    Assert.IsNotNull(keyPair);
                    Assert.IsFalse(keyPair.IsInvalid);
                    Assert.IsFalse(keyPair.IsClosed);
                }
            }
        }

        private byte[] GetExportedKey(SafeBCryptKeyHandle protectionKey)
        {
            var wrapper = new CngWrapper();
            const int keySize = 16;
            using (var keyToExport = wrapper.GenerateSymmetricKey(CngProvider2.MicrosoftPrimitiveAlgorithmProvider, AlgorithmName.Aes, keySize))
            {
                byte[] exportedKey = wrapper.ExportSymmetricKey(keyToExport, protectionKey, BlobTypes.AesWrapKeyBlobType);
                return exportedKey;
            }
        }
    }
}