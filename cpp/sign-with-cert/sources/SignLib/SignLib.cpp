//// This is the main DLL file.
//
#include "stdafx.h"

#include "SignLib.h"

#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <windows.h>
#include <wincrypt.h>
#include <vector>


#define CERT_PERSONAL_STORE_NAME  L"My"
#define CERT_OTHER_PEOPLE_STORE_NAME L"AddressBook"
#define MY_TYPE  (PKCS_7_ASN_ENCODING | X509_ASN_ENCODING)
#define BUFSIZE 1024

#pragma comment(lib, "crypt32.lib")
#pragma comment(lib, "Advapi32.lib")

BOOL WINAPI CmsgStreamOutputCallback(
  const void *pvArg,  //in
  BYTE *pbData,       //in
  DWORD cbData,       //in
  BOOL fFinal         //in
  );

//прикрепленная подпись с одним подписантом.
void SignLib::Signer::Sign(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ sourceileName, System::String^ targetFileName)
{
     CMSG_SIGNER_ENCODE_INFO SignerEncodeInfoArray[1];

     //  инициализации контекста
     BOOL should_release_ctx = FALSE;

     BOOL bResult = FALSE;
     DWORD keySpec = 0;
     HCRYPTPROV hCryptProv = NULL;
     PCCERT_CONTEXT context = (PCCERT_CONTEXT)(void*)cert->Handle;
     HANDLE hDataFile = NULL; 
     BOOL include = TRUE;
     

     CryptAcquireCertificatePrivateKey(context, CRYPT_ACQUIRE_SILENT_FLAG, NULL, &hCryptProv, &keySpec, &should_release_ctx);
     
     CMSG_STREAM_INFO stStreamInfo;
     stStreamInfo.cbContent = 0xffffffff;
     stStreamInfo.pfnStreamOutput = CmsgStreamOutputCallback;
     stStreamInfo.pvArg = NULL;

     //--------------------------------------------------------------------
     // Initialize the algorithm identifier structure.
     const char* oid = "1.2.643.2.2.9";
     CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
     memset(&HashAlgorithm, 0, sizeof(HashAlgorithm)); // Init. to zero.
     HashAlgorithm.pszObjId = (char*)oid;	    // Initialize the necessary member.

     //--------------------------------------------------------------------
     // Initialize the CMSG_SIGNER_ENCODE_INFO structure.
     CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfo;
     memset(&SignerEncodeInfo, 0, sizeof(CMSG_SIGNER_ENCODE_INFO));
     SignerEncodeInfo.cbSize = sizeof(CMSG_SIGNER_ENCODE_INFO);
     SignerEncodeInfo.pCertInfo = context->pCertInfo;
     SignerEncodeInfo.hCryptProv = hCryptProv;
     SignerEncodeInfo.dwKeySpec = keySpec;
     SignerEncodeInfo.HashAlgorithm = HashAlgorithm;
     SignerEncodeInfo.pvHashAuxInfo = NULL;

     //--------------------------------------------------------------------
     // Create an array of one. Note: Currently, there can be only one
     // signer.
     SignerEncodeInfoArray[0] = SignerEncodeInfo;

     //--------------------------------------------------------------------
     // Initialize the CMSG_SIGNED_ENCODE_INFO structure.

     CERT_BLOB SignerCertBlob;
     SignerCertBlob.cbData = context->cbCertEncoded;
     SignerCertBlob.pbData = context->pbCertEncoded;

     //--------------------------------------------------------------------
     // Initialize the array of one CertBlob.

     CMSG_SIGNED_ENCODE_INFO SignedMsgEncodeInfo;
     memset(&SignedMsgEncodeInfo, 0, sizeof(CMSG_SIGNED_ENCODE_INFO));
     SignedMsgEncodeInfo.cbSize = sizeof(CMSG_SIGNED_ENCODE_INFO);
     SignedMsgEncodeInfo.cSigners = 1;
     SignedMsgEncodeInfo.rgSigners = SignerEncodeInfoArray;
     SignedMsgEncodeInfo.cCertEncoded = include;
     PCCERT_CHAIN_CONTEXT chainContext;
     CERT_BLOB SignerCertBlobArray[1];
     SignerCertBlobArray[0].cbData = context->cbCertEncoded;
     SignerCertBlobArray[0].pbData = context->pbCertEncoded;

     SignedMsgEncodeInfo.rgCertEncoded = &SignerCertBlobArray[0];
     SignedMsgEncodeInfo.rgCrlEncoded = NULL;

     DWORD dwFlags = 0;

     //--------------------------------------------------------------------
     // Open a message to encode.
     HCRYPTMSG hMsg;
     hMsg = CryptMsgOpenToEncode(
          (X509_ASN_ENCODING | PKCS_7_ASN_ENCODING),       // Message encoding type
          dwFlags,                       // Flags
          CMSG_SIGNED,             // Message type
          &SignedMsgEncodeInfo,    // Pointer to structure
          NULL,                    // Inner content object ID
          &stStreamInfo);          // Stream information (not used)

     //читаем файлег блоками
     {
          System::IO::Stream^ inputStream = System::IO::File::OpenRead(sourceileName);
          int bytesRead = 0;
          array<unsigned char>^ buffer = gcnew array<unsigned char>(8192);
          
          bool lastCall = FALSE;
          int bufferSize = bufferSize;
          while(bytesRead = inputStream->Read(buffer, 0, bufferSize > 0))
          {
               lastCall = bytesRead != bufferSize;
               pin_ptr<unsigned char> array_pin = &buffer[0];
               unsigned char * nativeArray = array_pin;
               CryptMsgUpdate(hMsg, nativeArray, (DWORD)bytesRead, lastCall);
          }
     }

     

     size_t dwBytesRead = 0;
     BOOL lastCall = FALSE;
}

