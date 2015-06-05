using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using НарlochromisBurtoni.Security.Cryptography;
using НарlochromisBurtoni.Security.Cryptography.BCrypt;

namespace НарlochromisBurtoni
{
    /// <summary>
    /// CNG Wrapper.
    /// </summary>
    public sealed class CngWrapper
    {
        /// <summary>
        /// Сгенерировать симметричный ключ.
        /// </summary>
        /// <param name="provider"><see cref="CngProvider2"/>.</param>
        /// <param name="algorithm">Алгоритм.</param>
        /// <param name="keySizeInBytes">Размер ключа в байтах.</param>
        /// <returns><see cref="SafeBCryptKeyHandle"/>.</returns>
        public SafeBCryptKeyHandle GenerateSymmetricKey(CngProvider2 provider, string algorithm, int keySizeInBytes)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (string.IsNullOrWhiteSpace(algorithm))
            {
                throw new ArgumentNullException("algorithm");
            }

            using (SafeBCryptAlgorithmHandle alg = BCryptNative.OpenAlgorithm(
                algorithm,
                provider.Provider,
                AlgorithmProviderOptions.None
                ))
            {
                var keyMaterial = new byte[keySizeInBytes];
                using (var rng = new RngCng(provider.CngProvider))
                {
                    rng.GetBytes(keyMaterial);
                }

                SafeBCryptKeyHandle key;
                ErrorCode errorCode = BCryptNative.BCryptGenerateSymmetricKey(alg, out key, null, 0, keyMaterial, keyMaterial.Length, 0);
                if (errorCode != ErrorCode.Success)
                {
                    throw new CryptographicException(Win32Native.GetNTStatusMessage((int)errorCode));
                }
                return key;
            }
        }
    
