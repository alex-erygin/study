using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SignLib
{
    partial class Win32
    {
        #region "HELPER FUNCTIONS"

        public static SafeCSPHandle GetCSPHandle(X509Certificate2 cert)
        {
            // Variables
            SafeCSPHandle hProv = SafeCSPHandle.Null;
            RSACryptoServiceProvider key = null;
            bool bResult = false;

            // Get CSP handle            
            key = (RSACryptoServiceProvider)cert.PrivateKey;
            bResult = CryptAcquireContext(
                out hProv,
                key.CspKeyContainerInfo.KeyContainerName,
                key.CspKeyContainerInfo.ProviderName,
                key.CspKeyContainerInfo.ProviderType,
                0
                );
            if (!bResult)
            {
                throw new Exception("CryptAcquireContext error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }

            // Return handle
            return hProv;
        }

        public static void EncodeSigningTime(DateTime signingTime, out SafeNTHeapHandle pbEncodedSigningTime, out int cbEncodedSigningTime)
        {
            // Variables
            long lFileTime = 0;
            bool bResult = false;

            // Get time in filetime format
            lFileTime = signingTime.ToFileTime();

            // Get size for encoded data
            pbEncodedSigningTime = SafeNTHeapHandle.Null;
            cbEncodedSigningTime = 0;
            bResult = CryptEncodeObject(
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                szOID_RSA_signingTime,
                ref lFileTime,
                pbEncodedSigningTime,
                ref cbEncodedSigningTime
                );
            if (!bResult)
            {
                throw new Exception("CryptEncodeObject error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }

            // Create buffer for encoded data
            pbEncodedSigningTime = new SafeNTHeapHandle(Marshal.AllocHGlobal(cbEncodedSigningTime));

            // Get encoded data
            bResult = CryptEncodeObject(
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                szOID_RSA_signingTime,
                ref lFileTime,
                pbEncodedSigningTime,
                ref cbEncodedSigningTime
                );
            if (!bResult)
            {
                throw new Exception("CryptEncodeObject error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }

        public static CRYPT_ATTRIBUTE CreateCryptAttribute(string pszObjId, SafeNTHeapHandle pbEncodedData, int cbEncodedData)
        {
            // Variables
            BLOB CryptAttrBlob = new BLOB();
            CRYPT_ATTRIBUTE CryptAttr = new CRYPT_ATTRIBUTE();

            // Populate Crypt Attribute struct
            CryptAttrBlob.cbData = cbEncodedData;
            CryptAttrBlob.pbData = pbEncodedData.DangerousGetHandle();

            CryptAttr.pszObjId = pszObjId;
            CryptAttr.cValue = 1;
            CryptAttr.rgValue = Marshal.AllocHGlobal(Marshal.SizeOf(CryptAttrBlob));
            Marshal.StructureToPtr(CryptAttrBlob, CryptAttr.rgValue, false);

            // Return the struct
            return CryptAttr;
        }

        public static CMSG_SIGNER_ENCODE_INFO CreateSignerEncodeInfo(X509Certificate2 cert, CRYPT_ATTRIBUTE CryptAttr)
        {
            // Variables
            CERT_CONTEXT CertContext = new CERT_CONTEXT();
            CMSG_SIGNER_ENCODE_INFO SignerEncodeInfo = new CMSG_SIGNER_ENCODE_INFO();
            SafeCSPHandle hProv = SafeCSPHandle.Null;

            // Get CSP of certificate
            hProv = GetCSPHandle(cert);

            // Get context of certificate
            CertContext = (CERT_CONTEXT)Marshal.PtrToStructure(cert.Handle, typeof(CERT_CONTEXT));

            // Populate Signer Info struct
            SignerEncodeInfo.cbSize = Marshal.SizeOf(SignerEncodeInfo);
            SignerEncodeInfo.pCertInfo = CertContext.pCertInfo;
            SignerEncodeInfo.hCryptProvOrhNCryptKey = hProv.DangerousGetHandle();
            GC.SuppressFinalize(hProv);
            SignerEncodeInfo.dwKeySpec = (int)((RSACryptoServiceProvider)cert.PrivateKey).CspKeyContainerInfo.KeyNumber;
            SignerEncodeInfo.HashAlgorithm.pszObjId = szOID_OIWSEC_sha1;
            // Add auth attribute to it
            SignerEncodeInfo.cAuthAttr = 1;
            SignerEncodeInfo.rgAuthAttr = Marshal.AllocHGlobal(Marshal.SizeOf(CryptAttr));
            Marshal.StructureToPtr(CryptAttr, SignerEncodeInfo.rgAuthAttr, false);

            // Return the struct
            return SignerEncodeInfo;
        }

        public static CMSG_SIGNED_ENCODE_INFO CreateSignedEncodeInfo(X509Certificate2Collection certs, CMSG_SIGNER_ENCODE_INFO SignerInfo)
        {
            // Variables 
            CMSG_SIGNED_ENCODE_INFO SignedEncodeInfo = new CMSG_SIGNED_ENCODE_INFO();
            CERT_CONTEXT[] CertContexts = null;
            BLOB[] CertBlobs = null;
            int iCount = 0;

            // Get context of all certs
            CertContexts = new CERT_CONTEXT[certs.Count];
            foreach (X509Certificate2 cert in certs)
            {
                CertContexts[iCount] = (CERT_CONTEXT)Marshal.PtrToStructure(cert.Handle, typeof(CERT_CONTEXT));
                iCount++;
            }

            // Get cert blob of all certs
            CertBlobs = new BLOB[CertContexts.Length];
            for (iCount = 0; iCount < CertContexts.Length; iCount++)
            {
                CertBlobs[iCount].cbData = CertContexts[iCount].cbCertEncoded;
                CertBlobs[iCount].pbData = CertContexts[iCount].pbCertEncoded;
            }

            // Populate Signed Encode Info struct
            SignedEncodeInfo.cbSize = Marshal.SizeOf(SignedEncodeInfo);

            SignedEncodeInfo.cSigners = 1;
            SignedEncodeInfo.rgSigners = Marshal.AllocHGlobal(Marshal.SizeOf(SignerInfo));
            Marshal.StructureToPtr(SignerInfo, SignedEncodeInfo.rgSigners, false);

            SignedEncodeInfo.cCertEncoded = CertBlobs.Length;
            SignedEncodeInfo.rgCertEncoded = Marshal.AllocHGlobal(Marshal.SizeOf(CertBlobs[0]) * CertBlobs.Length);
            for (iCount = 0; iCount < CertBlobs.Length; iCount++)
            {
                Marshal.StructureToPtr(CertBlobs[iCount], new IntPtr(SignedEncodeInfo.rgCertEncoded.ToInt64() + (Marshal.SizeOf(CertBlobs[iCount]) * iCount)), false);
            }

            return SignedEncodeInfo;
        }

        public static CMSG_ENVELOPED_ENCODE_INFO CreateEnvelopedEncodeInfo(X509Certificate2Collection recipients, string algorithm)
        {
            // Variables
            CMSG_ENVELOPED_ENCODE_INFO EnvelopedEncodeInfo = new CMSG_ENVELOPED_ENCODE_INFO();
            CMSG_RC4_AUX_INFO AuxInfo = new CMSG_RC4_AUX_INFO();
            CERT_CONTEXT[] RecipientContexts = null;
            CERT_CONTEXT[] CertContexts = null;
            BLOB[] CertBlobs = null;
            X509Chain chain = null;
            X509Certificate2Collection certs = null;
            int iCount = 0;

            // Get context of all recipients, and all certs in their cert chains (without duplicates)
            RecipientContexts = new CERT_CONTEXT[recipients.Count];
            certs = new X509Certificate2Collection();
            iCount = 0;
            foreach (X509Certificate2 recipient in recipients)
            {
                // Context of recipient cert
                RecipientContexts[iCount] = (CERT_CONTEXT)Marshal.PtrToStructure(recipient.Handle, typeof(CERT_CONTEXT));

                // Cert chain of recipient cert
                chain = new X509Chain();
                chain.Build(recipient);
                foreach (X509ChainElement chainElement in chain.ChainElements)
                {
                    if (!certs.Contains(chainElement.Certificate))
                    {
                        certs.Add(chainElement.Certificate);
                    }
                }

                iCount++;
            }

            // Get context of all certs
            CertContexts = new CERT_CONTEXT[certs.Count];
            iCount = 0;
            foreach (X509Certificate2 cert in certs)
            {
                CertContexts[iCount] = (CERT_CONTEXT)Marshal.PtrToStructure(cert.Handle, typeof(CERT_CONTEXT));
                iCount++;
            }

            // Get cert blob of all certs
            CertBlobs = new BLOB[CertContexts.Length];
            for (iCount = 0; iCount < CertContexts.Length; iCount++)
            {
                CertBlobs[iCount].cbData = CertContexts[iCount].cbCertEncoded;
                CertBlobs[iCount].pbData = CertContexts[iCount].pbCertEncoded;
            }

            // Populate Enveloped Encode Info struct
            EnvelopedEncodeInfo.cbSize = Marshal.SizeOf(EnvelopedEncodeInfo);

            // Algorithm
            EnvelopedEncodeInfo.ContentEncryptionAlgorithm.pszObjId = algorithm;
            switch (algorithm)
            {
                // RC4
                case szOID_RSA_RC4:
                    // RC4 Aux Info
                    AuxInfo.cbSize = Marshal.SizeOf(AuxInfo);
                    AuxInfo.dwBitLen = CMSG_RC4_NO_SALT_FLAG;
                    EnvelopedEncodeInfo.pvEncryptionAuxInfo = Marshal.AllocHGlobal(Marshal.SizeOf(AuxInfo));
                    Marshal.StructureToPtr(AuxInfo, EnvelopedEncodeInfo.pvEncryptionAuxInfo, false);
                    break;

                // 3DES
                case szOID_RSA_DES_EDE3_CBC:
                    // IV generated automatically
                    EnvelopedEncodeInfo.ContentEncryptionAlgorithm.Parameters.cbData = 0;
                    EnvelopedEncodeInfo.pvEncryptionAuxInfo = IntPtr.Zero;
                    break;
                default:
                    break;
            }

            // Recipients
            EnvelopedEncodeInfo.cRecipients = RecipientContexts.Length;
            EnvelopedEncodeInfo.rgpRecipients = Marshal.AllocHGlobal(Marshal.SizeOf(RecipientContexts[0].pCertInfo) * RecipientContexts.Length);
            for (iCount = 0; iCount < RecipientContexts.Length; iCount++)
            {
                Marshal.WriteIntPtr(new IntPtr(EnvelopedEncodeInfo.rgpRecipients.ToInt64() + Marshal.SizeOf(RecipientContexts[iCount].pCertInfo) * iCount), RecipientContexts[iCount].pCertInfo);
            }

            // Certs
            EnvelopedEncodeInfo.cCertEncoded = CertBlobs.Length;
            EnvelopedEncodeInfo.rgCertEncoded = Marshal.AllocHGlobal(Marshal.SizeOf(CertBlobs[0]) * CertBlobs.Length);
            for (iCount = 0; iCount < CertBlobs.Length; iCount++)
            {
                Marshal.StructureToPtr(CertBlobs[iCount], new IntPtr(EnvelopedEncodeInfo.rgCertEncoded.ToInt64() + (Marshal.SizeOf(CertBlobs[iCount]) * iCount)), false);
            }

            return EnvelopedEncodeInfo;
        }

        public static CMSG_STREAM_INFO CreateStreamInfo(int dwFileSize, StreamOutputCallbackDelegate pfnCallback)
        {
            // Variables
            CMSG_STREAM_INFO StreamInfo = new CMSG_STREAM_INFO();

            // Populate Stream Info struct
            StreamInfo.cbContent = dwFileSize;
            StreamInfo.pfnStreamOutput = pfnCallback;

            // Return it
            return StreamInfo;
        }

        public static SafeMsgHandle OpenMessageToEncode(CMSG_SIGNED_ENCODE_INFO SignedEncodeInfo, CMSG_STREAM_INFO StreamInfo, bool bDetached)
        {
            // Variables
            SafeMsgHandle hMsg = SafeMsgHandle.Null;

            // Open message to encode
            hMsg = CryptMsgOpenToEncode(
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                CMSG_AUTHENTICATED_ATTRIBUTES_FLAG | (bDetached ? CMSG_DETACHED_FLAG : 0),
                CMSG_SIGNED,
                ref SignedEncodeInfo,
                null,
                ref StreamInfo
                );
            if ((hMsg == null) || (hMsg.IsInvalid))
            {
                throw new Exception("CryptMsgOpenToEncode error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }

            return hMsg;
        }

        public static SafeMsgHandle OpenMessageToEncode(CMSG_ENVELOPED_ENCODE_INFO EnvelopedEncodeInfo, CMSG_STREAM_INFO StreamInfo)
        {
            // Variables
            SafeMsgHandle hMsg = SafeMsgHandle.Null;

            // Open message to encode
            hMsg = CryptMsgOpenToEncode(
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                0,
                CMSG_ENVELOPED,
                ref EnvelopedEncodeInfo,
                null,
                ref StreamInfo
                );
            if ((hMsg == null) || (hMsg.IsInvalid))
            {
                throw new Exception("CryptMsgOpenToEncode error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }

            return hMsg;
        }

        public static SafeMsgHandle OpenMessageToDecode(CMSG_STREAM_INFO StreamInfo)
        {
            // Open message to decode
            return OpenMessageToDecode(StreamInfo, false);
        }

        public static SafeMsgHandle OpenMessageToDecode(CMSG_STREAM_INFO StreamInfo, bool bDetached)
        {
            // Variables
            SafeMsgHandle hMsg = SafeMsgHandle.Null;

            // Open message to decode
            hMsg = CryptMsgOpenToDecode(
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                bDetached ? CMSG_DETACHED_FLAG : 0,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                ref StreamInfo
                );
            if ((hMsg == null) || (hMsg.IsInvalid))
            {
                throw new Exception("CryptMsgOpenToDecode error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }

            return hMsg;
        }

        public unsafe static void ProcessMessage(SafeMsgHandle hMsg, FileStream file)
        {
            // Variables
            BinaryReader stream = null;
            byte[] pbData = null;
            long dwRemaining = 0;
            bool bFinal = false;
            bool bResult = false;

            // Process message
            dwRemaining = file.Length;
            stream = new BinaryReader(file);
            do
            {
                // Read one chunk of data
                pbData = stream.ReadBytes(1024 * 1000 * 100);
                if (pbData.Length == 0)
                {
                    break;
                }

                // Update message piece by piece    
                bFinal = (dwRemaining <= 1024 * 1000 * 100);
                fixed (byte* pAux = &pbData[0])
                {
                    bResult = CryptMsgUpdate(
                        hMsg.DangerousGetHandle(),
                        new IntPtr(pAux),
                        pbData.Length,
                        bFinal
                        );
                    if (!bResult)
                    {
                        throw new Exception("CryptMsgUpdate error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                    }
                }
                dwRemaining = dwRemaining - pbData.Length;

            } while (!bFinal);
        }

        public static void GetMessageParam(SafeMsgHandle hMsg, int dwParamType, out int iParam)
        {
            // Variables 
            int cbParam = 0;
            bool bResult = false;

            // Get integer param
            iParam = 0;
            cbParam = Marshal.SizeOf(iParam);
            bResult = CryptMsgGetParam(
                hMsg,
                dwParamType,
                0,
                out iParam,
                ref cbParam
                );
            if (!bResult)
            {
                throw new Exception("CryptMsgGetParam error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }

        public static void GetMessageParam(SafeMsgHandle hMsg, int dwParamType, out SafeNTHeapHandle pParam)
        {
            // Variables 
            int cbParam = 0;

            // Get param
            GetMessageParam(hMsg, dwParamType, 0, out pParam, out cbParam);
        }

        public static void GetMessageParam(SafeMsgHandle hMsg, int dwParamType, int dwIndex, out SafeNTHeapHandle pParam)
        {
            // Variables 
            int cbParam = 0;

            // Get param
            GetMessageParam(hMsg, dwParamType, dwIndex, out pParam, out cbParam);
        }

        public static void GetMessageParam(SafeMsgHandle hMsg, int dwParamType, int dwIndex, out SafeNTHeapHandle pParam, out int cbParam)
        {
            // Variables
            bool bResult = false;

            // Get size of data
            pParam = SafeNTHeapHandle.Null;
            cbParam = 0;
            bResult = CryptMsgGetParam(
                hMsg,
                dwParamType,
                dwIndex,
                pParam,
                ref cbParam
                );
            if (!bResult)
            {
                throw new Exception("CryptMsgGetParam error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }

            // Create buffer for data
            pParam = new SafeNTHeapHandle(Marshal.AllocHGlobal(cbParam));

            // Memory is not zero-filled. Do it, otherwise we will AV when marshalling some fields of structs
            MemSet(
                pParam.DangerousGetHandle(),
                0,
                cbParam
                );

            // Get data
            bResult = CryptMsgGetParam(
                hMsg,
                dwParamType,
                dwIndex,
                pParam,
                ref cbParam
                );
            if (!bResult)
            {
                throw new Exception("CryptMsgGetParam error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }

        public static void GetCertContextProperty(SafeCertContextHandle pCertContext, int dwPropId, out SafeNTHeapHandle pvData)
        {
            // Variables
            int cbData = 0;
            bool bResult = false;

            // Get size of data
            pvData = SafeNTHeapHandle.Null;
            bResult = CertGetCertificateContextProperty(
                pCertContext,
                dwPropId,
                pvData,
                ref cbData
                );
            if (!bResult)
            {
                throw new Exception("CertGetCertificateContextProperty error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }

            // Create buffer for data
            pvData = new SafeNTHeapHandle(Marshal.AllocHGlobal(cbData));

            // Get data
            bResult = CertGetCertificateContextProperty(
                pCertContext,
                dwPropId,
                pvData,
                ref cbData
                );
            if (!bResult)
            {
                throw new Exception("CertGetCertificateContextProperty error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }

        public static DateTime[] GetSigningTimes(SafeMsgHandle hMsg)
        {
            // Variables
            CMSG_SIGNER_INFO SignerInfo = new CMSG_SIGNER_INFO();
            CRYPT_ATTRIBUTE CryptAttr = new CRYPT_ATTRIBUTE();
            BLOB CryptAttrBlob = new BLOB();
            DateTime[] signingTimes = null;
            SafeNTHeapHandle pSignerInfo = SafeNTHeapHandle.Null;
            IntPtr pCryptAttr = IntPtr.Zero;
            long lFileTime = 0;
            int cbFileTime = 0;
            int iCount = 0;
            bool bResult = false;

            try
            {
                // Get number of signers
                iCount = GetSignerCount(hMsg);

                // Create array of signing times 
                signingTimes = new DateTime[iCount];

                // Get all signing times
                for (int i = 0; i < iCount; i++)
                {
                    // Get signer info
                    GetMessageParam(hMsg, CMSG_SIGNER_INFO_PARAM, i, out pSignerInfo);

                    // Get signing time from authenticated attributes
                    SignerInfo = (CMSG_SIGNER_INFO)Marshal.PtrToStructure(pSignerInfo.DangerousGetHandle(), typeof(CMSG_SIGNER_INFO));
                    pCryptAttr = SignerInfo.AuthAttrs.rgAttr;
                    for (int j = 0; j < SignerInfo.AuthAttrs.cAttr; j++)
                    {
                        CryptAttr = (CRYPT_ATTRIBUTE)Marshal.PtrToStructure(pCryptAttr, typeof(CRYPT_ATTRIBUTE));
                        if (CryptAttr.pszObjId.Equals(szOID_RSA_signingTime))
                        {
                            CryptAttrBlob = (BLOB)Marshal.PtrToStructure(CryptAttr.rgValue, typeof(BLOB));

                            cbFileTime = Marshal.SizeOf(lFileTime);
                            bResult = CryptDecodeObject(
                                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                                szOID_RSA_signingTime,
                                CryptAttrBlob.pbData,
                                CryptAttrBlob.cbData,
                                0,
                                out lFileTime,
                                ref cbFileTime
                                );
                            if (!bResult)
                            {
                                throw new Exception("CryptDecodeObject error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                            }

                            // Found it. Add it to array
                            signingTimes[i] = DateTime.FromFileTime(lFileTime);

                            break;
                        }
                        pCryptAttr = new IntPtr(pCryptAttr.ToInt32() + Marshal.SizeOf(CryptAttr));
                    }

                    // Clean up
                    pSignerInfo.Dispose();
                }
            }
            finally
            {
                pSignerInfo.Dispose();
            }

            return signingTimes;
        }

        public static X509Certificate2Collection GetCerts(SafeMsgHandle hMsg)
        {
            // Variables
            X509Certificate2Collection certs = null;
            X509Certificate2 cert = null;
            SafeNTHeapHandle pbCert = SafeNTHeapHandle.Null;
            SafeCertContextHandle pCertContext = SafeCertContextHandle.Null;
            int iCount = 0;
            int cbCert = 0;

            try
            {
                // Get number of certificates
                GetMessageParam(hMsg, CMSG_CERT_COUNT_PARAM, out iCount);

                // Get certificates
                certs = new X509Certificate2Collection();
                for (int i = 0; i < iCount; i++)
                {
                    // Get binary data for cert
                    pbCert = SafeNTHeapHandle.Null;
                    GetMessageParam(hMsg, CMSG_CERT_PARAM, i, out pbCert, out cbCert);

                    // Create a cert context with that data
                    pCertContext = SafeCertContextHandle.Null;
                    pCertContext = CertCreateCertificateContext(
                        X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                        pbCert,
                        cbCert
                        );
                    if ((pCertContext == null) || (pCertContext.IsInvalid))
                    {
                        throw new Exception("CertCreateCertificateContext error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                    }

                    // Create cert and add to collection
                    cert = new X509Certificate2(pCertContext.DangerousGetHandle());
                    certs.Add(cert);

                    // Clean up
                    pCertContext.Dispose();
                    pbCert.Dispose();
                }
            }
            finally
            {
                // Clean up
                pCertContext.Dispose();
                pbCert.Dispose();
            }

            return certs;
        }

        public static X509Certificate2Collection GetSignerCerts(SafeMsgHandle hMsg)
        {
            // Variables
            CMSG_SIGNER_INFO SignerInfo = new CMSG_SIGNER_INFO();
            X500DistinguishedName issuer = null;
            X509Certificate2Collection allCerts = null;
            X509Certificate2Collection certsByIssuer = null;
            X509Certificate2Collection certsBySerial = null;
            X509Certificate2Collection signerCerts = null;
            X509Certificate2 cert = null;
            SafeNTHeapHandle pSignerInfo = SafeNTHeapHandle.Null;
            string strSerial = null;
            byte[] pbIssuer = null;
            byte[] pbSerial = null;
            int iCount = 0;

            try
            {
                // Get number of signers
                iCount = GetSignerCount(hMsg);

                // Create cert collection 
                signerCerts = new X509Certificate2Collection();

                // Get all signers
                for (int i = 0; i < iCount; i++)
                {
                    // Get signer info
                    GetMessageParam(hMsg, CMSG_SIGNER_INFO_PARAM, i, out pSignerInfo);
                    SignerInfo = (CMSG_SIGNER_INFO)Marshal.PtrToStructure(pSignerInfo.DangerousGetHandle(), typeof(CMSG_SIGNER_INFO));

                    // Get signer's Issuer
                    pbIssuer = new byte[SignerInfo.Issuer.cbData];
                    Marshal.Copy(SignerInfo.Issuer.pbData, pbIssuer, 0, SignerInfo.Issuer.cbData);
                    issuer = new X500DistinguishedName(pbIssuer);

                    // Get signer's Serial Number in Hexadecimal
                    pbSerial = new byte[SignerInfo.SerialNumber.cbData];
                    Marshal.Copy(SignerInfo.SerialNumber.pbData, pbSerial, 0, SignerInfo.SerialNumber.cbData);
                    Array.Reverse(pbSerial);
                    strSerial = BitConverter.ToString(pbSerial);
                    strSerial = strSerial.Replace("-", "");

                    // Get all certs
                    allCerts = GetCerts(hMsg);

                    // Get certs with signer's Issuer
                    certsByIssuer = allCerts.Find(X509FindType.FindByIssuerDistinguishedName, issuer.Name, false);

                    // Get certs with signer's Serial Number
                    certsBySerial = certsByIssuer.Find(X509FindType.FindBySerialNumber, strSerial, false);

                    // There should be only one. Get it
                    cert = certsBySerial[0];

                    // Add the signer cert we found to the collection
                    signerCerts.Add(cert);

                    // Clean up 
                    pSignerInfo.Dispose();
                }
            }
            finally
            {
                // Clean up
                pSignerInfo.Dispose();
            }

            // Return the cert collection
            return signerCerts;
        }

        public static X509Certificate2Collection GetRecipientCerts(SafeMsgHandle hMsg)
        {
            // Variables
            CERT_INFO CertInfo = new CERT_INFO();
            X500DistinguishedName issuer = null;
            X509Certificate2Collection allCerts = null;
            X509Certificate2Collection certsByIssuer = null;
            X509Certificate2Collection certsBySerial = null;
            X509Certificate2Collection recipients = null;
            X509Certificate2 cert = null;
            SafeNTHeapHandle pCertInfo = SafeNTHeapHandle.Null;
            string strSerial = null;
            byte[] pbIssuer = null;
            byte[] pbSerial = null;
            int iCount = 0;

            try
            {
                // Get number of recipients
                iCount = GetRecipientCount(hMsg);

                // Create cert collection 
                recipients = new X509Certificate2Collection();

                // Get all recipients
                for (int i = 0; i < iCount; i++)
                {
                    // Get recipient info
                    GetMessageParam(hMsg, CMSG_RECIPIENT_INFO_PARAM, i, out pCertInfo);
                    CertInfo = (CERT_INFO)Marshal.PtrToStructure(pCertInfo.DangerousGetHandle(), typeof(CERT_INFO));

                    // Get recipient's Issuer
                    pbIssuer = new byte[CertInfo.Issuer.cbData];
                    Marshal.Copy(CertInfo.Issuer.pbData, pbIssuer, 0, CertInfo.Issuer.cbData);
                    issuer = new X500DistinguishedName(pbIssuer);

                    // Get recipient's Serial Number in Hexadecimal
                    pbSerial = new byte[CertInfo.SerialNumber.cbData];
                    Marshal.Copy(CertInfo.SerialNumber.pbData, pbSerial, 0, CertInfo.SerialNumber.cbData);
                    Array.Reverse(pbSerial);
                    strSerial = BitConverter.ToString(pbSerial);
                    strSerial = strSerial.Replace("-", "");

                    // Get all certs
                    allCerts = GetCerts(hMsg);

                    // Get certs with recipient's Issuer
                    certsByIssuer = allCerts.Find(X509FindType.FindByIssuerDistinguishedName, issuer.Name, false);

                    // Get certs with recipient's Serial Number
                    certsBySerial = certsByIssuer.Find(X509FindType.FindBySerialNumber, strSerial, false);

                    // There should be only one. Get it
                    cert = certsBySerial[0];

                    // Add the recipient cert we found to the collection
                    recipients.Add(cert);

                    // Clean up 
                    pCertInfo.Dispose();
                }
            }
            finally
            {
                // Clean up
                pCertInfo.Dispose();
            }

            // Return the cert collection
            return recipients;
        }

        public static void CheckEnvelopeAlg(SafeMsgHandle hMsg)
        {
            // Variables
            CRYPT_ALGORITHM_IDENTIFIER AlgId = new CRYPT_ALGORITHM_IDENTIFIER();
            SafeNTHeapHandle pEnvelopeAlg = SafeNTHeapHandle.Null;

            try
            {
                // Get envelope encryption algorithm
                GetMessageParam(hMsg, CMSG_ENVELOPE_ALGORITHM_PARAM, out pEnvelopeAlg);
                AlgId = (CRYPT_ALGORITHM_IDENTIFIER)Marshal.PtrToStructure(pEnvelopeAlg.DangerousGetHandle(), typeof(CRYPT_ALGORITHM_IDENTIFIER));
            }
            finally
            {
                // Clean up
                pEnvelopeAlg.Dispose();
            }
        }

        public static int GetSignerCount(SafeMsgHandle hMsg)
        {
            // Variables
            int iCount = 0;

            // Get signer count
            GetMessageParam(hMsg, CMSG_SIGNER_COUNT_PARAM, out iCount);

            // Return the count
            return iCount;
        }

        public static int GetRecipientCount(SafeMsgHandle hMsg)
        {
            // Variables
            int iCount = 0;

            // Get signer count
            GetMessageParam(hMsg, CMSG_RECIPIENT_COUNT_PARAM, out iCount);

            // Return the count
            return iCount;
        }

        public static void AddSigner(SafeMsgHandle hMsg, CMSG_SIGNER_ENCODE_INFO signerEncodeInfo)
        {
            // Variables
            bool bResult = false;

            // Add signer to message
            bResult = CryptMsgControl(
                hMsg,
                0,
                CMSG_CTRL_ADD_SIGNER,
                ref signerEncodeInfo
                );
            if (!bResult)
            {
                throw new Exception("CryptMsgControl error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }

        public static void AddCerts(SafeMsgHandle hMsg, X509Certificate2Collection newCerts)
        {
            // Variables
            CERT_CONTEXT CertContext = new CERT_CONTEXT();
            BLOB CertBlob = new BLOB();
            X509Certificate2Collection previousCerts = null;
            bool bResult = false;

            // Get all certs already in the message
            previousCerts = GetCerts(hMsg);

            // Check if new certs are already in the message
            foreach (X509Certificate2 cert in newCerts)
            {
                if (previousCerts.Find(X509FindType.FindByThumbprint, cert.Thumbprint, false).Count == 0)
                {
                    // Get context of the cert
                    CertContext = (CERT_CONTEXT)Marshal.PtrToStructure(cert.Handle, typeof(CERT_CONTEXT));

                    // Get cert blob
                    CertBlob.cbData = CertContext.cbCertEncoded;
                    CertBlob.pbData = CertContext.pbCertEncoded;

                    // Add cert to message
                    bResult = CryptMsgControl(
                        hMsg,
                        0,
                        CMSG_CTRL_ADD_CERT,
                        ref CertBlob
                        );
                    if (!bResult)
                    {
                        throw new Exception("CryptMsgControl error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                    }
                }
            }
        }

        public static void UpdateMessage(SafeMsgHandle hMsg, FileStream reencodedFile)
        {
            // Variables
            SafeNTHeapHandle pMessage = SafeNTHeapHandle.Null;
            byte[] pbMessage = null;
            int cbMessage = 0;

            try
            {
                // Update encoded message
                GetMessageParam(hMsg, CMSG_ENCODED_MESSAGE, 0, out pMessage, out cbMessage);

                // Get message
                pbMessage = new byte[cbMessage];
                Marshal.Copy(pMessage.DangerousGetHandle(), pbMessage, 0, cbMessage);

                // Save it to file
                reencodedFile.Write(pbMessage, 0, pbMessage.Length);
            }
            finally
            {
                pMessage.Dispose();
            }
        }

        public static void CheckSignature(SafeMsgHandle hMsg, int dwIndex)
        {
            // Variables
            CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA VerifyParam = new CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA();
            SafeNTHeapHandle pSignerCertInfo = SafeNTHeapHandle.Null;
            SafeStoreHandle hStore = SafeStoreHandle.Null;
            SafeCertContextHandle pSignerCertContext = SafeCertContextHandle.Null;
            bool bResult = false;

            try
            {
                // Get signer cert info
                GetMessageParam(hMsg, CMSG_SIGNER_CERT_INFO_PARAM, dwIndex, out pSignerCertInfo);

                // Open a cert store in memory with the certs from the message
                hStore = CertOpenStore(
                    CERT_STORE_PROV_MSG,
                    X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                    IntPtr.Zero,
                    0,
                    hMsg
                    );
                if ((hStore == null) || (hStore.IsInvalid))
                {
                    throw new Exception("CertOpenStore error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Find the signer's cert context in the store
                pSignerCertContext = CertGetSubjectCertificateFromStore(
                    hStore,
                    X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                    pSignerCertInfo
                    );
                if ((pSignerCertContext == null) || (pSignerCertContext.IsInvalid))
                {
                    throw new Exception("CertGetSubjectCertificateFromStore error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                VerifyParam.cbSize = Marshal.SizeOf(VerifyParam);
                VerifyParam.hCryptProv = IntPtr.Zero;
                VerifyParam.dwSignerIndex = dwIndex;
                VerifyParam.dwSignerType = CMSG_VERIFY_SIGNER_CERT;
                VerifyParam.pvSigner = pSignerCertContext.DangerousGetHandle();

                // Verify signature in message
                bResult = CryptMsgControl(
                    hMsg,
                    0,
                    CMSG_CTRL_VERIFY_SIGNATURE_EX,
                    ref VerifyParam
                    );
                if (!bResult)
                {
                    throw new Exception("CryptMsgControl error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }
            finally
            {
                // Clean up
                pSignerCertContext.Dispose();
                hStore.Dispose();
                pSignerCertInfo.Dispose();
            }
        }

        public static void CheckSignatures(SafeMsgHandle hMsg)
        {
            // Variables
            int iCount = 0;

            // Get number of signers
            iCount = GetSignerCount(hMsg);

            // Verify all signatures
            for (int i = 0; i < iCount; i++)
            {
                // Verify the signature of one signer
                CheckSignature(hMsg, i);
            }
        }

        public static void Decrypt(SafeMsgHandle hMsg)
        {
            // Variables
            CRYPT_KEY_PROV_INFO KeyProvInfo = new CRYPT_KEY_PROV_INFO();
            CMSG_CTRL_DECRYPT_PARA DecryptPara = new CMSG_CTRL_DECRYPT_PARA();
            SafeNTHeapHandle pCertInfo = SafeNTHeapHandle.Null;
            SafeStoreHandle hStore = SafeStoreHandle.Null;
            SafeCertContextHandle pCertContext = SafeCertContextHandle.Null;
            SafeNTHeapHandle pKeyProvInfo = SafeNTHeapHandle.Null;
            SafeCSPHandle hProv = SafeCSPHandle.Null;
            bool bResult = false;

            try
            {
                // Get recipient cert
                GetMessageParam(hMsg, CMSG_RECIPIENT_INFO_PARAM, out pCertInfo);

                // Open personal cert store
                hStore = CertOpenSystemStore(
                    IntPtr.Zero,
                    "MY"
                    );
                if ((hStore == null) || (hStore.IsInvalid))
                {
                    throw new Exception("CertOpenStore error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Find recipient cert in store
                pCertContext = CertGetSubjectCertificateFromStore(
                    hStore,
                    X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                    pCertInfo
                    );
                if ((pCertContext == null) || (pCertContext.IsInvalid))
                {
                    throw new Exception("CertGetSubjectCertificateFromStore error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Get key info for that cert
                GetCertContextProperty(pCertContext, CERT_KEY_PROV_INFO_PROP_ID, out pKeyProvInfo);
                KeyProvInfo = (CRYPT_KEY_PROV_INFO)Marshal.PtrToStructure(pKeyProvInfo.DangerousGetHandle(), typeof(CRYPT_KEY_PROV_INFO));

                // Get CSP for that key
                bResult = CryptAcquireContext(
                    out hProv,
                    KeyProvInfo.pwszContainerName,
                    KeyProvInfo.pwszProvName,
                    KeyProvInfo.dwProvType,
                    0
                    );
                if (!bResult)
                {
                    throw new Exception("CryptAcquireContext error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }

                // Prepare decryption
                DecryptPara.cbSize = Marshal.SizeOf(DecryptPara);
                DecryptPara.hCryptProvOrNCryptKey = hProv.DangerousGetHandle();
                DecryptPara.dwKeySpec = KeyProvInfo.dwKeySpec;
                DecryptPara.dwRecipientIndex = 0;

                // Decrypt the message
                bResult = CryptMsgControl(
                    hMsg,
                    0,
                    CMSG_CTRL_DECRYPT,
                    ref DecryptPara
                    );
                if (!bResult)
                {
                    throw new Exception("CryptMsgControl error #" + Marshal.GetLastWin32Error().ToString(), new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }
            finally
            {
                // Clean up
                pCertContext.Dispose();
                hStore.Dispose();
                pCertInfo.Dispose();
                //hProv.Dispose(); // This is not needed. We get an AV when disposing the message handle if we do this now.
                pKeyProvInfo.Dispose();
            }
        }

        #endregion
    }

    partial class Win32
    {
        #region "CONSTS"

        //#define X509_ASN_ENCODING           0x00000001
        internal const int X509_ASN_ENCODING = 0x00000001;

        //#define PKCS_7_ASN_ENCODING         0x00010000
        internal const int PKCS_7_ASN_ENCODING = 0x00010000;

        //#define CMSG_SIGNED                  2
        internal const int CMSG_SIGNED = 2;

        //#define CMSG_ENVELOPED               3
        internal const int CMSG_ENVELOPED = 3;

        //#define CMSG_DETACHED_FLAG                  0x00000004
        internal const int CMSG_DETACHED_FLAG = 0x00000004;

        //#define CMSG_AUTHENTICATED_ATTRIBUTES_FLAG  0x00000008
        internal const int CMSG_AUTHENTICATED_ATTRIBUTES_FLAG = 0x00000008;

        //#define CMSG_ENVELOPE_ALGORITHM_PARAM                15
        internal const int CMSG_ENVELOPE_ALGORITHM_PARAM = 15;

        //#define AT_KEYEXCHANGE		1
        internal const int AT_KEYEXCHANGE = 1;

        //#define AT_SIGNATURE		2
        internal const int AT_SIGNATURE = 2;

        //#define szOID_OIWSEC_rsaSign    "1.3.14.3.2.11"
        internal const String szOID_OIWSEC_rsaSign = "1.3.14.3.2.11";

        //#define szOID_OIWSEC_shaRSA     "1.3.14.3.2.15"
        internal const String szOID_OIWSEC_shaRSA = "1.3.14.3.2.15";

        //#define szOID_OIWSEC_sha        "1.3.14.3.2.18"
        internal const String szOID_OIWSEC_sha = "1.3.14.3.2.18";

        //#define szOID_OIWSEC_sha1       "1.3.14.3.2.26"
        internal const String szOID_OIWSEC_sha1 = "1.3.14.3.2.26";

        //#define szOID_OIWSEC_sha1RSASign "1.3.14.3.2.29"
        internal const String szOID_OIWSEC_sha1RSASign = "1.3.14.3.2.29";

        //#define szOID_RSA_RC4           "1.2.840.113549.3.4"
        internal const String szOID_RSA_RC4 = "1.2.840.113549.3.4";

        //#define szOID_RSA_DES_EDE3_CBC  "1.2.840.113549.3.7"
        internal const String szOID_RSA_DES_EDE3_CBC = "1.2.840.113549.3.7";

        //#define szOID_RSA_signingTime   "1.2.840.113549.1.9.5"
        internal const String szOID_RSA_signingTime = "1.2.840.113549.1.9.5";

        //#define X509_NAME                           ((LPCSTR) 7)
        internal const int X509_NAME = 7;

        //#define CMSG_ENCODED_SIGNER                          28
        internal const int CMSG_ENCODED_SIGNER = 8;

        //#define CMSG_CTRL_VERIFY_SIGNATURE       1
        internal const int CMSG_CTRL_VERIFY_SIGNATURE = 1;

        //#define CMSG_CTRL_DECRYPT                2
        internal const int CMSG_CTRL_DECRYPT = 2;

        //#define CMSG_CTRL_VERIFY_SIGNATURE_EX    19
        internal const int CMSG_CTRL_VERIFY_SIGNATURE_EX = 19;

        //#define CMSG_CTRL_ADD_SIGNER             6
        internal const int CMSG_CTRL_ADD_SIGNER = 6;

        //#define CMSG_CTRL_ADD_CERT               10
        internal const int CMSG_CTRL_ADD_CERT = 10;

        //#define CMSG_SIGNER_COUNT_PARAM                      5
        internal const int CMSG_SIGNER_COUNT_PARAM = 5;

        //#define CMSG_SIGNER_INFO_PARAM                       6
        internal const int CMSG_SIGNER_INFO_PARAM = 6;

        //#define CMSG_SIGNER_CERT_INFO_PARAM                  7
        internal const int CMSG_SIGNER_CERT_INFO_PARAM = 7;

        //#define CMSG_CERT_COUNT_PARAM                        11
        internal const int CMSG_CERT_COUNT_PARAM = 11;

        //#define CMSG_CERT_PARAM                              12
        internal const int CMSG_CERT_PARAM = 12;

        //#define CMSG_RECIPIENT_COUNT_PARAM                   17
        internal const int CMSG_RECIPIENT_COUNT_PARAM = 17;

        //#define CMSG_RECIPIENT_INFO_PARAM                    19
        internal const int CMSG_RECIPIENT_INFO_PARAM = 19;

        //#define CMSG_ENCODED_MESSAGE                         29
        internal const int CMSG_ENCODED_MESSAGE = 29;

        //#define CERT_STORE_PROV_MSG                 ((LPCSTR) 1)
        internal const int CERT_STORE_PROV_MSG = 1;

        //#define CERT_CLOSE_STORE_FORCE_FLAG         0x00000001
        internal const int CERT_CLOSE_STORE_FORCE_FLAG = 1;

        //#define CERT_KEY_PROV_INFO_PROP_ID          2
        internal const int CERT_KEY_PROV_INFO_PROP_ID = 2;

        //#define CMSG_VERIFY_SIGNER_CERT                     2
        internal const int CMSG_VERIFY_SIGNER_CERT = 2;

        //#define CMSG_RC4_NO_SALT_FLAG               0x40000000
        internal const int CMSG_RC4_NO_SALT_FLAG = 0x40000000;

        //#define CERT_STORE_PROV_SYSTEM_W            ((LPCSTR) 10)
        //#define CERT_STORE_PROV_SYSTEM              CERT_STORE_PROV_SYSTEM_W
        internal const int CERT_STORE_PROV_SYSTEM = 10;

        //#define CERT_SYSTEM_STORE_CURRENT_USER_ID       1
        //#define CERT_SYSTEM_STORE_LOCATION_SHIFT        16
        //#define CERT_SYSTEM_STORE_CURRENT_USER          \
        //  (CERT_SYSTEM_STORE_CURRENT_USER_ID << CERT_SYSTEM_STORE_LOCATION_SHIFT)
        internal const int CERT_SYSTEM_STORE_CURRENT_USER = 1 << 16;
        #endregion

        #region "STRUCTS"

        //typedef struct _CRYPT_ALGORITHM_IDENTIFIER {
        //  LPSTR            pszObjId;
        //  CRYPT_OBJID_BLOB Parameters;
        //} CRYPT_ALGORITHM_IDENTIFIER, *PCRYPT_ALGORITHM_IDENTIFIER;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_ALGORITHM_IDENTIFIER
        {
            public string pszObjId;
            public BLOB Parameters;

            public void Dispose()
            {
                Parameters.Dispose();
            }
        }

        //typedef struct _CERT_ID {
        //  DWORD dwIdChoice;
        //  union {
        //    CERT_ISSUER_SERIAL_NUMBER IssuerSerialNumber;
        //    CRYPT_HASH_BLOB KeyId;
        //    CRYPT_HASH_BLOB HashId;
        //  } ;
        //} CERT_ID, *PCERT_ID;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_ID
        {
            public int dwIdChoice;
            public BLOB IssuerSerialNumberOrKeyIdOrHashId;
        }

        //typedef struct _CRYPT_ATTRIBUTE {
        //  LPSTR            pszObjId;
        //  DWORD            cValue;
        //  PCRYPT_ATTR_BLOB rgValue;
        //} CRYPT_ATTRIBUTE, *PCRYPT_ATTRIBUTE;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_ATTRIBUTE
        {
            public string pszObjId;
            public int cValue;
            public IntPtr rgValue;

            public void Dispose()
            {
                if (!rgValue.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgValue); }
            }
        }

        //typedef struct _CMSG_SIGNER_ENCODE_INFO {
        //  DWORD                      cbSize;
        //  PCERT_INFO                 pCertInfo;
        //  union {
        //    HCRYPTPROV hCryptProv;
        //    NCRYPT_KEY_HANDLE hNCryptKey;
        //  } ;
        //  DWORD                      dwKeySpec;
        //  CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
        //  void *                     pvHashAuxInfo;
        //  DWORD                      cAuthAttr;
        //  PCRYPT_ATTRIBUTE           rgAuthAttr;
        //  DWORD                      cUnauthAttr;
        //  PCRYPT_ATTRIBUTE           rgUnauthAttr;
        //  CERT_ID                    SignerId;
        //  CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
        //  void *                     pvHashEncryptionAuxInfo;
        //} CMSG_SIGNER_ENCODE_INFO, *PCMSG_SIGNER_ENCODE_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_SIGNER_ENCODE_INFO
        {
            public int cbSize;
            public IntPtr pCertInfo;
            public IntPtr hCryptProvOrhNCryptKey;
            public int dwKeySpec;
            public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
            public IntPtr pvHashAuxInfo;
            public int cAuthAttr;
            public IntPtr rgAuthAttr;
            public int cUnauthAttr;
            public IntPtr rgUnauthAttr;
            public CERT_ID SignerId;
            public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
            public IntPtr pvHashEncryptionAuxInfo;

            public void Dispose()
            {
                if (!hCryptProvOrhNCryptKey.Equals(IntPtr.Zero)) { Win32.CryptReleaseContext(hCryptProvOrhNCryptKey, 0); }
                if (!rgAuthAttr.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgAuthAttr); }
                if (!rgUnauthAttr.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgUnauthAttr); }
            }
        }

        //typedef struct _CERT_CONTEXT {
        //  DWORD      dwCertEncodingType;
        //  BYTE *     pbCertEncoded;
        //  DWORD      cbCertEncoded;
        //  PCERT_INFO pCertInfo;
        //  HCERTSTORE hCertStore;
        //} CERT_CONTEXT, *PCERT_CONTEXT;
        //typedef const CERT_CONTEXT *PCCERT_CONTEXT;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_CONTEXT
        {
            public int dwCertEncodingType;
            public IntPtr pbCertEncoded;
            public int cbCertEncoded;
            public IntPtr pCertInfo;
            public IntPtr hCertStore;
        }

        //typedef struct _CRYPTOAPI_BLOB {
        //  DWORD cbData;
        //  BYTE *pbData;
        //} CRYPT_intEGER_BLOB, *PCRYPT_intEGER_BLOB, CRYPT_Uint_BLOB, *PCRYPT_Uint_BLOB, CRYPT_OBJID_BLOB, *PCRYPT_OBJID_BLOB, CERT_NAME_BLOB, CERT_RDN_VALUE_BLOB, *PCERT_NAME_BLOB, *PCERT_RDN_VALUE_BLOB, CERT_BLOB, *PCERT_BLOB, CRL_BLOB, *PCRL_BLOB, DATA_BLOB, *PDATA_BLOB, CRYPT_DATA_BLOB, *PCRYPT_DATA_BLOB, CRYPT_HASH_BLOB, *PCRYPT_HASH_BLOB, CRYPT_DIGEST_BLOB, *PCRYPT_DIGEST_BLOB, CRYPT_DER_BLOB, PCRYPT_DER_BLOB, CRYPT_ATTR_BLOB, *PCRYPT_ATTR_BLOB;
        [StructLayout(LayoutKind.Sequential)]
        internal struct BLOB
        {
            public int cbData;
            public IntPtr pbData;

            public void Dispose()
            {
                if (!pbData.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(pbData); }
            }
        }

        //typedef struct _CRYPT_BIT_BLOB {
        //  DWORD cbData;
        //  BYTE  *pbData;
        //  DWORD cUnusedBits;
        //} CRYPT_BIT_BLOB, *PCRYPT_BIT_BLOB;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_BIT_BLOB
        {
            public int cbData;
            public IntPtr pbData;
            public int cUnusedBits;
        }

        //struct CMSG_SIGNED_ENCODE_INFO
        //{
        //    DWORD cbSize;
        //    DWORD cSigners;
        //    PCMSG_SIGNER_ENCODE_INFO rgSigners;
        //    DWORD cCertEncoded;
        //    CERT_BLOB rgCertEncoded;
        //    DWORD cCrlEncoded;
        //    PCRL_BLOB rgCrlEncoded;
        //    DWORD cAttrCertEncoded;
        //    PCERT_BLOB rgAttrCertEncoded;
        //};
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_SIGNED_ENCODE_INFO
        {
            public int cbSize;
            public int cSigners;
            public IntPtr rgSigners;
            public int cCertEncoded;
            public IntPtr rgCertEncoded;
            public int cCrlEncoded;
            public IntPtr rgCrlEncoded;
            public int cAttrCertEncoded;
            public IntPtr rgAttrCertEncoded;

            public void Dispose()
            {
                if (!rgSigners.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgSigners); }
                if (!rgCertEncoded.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgCertEncoded); }
                if (!rgCrlEncoded.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgCrlEncoded); }
                if (!rgAttrCertEncoded.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgAttrCertEncoded); }
            }
        }

        //typedef struct _CRYPT_ATTRIBUTES {
        //  DWORD            cAttr;
        //  PCRYPT_ATTRIBUTE rgAttr;
        //} CRYPT_ATTRIBUTES, *PCRYPT_ATTRIBUTES, CMSG_ATTR;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_ATTRIBUTES
        {
            public int cAttr;
            public IntPtr rgAttr;
        }

        //typedef struct _CMSG_SIGNER_INFO {
        //  DWORD                      dwVersion;
        //  CERT_NAME_BLOB             Issuer;
        //  CRYPT_INTEGER_BLOB         SerialNumber;
        //  CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
        //  CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
        //  CRYPT_DATA_BLOB            EncryptedHash;
        //  CRYPT_ATTRIBUTES           AuthAttrs;
        //  CRYPT_ATTRIBUTES           UnauthAttrs;
        //} CMSG_SIGNER_INFO, *PCMSG_SIGNER_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_SIGNER_INFO
        {
            public int dwVersion;
            public BLOB Issuer;
            public BLOB SerialNumber;
            public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
            public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
            public BLOB EncryptedHash;
            public CRYPT_ATTRIBUTES AuthAttrs;
            public CRYPT_ATTRIBUTES UnauthAttrs;
        }

        //typedef struct _CMSG_STREAM_INFO {
        //  DWORD                  cbContent;
        //  PFN_CMSG_STREAM_OUTPUT pfnStreamOutput;
        //  void *                 pvArg;
        //} CMSG_STREAM_INFO, *PCMSG_STREAM_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_STREAM_INFO
        {
            public int cbContent;
            public StreamOutputCallbackDelegate pfnStreamOutput;
            public IntPtr pvArg;
        }

        //typedef struct _CERT_PUBLIC_KEY_INFO {
        //  CRYPT_ALGORITHM_IDENTIFIER Algorithm;
        //  CRYPT_BIT_BLOB             PublicKey;
        //} CERT_PUBLIC_KEY_INFO, *PCERT_PUBLIC_KEY_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_PUBLIC_KEY_INFO
        {
            public CRYPT_ALGORITHM_IDENTIFIER Algorithm;
            public CRYPT_BIT_BLOB PublicKey;
        }

        //typedef struct _FILETIME {
        //  DWORD dwLowDateTime;
        //  DWORD dwHighDateTime;
        //} FILETIME, *PFILETIME;
        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME
        {
            public int dwLowDateTime;
            public int dwHighDateTime;
        }

        //typedef struct _CERT_INFO {
        //  DWORD                      dwVersion;
        //  CRYPT_INTEGER_BLOB         SerialNumber;
        //  CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
        //  CERT_NAME_BLOB             Issuer;
        //  FILETIME                   NotBefore;
        //  FILETIME                   NotAfter;
        //  CERT_NAME_BLOB             Subject;
        //  CERT_PUBLIC_KEY_INFO       SubjectPublicKeyInfo;
        //  CRYPT_BIT_BLOB             IssuerUniqueId;
        //  CRYPT_BIT_BLOB             SubjectUniqueId;
        //  DWORD                      cExtension;
        //  PCERT_EXTENSION            rgExtension;
        //} CERT_INFO, *PCERT_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_INFO
        {
            public int dwVersion;
            public BLOB SerialNumber;
            public CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
            public BLOB Issuer;
            public FILETIME NotBefore;
            public FILETIME NotAfter;
            public BLOB Subject;
            public CERT_PUBLIC_KEY_INFO SubjectPublicKeyInfo;
            public CRYPT_BIT_BLOB IssuerUniqueId;
            public CRYPT_BIT_BLOB SubjectUniqueId;
            public int cExtension;
            public IntPtr rgExtension;
        }

        //typedef struct _CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA {
        //  DWORD             cbSize;
        //  HCRYPTPROV_LEGACY hCryptProv;
        //  DWORD             dwSignerIndex;
        //  DWORD             dwSignerType;
        //  void              *pvSigner;
        //} CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA, *PCMSG_CTRL_VERIFY_SIGNATURE_EX_PARA;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA
        {
            public int cbSize;
            public IntPtr hCryptProv;
            public int dwSignerIndex;
            public int dwSignerType;
            public IntPtr pvSigner;
        }

        //typedef struct _CMSG_ENVELOPED_ENCODE_INFO {
        //  DWORD                       cbSize;
        //  HCRYPTPROV_LEGACY           hCryptProv;
        //  CRYPT_ALGORITHM_IDENTIFIER  ContentEncryptionAlgorithm;
        //  void                        *pvEncryptionAuxInfo;
        //  DWORD                       cRecipients;
        //  PCERT_INFO                  *rgpRecipients;
        //  PCMSG_RECIPIENT_ENCODE_INFO rgCmsRecipients;
        //  DWORD                       cCertEncoded;
        //  PCERT_BLOB                  rgCertEncoded;
        //  DWORD                       cCrlEncoded;
        //  PCRL_BLOB                   rgCrlEncoded;
        //  DWORD                       cAttrCertEncoded;
        //  PCERT_BLOB                  rgAttrCertEncoded;
        //  DWORD                       cUnprotectedAttr;
        //  PCRYPT_ATTRIBUTE            rgUnprotectedAttr;
        //} CMSG_ENVELOPED_ENCODE_INFO, *PCMSG_ENVELOPED_ENCODE_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_ENVELOPED_ENCODE_INFO
        {
            public int cbSize;
            public IntPtr hCryptProv;
            public CRYPT_ALGORITHM_IDENTIFIER ContentEncryptionAlgorithm;
            public IntPtr pvEncryptionAuxInfo;
            public int cRecipients;
            public IntPtr rgpRecipients;
            public IntPtr rgCmsRecipients;
            public int cCertEncoded;
            public IntPtr rgCertEncoded;
            public int cCrlEncoded;
            public IntPtr rgCrlEncoded;
            public int cAttrCertEncoded;
            public IntPtr rgAttrCertEncoded;
            public int cUnprotectedAttr;
            public IntPtr rgUnprotectedAttr;

            public void Dispose()
            {
                ContentEncryptionAlgorithm.Dispose();
                if (!pvEncryptionAuxInfo.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(pvEncryptionAuxInfo); }
                if (!rgpRecipients.Equals(IntPtr.Zero)) { Marshal.FreeHGlobal(rgpRecipients); }
            }
        }

        //typedef struct _CMSG_RC4_AUX_INFO {
        //  DWORD cbSize;
        //  DWORD dwBitLen;
        //} CMSG_RC4_AUX_INFO, *PCMSG_RC4_AUX_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_RC4_AUX_INFO
        {
            public int cbSize;
            public int dwBitLen;
        }

        //typedef struct _CMSG_CTRL_DECRYPT_PARA {
        //  DWORD cbSize;
        //  union {
        //    HCRYPTPROV        hCryptProv;
        //    NCRYPT_KEY_HANDLE hNCryptKey;
        //  } ;
        //  DWORD dwKeySpec;
        //  DWORD dwRecipientIndex;
        //} CMSG_CTRL_DECRYPT_PARA, *PCMSG_CTRL_DECRYPT_PARA;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_CTRL_DECRYPT_PARA
        {
            public int cbSize;
            public IntPtr hCryptProvOrNCryptKey;
            public int dwKeySpec;
            public int dwRecipientIndex;
        }

        //typedef struct _CRYPT_KEY_PROV_INFO {
        //  LPWSTR                pwszContainerName;
        //  LPWSTR                pwszProvName;
        //  DWORD                 dwProvType;
        //  DWORD                 dwFlags;
        //  DWORD                 cProvParam;
        //  PCRYPT_KEY_PROV_PARAM rgProvParam;
        //  DWORD                 dwKeySpec;
        //} CRYPT_KEY_PROV_INFO, *PCRYPT_KEY_PROV_INFO;
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_KEY_PROV_INFO
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszContainerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszProvName;
            public int dwProvType;
            public int dwFlags;
            public int cProvParam;
            public IntPtr rgProvParam;
            public int dwKeySpec;
        }

        #endregion

        #region "DELEGATES"

        internal delegate Boolean StreamOutputCallbackDelegate(IntPtr pvArg, IntPtr pbData, int cbData, Boolean fFinal);

        #endregion

        #region "API"

        //BOOL WINAPI CryptAcquireContext(
        //  __out  HCRYPTPROV *phProv,
        //  __in   LPCTSTR pszContainer,
        //  __in   LPCTSTR pszProvider,
        //  __in   DWORD dwProvType,
        //  __in   DWORD dwFlags
        //);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Boolean CryptAcquireContext(
            out SafeCSPHandle hProv,
            string pszContainer,
            string pszProvider,
            int dwProvType,
            int dwFlags
            );

        //BOOL WINAPI CryptEncodeObject(
        //  __in     DWORD dwCertEncodingType,
        //  __in     LPCSTR lpszStructType,
        //  __in     const void *pvStructInfo,
        //  __out    BYTE *pbEncoded,
        //  __inout  DWORD *pcbEncoded
        //);
        [DllImport("crypt32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern bool CryptEncodeObject(
            int dwCertEncodingType,
            string lpszStructType,
            ref long pvStructInfo,
            SafeNTHeapHandle pbEncoded,
            ref int pcbEncoded
            );

        //BOOL WINAPI CryptDecodeObject(
        //  __in     DWORD dwCertEncodingType,
        //  __in     LPCSTR lpszStructType,
        //  __in     const BYTE *pbEncoded,
        //  __in     DWORD cbEncoded,
        //  __in     DWORD dwFlags,
        //  __out    void *pvStructInfo,
        //  __inout  DWORD *pcbStructInfo
        //);
        [DllImport("crypt32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern Boolean CryptDecodeObject(
            int dwCertEncodingType,
            string lpszStructType,
            IntPtr pbEncoded,
            int cbEncoded,
            int dwFlags,
            out long pvStructInfo,
            ref int pcbStructInfo
            );
        [DllImport("crypt32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern Boolean CryptDecodeObject(
            int dwCertEncodingType,
            int lpszStructType,
            IntPtr pbEncoded,
            int cbEncoded,
            int dwFlags,
            StringBuilder pvStructInfo,
            ref int pcbStructInfo
            );

        //HCRYPTMSG WINAPI CryptMsgOpenToEncode(
        //  __in      DWORD dwMsgEncodingType,
        //  __in      DWORD dwFlags,
        //  __in      DWORD dwMsgType,
        //  __in      const void *pvMsgEncodeInfo,
        //  __in_opt  LPSTR pszInnerContentObjID,
        //  __in      PCMSG_STREAM_INFO pStreamInfo
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeMsgHandle CryptMsgOpenToEncode(
            int dwMsgEncodingType,
            int dwFlags,
            int dwMsgType,
            ref CMSG_SIGNED_ENCODE_INFO pvMsgEncodeInfo,
            string pszInnerContentObjID,
            ref CMSG_STREAM_INFO pStreamInfo
            );
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeMsgHandle CryptMsgOpenToEncode(
            int dwMsgEncodingType,
            int dwFlags,
            int dwMsgType,
            ref CMSG_ENVELOPED_ENCODE_INFO pvMsgEncodeInfo,
            string pszInnerContentObjID,
            ref CMSG_STREAM_INFO pStreamInfo
            );

        //HCRYPTMSG WINAPI CryptMsgOpenToDecode(
        //  __in      DWORD dwMsgEncodingType,
        //  __in      DWORD dwFlags,
        //  __in      DWORD dwMsgType,
        //  __in      HCRYPTPROV_LEGACY hCryptProv,
        //  __in      PCERT_INFO pRecipientInfo,
        //  __in_opt  PCMSG_STREAM_INFO pStreamInfo
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeMsgHandle CryptMsgOpenToDecode(
            int dwMsgEncodingType,
            int dwFlags,
            int dwMsgType,
            IntPtr hCryptProv,
            IntPtr pRecipientInfo,
            ref CMSG_STREAM_INFO pStreamInfo
            );
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeMsgHandle CryptMsgOpenToDecode(
            int dwMsgEncodingType,
            int dwFlags,
            int dwMsgType,
            IntPtr hCryptProv,
            IntPtr pRecipientInfo,
            IntPtr pStreamInfo
            );

        //BOOL WINAPI CryptMsgClose(
        //  __in  HCRYPTMSG hCryptMsg
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgClose(
            IntPtr hCryptMsg
            );

        //BOOL WINAPI CryptMsgUpdate(
        //  __in  HCRYPTMSG hCryptMsg,
        //  __in  const BYTE *pbData,
        //  __in  DWORD cbData,
        //  __in  BOOL fFinal
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgUpdate(
            IntPtr hCryptMsg,
            Byte[] pbData,
            int cbData,
            Boolean fFinal
            );

        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgUpdate(
            IntPtr hCryptMsg,
            IntPtr pbData,
            int cbData,
            Boolean fFinal
            );

        //BOOL WINAPI CryptMsgGetParam(
        //  __in     HCRYPTMSG hCryptMsg,
        //  __in     DWORD dwParamType,
        //  __in     DWORD dwIndex,
        //  __out    void *pvData,
        //  __inout  DWORD *pcbData
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgGetParam(
            SafeMsgHandle hCryptMsg,
            int dwParamType,
            int dwIndex,
            SafeNTHeapHandle pvData,
            ref int pcbData
            );
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgGetParam(
            SafeMsgHandle hCryptMsg,
            int dwParamType,
            int dwIndex,
            out int pvData,
            ref int pcbData
            );

        //BOOL WINAPI CryptMsgControl(
        //  __in  HCRYPTMSG hCryptMsg,
        //  __in  DWORD dwFlags,
        //  __in  DWORD dwCtrlType,
        //  __in  const void *pvCtrlPara
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgControl(
            SafeMsgHandle hCryptMsg,
            int dwFlags,
            int dwCtrlType,
            IntPtr pvCtrlPara
            );
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgControl(
            SafeMsgHandle hCryptMsg,
            int dwFlags,
            int dwCtrlType,
            ref CMSG_SIGNER_ENCODE_INFO pvCtrlPara
            );
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgControl(
            SafeMsgHandle hCryptMsg,
            int dwFlags,
            int dwCtrlType,
            ref BLOB pvCtrlPara
            );
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgControl(
            SafeMsgHandle hCryptMsg,
            int dwFlags,
            int dwCtrlType,
            ref CMSG_CTRL_VERIFY_SIGNATURE_EX_PARA pvCtrlPara
            );
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CryptMsgControl(
            SafeMsgHandle hCryptMsg,
            int dwFlags,
            int dwCtrlType,
            ref CMSG_CTRL_DECRYPT_PARA pvCtrlPara
            );

        //BOOL WINAPI CryptReleaseContext(
        //  __in  HCRYPTPROV hProv,
        //  __in  DWORD dwFlags
        //);
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern Boolean CryptReleaseContext(
            IntPtr hProv,
            int dwFlags
            );

        //PCCERT_CONTEXT WINAPI CertCreateCertificateContext(
        //  __in  DWORD dwCertEncodingType,
        //  __in  const BYTE *pbCertEncoded,
        //  __in  DWORD cbCertEncoded
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeCertContextHandle CertCreateCertificateContext(
            int dwCertEncodingType,
            SafeNTHeapHandle pbCertEncoded,
            int cbCertEncoded
            );

        //BOOL WINAPI CertFreeCertificateContext(
        //  __in  PCCERT_CONTEXT pCertContext
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern Boolean CertFreeCertificateContext(
            IntPtr pCertContext
            );

        //HCERTSTORE WINAPI CertOpenStore(
        //  __in  LPCSTR lpszStoreProvider,
        //  __in  DWORD dwMsgAndCertEncodingType,
        //  __in  HCRYPTPROV_LEGACY hCryptProv,
        //  __in  DWORD dwFlags,
        //  __in  const void *pvPara
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeStoreHandle CertOpenStore(
            int lpszStoreProvider,
            int dwMsgAndCertEncodingType,
            IntPtr hCryptProv,
            int dwFlags,
            SafeMsgHandle pvPara
            );

        //HCERTSTORE WINAPI CertOpenSystemStore(
        //  __in  HCRYPTPROV_LEGACY hprov,
        //  __in  LPTCSTR szSubsystemProtocol
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeStoreHandle CertOpenSystemStore(
            IntPtr hprov,
            string szSubsystemProtocol
            );

        //PCCERT_CONTEXT WINAPI CertGetSubjectCertificateFromStore(
        //  __in  HCERTSTORE hCertStore,
        //  __in  DWORD dwCertEncodingType,
        //  __in  PCERT_INFO pCertId
        //);        
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern SafeCertContextHandle CertGetSubjectCertificateFromStore(
            SafeStoreHandle hCertStore,
            int dwCertEncodingType,
            SafeNTHeapHandle pCertId
            );

        //BOOL WINAPI CertCloseStore(
        //  __in  HCERTSTORE hCertStore,
        //  __in  DWORD dwFlags
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern IntPtr CertCloseStore(
            IntPtr hCertStore,
            int dwFlags
            );

        //BOOL WINAPI CertGetCertificateContextProperty(
        //  __in     PCCERT_CONTEXT pCertContext,
        //  __in     DWORD dwPropId,
        //  __out    void *pvData,
        //  __inout  DWORD *pcbData
        //);
        [DllImport("Crypt32.dll", SetLastError = true)]
        internal static extern bool CertGetCertificateContextProperty(
            SafeCertContextHandle pCertContext,
            int dwPropId,
            SafeNTHeapHandle pvData,
            ref int pcbData
            );

        // void * memset ( void * ptr, int value, size_t num );
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        internal static extern IntPtr MemSet(
            IntPtr ptr,
            int value,
            int num
            );

        #endregion
    }
}