BOOL WINAPI CmsgStreamOutputCallback(
  const void *pvArg,  //in
  BYTE *pbData,       //in
  DWORD cbData,       //in
  BOOL fFinal         //in
  ) {
       return 0;
}

void SignLib::Signer::Verify(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ dataFileName)
{
     System::Console::WriteLine("Verify");
}

void SignLib::Signer::Sign()
{
     HCERTSTORE hStorehandle = NULL;
     PCCERT_CONTEXT pSignerCert = NULL;
     HCRYPTPROV hCryptoProv = NULL;
     DWORD dsKeySpec = 0;
     HCRYPTHASH hHash = NULL;
     HANDLE hDataFile = NULL;
     BOOL bResult = FALSE;
     BYTE rgbFile[BUFSIZE];
     DWORD cbRead = 0;
     DWORD dwSigLen = 0;
     BYTE* pbSignature = NULL;
     HANDLE hSignatureFile = NULL;
     DWORD lpNumberOfBytesWritten = 0;

     System::Console::WriteLine("Signing");

     //open certificate store
     System::Console::WriteLine("Открываем хранилище сертификатов");
     hStorehandle = ::CertOpenStore(
          CERT_STORE_PROV_SYSTEM,
          0,
          NULL,
          CERT_SYSTEM_STORE_CURRENT_USER, CERT_PERSONAL_STORE_NAME);

     if (NULL == hStorehandle)
     {
          throw gcnew System::Exception("Не удалось открыть хранилище сертификатов");
     }

     PCCERT_CONTEXT pCertContext = NULL;
     while( pCertContext = CertEnumCertificatesInStore (hStorehandle, pCertContext) )
     {
          System::IntPtr certHandle((void*)pCertContext);
          System::Security::Cryptography::X509Certificates::X509Certificate2^ cert = gcnew System::Security::Cryptography::X509Certificates::X509Certificate2(certHandle);
          
          System::Console::WriteLine(cert->ToString());
     }
}