        /// <summary>
        /// Сгенерировать массив байт, заполненный случайными числами.
        /// </summary>
        /// <param name="provider"><see cref="CngProvider2"/>.</param>
        /// <param name="size">Размер массива.</param>
        /// <returns>Массив байт, заполненный случайными числами.</returns>
        public byte[] GenerateRandomBytes(CngProvider2 provider, int size)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            using (var rng = new RngCng(provider.CngProvider))
            {
                var result = new byte[size];
                rng.GetBytes(result);
                return result;
            }
        }


        /// <summary>
        /// Экспортировать симметричный ключ, защищенный на ключе <paramref name="protectionKey"/>.
        /// </summary>
        /// <param name="keyToExport">Ключ, который нужно экспортировать.</param>
        /// <param name="protectionKey">Ключ защиты.</param>
        /// <param name="blobType">Тип блоба.</param>
        /// <returns>Экспортированный симметричный ключ.</returns>
        public byte[] ExportSymmetricKey(SafeBCryptKeyHandle keyToExport, SafeBCryptKeyHandle protectionKey, string blobType)
        {
            if (keyToExport == null)
            {
                throw new ArgumentNullException("keyToExport");
            }

            if (protectionKey == null)
            {
                throw new ArgumentNullException("protectionKey");
            }

            int blobSize = 0;

            ErrorCode errorCode = BCryptNative.BCryptExportKey(keyToExport, protectionKey, blobType, null, 0, ref blobSize, 0);
            if (errorCode != ErrorCode.Success)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)errorCode));
            }
            
            var keyBlob = new byte[blobSize];
            errorCode = BCryptNative.BCryptExportKey(keyToExport, protectionKey, blobType, keyBlob, keyBlob.Length,
                                                     ref blobSize, 0);
            if (errorCode != ErrorCode.Success)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)errorCode));
            }

            return keyBlob;
        }


        /// <summary>
        /// Импортировать симметричный ключ.
        /// </summary>
        /// <returns>Импортированный ключ.</returns>
        public SafeBCryptKeyHandle ImportSymmetricKey(string algorithmName, CngProvider2 provider,
                                                      SafeBCryptKeyHandle protectionKey, byte[] keyBlob, string blobType)
        {
            IntPtr keyDataBuffer = IntPtr.Zero;
            SafeBCryptKeyHandle result = null;

            try
            {
                using (SafeBCryptAlgorithmHandle algorithm = BCryptNative.OpenAlgorithm(algorithmName, provider.Provider))
                {
                    int keyDataSize = BCryptNative.GetInt32Property(algorithm, ObjectPropertyName.ObjectLength);
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                    }
                    finally
                    {
                        keyDataBuffer = Marshal.AllocCoTaskMem(keyDataSize);
                    }

                    var errorCode = UnsafeNativeMethods.BCryptImportKey(
                        algorithm,
                        protectionKey,
                        blobType,
                        out result,
                        keyDataBuffer,
                        keyDataSize,
                        keyBlob,
                        keyBlob.Length,
                        0);
                    if (errorCode != ErrorCode.Success)
                    {
                        throw new CryptographicException(Win32Native.GetNTStatusMessage((int) errorCode));
                    }

                    result.DataBuffer = keyDataBuffer;
                    return result;
                }
            }
            finally
            {
                // If we allocated a key data buffer, but never transfered ownership to the key handle, then
                // we need to free it now otherwise it will leak.
                if (keyDataBuffer != IntPtr.Zero)
                {
                    if (result == null || result.DataBuffer == IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(keyDataBuffer);
                    }
                }
            }
        }


        /// <summary>
        /// Зашифровать.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="iv">Вектор инициализации.</param>
        /// <param name="input">Данные для шифрования. (Длига должна быть кратной размеру ключа в байтах.)</param>
        /// <returns>Зшифрованные данные.</returns>
        public byte[] SymmetrycEncrypt(SafeBCryptKeyHandle key, byte[] iv, byte[] input)
        {
            byte[] result = new byte[input.Length];
            int resultSize = 0;

            ErrorCode error = UnsafeNativeMethods.BCryptEncrypt(
                key,
                input,
                input.Length,
                IntPtr.Zero,
                iv,
                iv != null ? iv.Length : 0,
                result,
                result.Length,
                out resultSize,
                0);

            if (error != ErrorCode.Success)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)error));
            }

            // If we didn't use the whole output array, trim down to the portion that was used
            if (resultSize != result.Length)
            {
                byte[] trimmedOutput = new byte[resultSize];
                Buffer.BlockCopy(result, 0, trimmedOutput, 0, trimmedOutput.Length);
                result = trimmedOutput;
            }

            return result;
        }


        /// <summary>
        /// Импортировать открытый ключ.
        /// </summary>
        /// <param name="publicKeyBlob">Экспортированный открытый ключ.</param>
        /// <param name="provider"><see cref="CngProvider2"/>.</param>
        /// <param name="algorithm">Алгоритм.</param>
        /// <returns>Импортированный ключ.</returns>
        public SafeBCryptKeyHandle ImportPublicKey(byte[] publicKeyBlob, CngProvider2 provider, string algorithm)
        {
            using (SafeBCryptAlgorithmHandle algorithmHandle = OpenAlgorithm(algorithm, provider.CngProvider))
            {
                SafeBCryptKeyHandle key;
                ErrorCode status = UnsafeNativeMethods.BCryptImportKeyPair(
                    algorithmHandle,
                    IntPtr.Zero,
                    BlobTypes.PublicKeyBlobType,
                    out key,
                    publicKeyBlob,
                    publicKeyBlob.Length,
                    0);
                if (status != ErrorCode.Success)
                {
                    throw new CryptographicException(Win32Native.GetNTStatusMessage((int)status));
                }
                return key;
            }
        }


        /// <summary>
        /// Расшифровать.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="iv">Вектор инициализации.</param>
        /// <param name="input">Данные для расшифрования.</param>
        /// <returns>Расшифрованные данные.</returns>
        public byte[] SymmetrycDecrypt(SafeBCryptKeyHandle key, byte[] iv, byte[] input)
        {
            byte[] output = new byte[input.Length];
            int outputSize = 0;
            ErrorCode error = UnsafeNativeMethods.BCryptDecrypt(key, input, input.Length, IntPtr.Zero, iv,
                                                                iv != null ? iv.Length : 0,
                                                                output, output.Length, out outputSize, 0);

            if (error != ErrorCode.Success)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)error));
            }

            // If we didn't use the whole output array, trim down to the portion that was used
            if (outputSize != output.Length)
            {
                byte[] trimmedOutput = new byte[outputSize];
                Buffer.BlockCopy(output, 0, trimmedOutput, 0, trimmedOutput.Length);
                output = trimmedOutput;
            }

            return output;
        }

        /// <summary>
        /// Вычислить хеш.
        /// </summary>
        /// <param name="data">Данные.</param>
        /// <returns>Хеш.</returns>
        public byte[] ComputeHash(byte[] data)
        {
            using (SHA512Cng hashAlgorithm = new SHA512Cng())
            {
                hashAlgorithm.Initialize();
                return hashAlgorithm.ComputeHash(data);
            }
        }

        
        /// <summary>
        /// Сгенерировать ключевую пару.
        /// </summary>
        /// <param name="algorithm">Алгоритм.</param>
        /// <param name="keyLengthInBits">Длина ключа.</param>
        /// <returns>Ключевая пара.</returns>
        public SafeBCryptKeyHandle GenerateKeyPair(SafeBCryptAlgorithmHandle algorithm, int keyLengthInBits)
        {
            SafeBCryptKeyHandle keyPair;
            ErrorCode code = UnsafeNativeMethods.BCryptGenerateKeyPair(algorithm, out keyPair, keyLengthInBits, 0);
            
            if (code != ErrorCode.Success)
            {
                throw new CryptographicException(
                    Win32Native.GetNTStatusMessage((int)code));
            }

            code = UnsafeNativeMethods.BCryptFinalizeKeyPair(keyPair, 0);
            if (code != ErrorCode.Success)
            {
                keyPair.Dispose();
                throw new System.Security.Cryptography.CryptographicException(
                    Win32Native.GetNTStatusMessage((int)code));
            }

            return keyPair;
        }


        /// <summary>
        /// Открыть алгоритм.
        /// </summary>
        /// <param name="algorithmName">Имя алгоритма.</param>
        /// <param name="provider">Провайдера.</param>
        /// <returns>Хендл алгоритма.</returns>
        [SecurityCritical]
        public SafeBCryptAlgorithmHandle OpenAlgorithm(string algorithmName, CngProvider provider)
        {
            return BCryptNative.OpenAlgorithm(
                algorithmName,
                provider.Provider,
                AlgorithmProviderOptions.None);
        }
    }
}