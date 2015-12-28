using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace SignLib
{
    public class SignedLargeCms : IDisposable
    {
        #region "FIELDS"

        // Crypto message handle
        private SafeMsgHandle m_hMsg;

        // File stream to use in callback function
        private FileStream m_callbackFile;

        #endregion

        #region "PROPERTIES"

        // Signing time
        public DateTime[] SigningTimes
        {
            get
            {
                return Win32.GetSigningTimes(m_hMsg);
            }
        }

        // Get all certificates in the message
        public X509Certificate2Collection Certificates
        {
            get
            {
                return Win32.GetCerts(m_hMsg);
            }
        }

        // Get all signer certificates in the message
        public X509Certificate2Collection SignerCertificates
        {
            get
            {
                return Win32.GetSignerCerts(m_hMsg);
            }
        }

        #endregion

        #region "METHODS"

        // Constructor
        public SignedLargeCms()
        {
            m_hMsg = SafeMsgHandle.Null;
            m_callbackFile = null;
        }

        // Streaming callback function for encoding/decoding
        private Boolean StreamOutputCallback(IntPtr pvArg, IntPtr pbData, int cbData, Boolean fFinal)
        {
            // Check parameters
            if (pbData.Equals(IntPtr.Zero) || cbData == 0)
            {
                return true;
            }
            if (m_callbackFile == null)
            {
                return true;
            }

            // Write all bytes to encoded file
            Byte[] bytes = new Byte[cbData];
            Marshal.Copy(pbData, bytes, 0, cbData);
            m_callbackFile.Write(bytes, 0, cbData);

            if (fFinal)
            {
                // This is the last piece. Close the file
                m_callbackFile.Flush();
                m_callbackFile.Close();
                m_callbackFile = null;
            }

            return true;
        }

        // Encode and sign CMS with streaming to support large data
        public void EncodeAndSign(X509Certificate2 cert, bool bDetached, FileStream dataFile, FileStream encodedFile)
        {
            // Variables
            Win32.CRYPT_ATTRIBUTE CryptAttribute = new Win32.CRYPT_ATTRIBUTE();
            Win32.CMSG_SIGNER_ENCODE_INFO SignerEncodeInfo = new Win32.CMSG_SIGNER_ENCODE_INFO(); ;
            Win32.CMSG_SIGNED_ENCODE_INFO SignedEncodeInfo = new Win32.CMSG_SIGNED_ENCODE_INFO(); ;
            Win32.CMSG_STREAM_INFO StreamInfo = new Win32.CMSG_STREAM_INFO(); ;
            X509Chain chain = null;
            X509Certificate2Collection certs = null;
            SafeNTHeapHandle pbEncodedSigningTime = SafeNTHeapHandle.Null;
            int cbEncodedSigningTime = 0;

            // Check parameters
            if (dataFile == null)
            {
                throw new Exception("EncodeAndSign error: Missing data file stream");
            }
            if (encodedFile == null)
            {
                throw new Exception("EncodeAndSign error: Missing encoded file stream");
            }

            try
            {
                // Prepare stream for encoded info
                m_callbackFile = encodedFile;

                // Get certs in cert chain
                chain = new X509Chain();
                chain.Build(cert);
                certs = new X509Certificate2Collection();
                foreach (X509ChainElement chainElement in chain.ChainElements)
                {
                    certs.Add(chainElement.Certificate);
                }

                // Add signing time auth attribute
                Win32.EncodeSigningTime(DateTime.Now, out pbEncodedSigningTime, out cbEncodedSigningTime);
                CryptAttribute = Win32.CreateCryptAttribute(Win32.szOID_RSA_signingTime, pbEncodedSigningTime, cbEncodedSigningTime);

                // Specify signer
                SignerEncodeInfo = Win32.CreateSignerEncodeInfo(cert, CryptAttribute);

                // Add all certs in chain to the message
                SignedEncodeInfo = Win32.CreateSignedEncodeInfo(certs, SignerEncodeInfo);

                // Set callback for streaming
                StreamInfo = Win32.CreateStreamInfo((int)dataFile.Length, new Win32.StreamOutputCallbackDelegate(StreamOutputCallback));

                // Open message to encode
                m_hMsg = Win32.OpenMessageToEncode(SignedEncodeInfo, StreamInfo, bDetached);

                // Process the whole message
                Win32.ProcessMessage(m_hMsg, dataFile);
            }
            finally
            {
                // Clean up
                pbEncodedSigningTime.Dispose();
                CryptAttribute.Dispose();
                SignerEncodeInfo.Dispose();
                SignedEncodeInfo.Dispose();
            }
        }

        // Cosign CMS with streaming to support large data
        public void CoSign(X509Certificate2 cert, FileStream dataFile, FileStream encodedFile, FileStream decodedFile, FileStream reencodedFile)
        {
            // Variables
            Win32.CRYPT_ATTRIBUTE CryptAttribute = new Win32.CRYPT_ATTRIBUTE();
            Win32.CMSG_SIGNER_ENCODE_INFO SignerEncodeInfo = new Win32.CMSG_SIGNER_ENCODE_INFO(); ;
            X509Chain chain = null;
            X509Certificate2Collection certs = null;
            SafeNTHeapHandle pbEncodedSigningTime = SafeNTHeapHandle.Null;
            int cbEncodedSigningTime = 0;
            int iCount = 0;

            // Decode data
            Decode(dataFile, encodedFile, decodedFile);

            try
            {
                // Check if there is already a signer
                iCount = Win32.GetSignerCount(m_hMsg);
                if (iCount == 0)
                {
                    throw new Exception("CoSign error: Message not signed");
                }

                // Add signing time auth attribute
                Win32.EncodeSigningTime(DateTime.Now, out pbEncodedSigningTime, out cbEncodedSigningTime);
                CryptAttribute = Win32.CreateCryptAttribute(Win32.szOID_RSA_signingTime, pbEncodedSigningTime, cbEncodedSigningTime);

                // Specify signer
                SignerEncodeInfo = Win32.CreateSignerEncodeInfo(cert, CryptAttribute);

                // Add signer to message
                Win32.AddSigner(m_hMsg, SignerEncodeInfo);

                // Get certs in cert chain
                chain = new X509Chain();
                chain.Build(cert);
                certs = new X509Certificate2Collection();
                foreach (X509ChainElement chainElement in chain.ChainElements)
                {
                    certs.Add(chainElement.Certificate);
                }

                // Add certs to message
                Win32.AddCerts(m_hMsg, certs);

                // Get update message (note that we will get a DETACHED message here!!!)               
                Win32.UpdateMessage(m_hMsg, reencodedFile);

            }
            finally
            {
                // Clean up
                pbEncodedSigningTime.Dispose();
                CryptAttribute.Dispose();
                SignerEncodeInfo.Dispose();
            }
        }

        // Decode CMS with streaming to support large data
        public void Decode(FileStream dataFile, FileStream encodedFile, FileStream decodedFile)
        {
            // Variables
            Win32.CMSG_STREAM_INFO StreamInfo = new Win32.CMSG_STREAM_INFO();

            // Check parameters
            if (encodedFile == null)
            {
                throw new Exception("Decode error: Missing encoded file stream");
            }

            // Prepare stream for decoded data
            m_callbackFile = decodedFile;

            // Set callback for streaming
            StreamInfo = Win32.CreateStreamInfo((int)encodedFile.Length, new Win32.StreamOutputCallbackDelegate(StreamOutputCallback));

            // Detached message
            if (dataFile != null)
            {
                // Open message to decode
                m_hMsg = Win32.OpenMessageToDecode(StreamInfo, true);

                // Process encoded message
                Win32.ProcessMessage(m_hMsg, encodedFile);

                // Process original data
                Win32.ProcessMessage(m_hMsg, dataFile);
            }

            // Non-detached message
            else
            {
                // Open message to decode
                m_hMsg = Win32.OpenMessageToDecode(StreamInfo, false);

                // Process encoded message
                Win32.ProcessMessage(m_hMsg, encodedFile);
            }
        }

        // Verify signature
        public void CheckSignature()
        {
            // Check parameters
            if (m_hMsg.IsInvalid)
            {
                throw new Exception("CheckSignature error: Decode method must be called first");
            }

            // Check all signature in the message
            Win32.CheckSignatures(m_hMsg);
        }

        #endregion

        #region "IDisposable"

        // Basic design pattern for implementing Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }

            // Free your own state (unmanaged objects).
            m_hMsg.Dispose();
        }

        ~SignedLargeCms()
        {
            Dispose(false);
        }

        #endregion
    }
}