//// This is the main DLL file.
//
#include "stdafx.h"

#include "SignLib.h"

#include <stdio.h>
#include <conio.h>
#include <windows.h>
#include <wincrypt.h>

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

     CryptAcquireCertificatePrivateKey(context, CRYPT_ACQUIRE_SILENT_FLAG, NULL, &hCryptProv, &keySpec, &should_release_ctx);
     
     CMSG_STREAM_INFO stStreamInfo;
     stStreamInfo.cbContent = 0xffffffff;
     stStreamInfo.pfnStreamOutput = CmsgStreamOutputCallback;
     stStreamInfo.pvArg = NULL;

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