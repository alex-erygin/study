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

///<summary>
/// Калбак - запись содержимого сообщения.
///</summary>
BOOL WINAPI CmsgStreamOutputCallback(
  IN const void *pvArg,  //in
  IN BYTE *pbData,       //in
  IN DWORD cbData,       //in
  IN BOOL fFinal         //in
  );


///<summary>
/// Подписать файл.
///</summary>
///<param name="cert">Сертификат для подписи.</param>
///<param name="sourceileName">Путь к файлу, который необходимо подписать.</param>
///<param name="targetFileName">Путь к выходному файлу.</param>
void SignLib::Signer::Sign(
     System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, 
     System::String^ sourceileName, 
     System::String^ targetFileName)
{
     CMSG_SIGNER_ENCODE_INFO signer[1];
     BOOL shouldReleaseCryptoContext = FALSE;
     
     DWORD keySpec = 0;
     HCRYPTPROV hCryptProv = NULL;
     PCCERT_CONTEXT context = (PCCERT_CONTEXT)(void*)cert->Handle;
     HANDLE hDataFile = NULL; 
     BOOL include = TRUE;
     DWORD error= GetLastError();
     //получили контекст криптопровайдера.
     auto result = CryptAcquireCertificatePrivateKey(context, 0, NULL, &hCryptProv, &keySpec, &shouldReleaseCryptoContext);
     
     //Настраиваем CMSG_STREAM_INFO.
     CMSG_STREAM_INFO streamInfo;
     //используем BER-кодировку.
     streamInfo.cbContent = 0xffffffff;
     //указываем калбэк.
     streamInfo.pfnStreamOutput = CmsgStreamOutputCallback;

     // Инициализируем алгоритм хеширования.
     // todo: Алгоритм подписи надо вытаскивать из сертификата или передавать как параметр.
     char* oid = "1.2.643.2.2.9";
     CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
     memset(&HashAlgorithm, 0, sizeof(HashAlgorithm)); // Init. to zero.
     HashAlgorithm.pszObjId = oid;    // Initialize the necessary member.

     // Заполняем информацию о подписанте (пока он только один).
     CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfo;
     memset(&SignerEncodeInfo, 0, sizeof(CMSG_SIGNER_ENCODE_INFO));
     SignerEncodeInfo.cbSize = sizeof(CMSG_SIGNER_ENCODE_INFO);
     SignerEncodeInfo.pCertInfo = context->pCertInfo;
     SignerEncodeInfo.hCryptProv = hCryptProv;
     SignerEncodeInfo.dwKeySpec = keySpec;
     SignerEncodeInfo.HashAlgorithm = HashAlgorithm;
     SignerEncodeInfo.pvHashAuxInfo = NULL;
     signer[0] = SignerEncodeInfo;

     // Инициализируем блоб
     CERT_BLOB SignerCertBlob;
     SignerCertBlob.cbData = context->cbCertEncoded;
     SignerCertBlob.pbData = context->pbCertEncoded;

     //--------------------------------------------------------------------
     // Initialize the array of one CertBlob.

     CMSG_SIGNED_ENCODE_INFO SignedMsgEncodeInfo;
     memset(&SignedMsgEncodeInfo, 0, sizeof(CMSG_SIGNED_ENCODE_INFO));
     SignedMsgEncodeInfo.cbSize = sizeof(CMSG_SIGNED_ENCODE_INFO);
     SignedMsgEncodeInfo.cSigners = 1;
     SignedMsgEncodeInfo.rgSigners = signer;
     SignedMsgEncodeInfo.cCertEncoded = include;
     PCCERT_CHAIN_CONTEXT chainContext;

     std::vector<CERT_BLOB> SignerCertBlobArray;
     /*заполняем цепочку сертификатов*/
     CERT_ENHKEY_USAGE        EnhkeyUsage;
     EnhkeyUsage.cUsageIdentifier = 0;
     EnhkeyUsage.rgpszUsageIdentifier=NULL;

     CERT_USAGE_MATCH         CertUsage;
     CertUsage.dwType = USAGE_MATCH_TYPE_AND;
     CertUsage.Usage  = EnhkeyUsage;

     CERT_CHAIN_PARA          ChainPara;
     ChainPara.cbSize = sizeof(CERT_CHAIN_PARA);
     ChainPara.RequestedUsage=CertUsage;

     //строим цепочку
     CertGetCertificateChain(NULL, context, NULL, NULL, &ChainPara, 0, 0, &chainContext);
     SignedMsgEncodeInfo.cCertEncoded = chainContext->rgpChain[0]->cElement - 1;
     SignerCertBlobArray.resize(SignedMsgEncodeInfo.cCertEncoded);

     for( DWORD i = 0; i < SignedMsgEncodeInfo.cCertEncoded; i++ )
     {
          SignerCertBlobArray[i].cbData = chainContext->rgpChain[0]->rgpElement[i]->pCertContext->cbCertEncoded;
          SignerCertBlobArray[i].pbData = chainContext->rgpChain[0]->rgpElement[i]->pCertContext->pbCertEncoded;
     }

     SignedMsgEncodeInfo.rgCertEncoded = &SignerCertBlobArray[0];
     SignedMsgEncodeInfo.rgCrlEncoded = NULL;

     if( SignerCertBlobArray.size() )
          SignedMsgEncodeInfo.rgCertEncoded = &SignerCertBlobArray.at(0);
     SignedMsgEncodeInfo.rgCrlEncoded = NULL;

     DWORD dwFlags = 0;

     //--------------------------------------------------------------------
     // Open a message to encode.
     HCRYPTMSG hMsg;
     hMsg = CryptMsgOpenToEncode(
          (X509_ASN_ENCODING | PKCS_7_ASN_ENCODING),       // Message encoding type
          dwFlags,                 // Flags
          CMSG_SIGNED,             // Message type
          &SignedMsgEncodeInfo,    // Pointer to structure
          NULL,                    // Inner content object ID
          &streamInfo);          // Stream information (not used)

     error= GetLastError();
     //читаем файлег блоками
     
     System::IO::Stream^ inputStream = System::IO::File::OpenRead(sourceileName);
     int bytesRead = 0;
     int bufferSize = 8192;
     array<unsigned char>^ buffer = gcnew array<unsigned char>(bufferSize);

     BOOL lastCall = FALSE;

     while((bytesRead = inputStream->Read(buffer, 0, bufferSize)) > 0)
     {
          lastCall = bytesRead != bufferSize;
          pin_ptr<unsigned char> array_pin = &buffer[0];
          unsigned char * nativeArray = array_pin;
          CryptMsgUpdate(hMsg, nativeArray, (DWORD)bytesRead, lastCall);
     }
     inputStream->Close();

     CryptMsgClose(hMsg);
     CryptReleaseContext(hCryptProv,0);
}

BOOL WINAPI CmsgStreamOutputCallback(
  const void *pvArg,  //in
  BYTE *pbData,       //in
  DWORD cbData,       //in
  BOOL fFinal         //in
  ) {
       array<unsigned char>^ buffer = gcnew array<unsigned char>(cbData);
       for (int i = 0; i < cbData; i++)
       {
            buffer[i] = pbData[i];
       }
       //TODO: передавать хендл на файл через *pvArg, использовать с++ для записи в файл.
       System::IO::Stream^ stream = System::IO::File::Open("f:\\_VM\\output.bin", System::IO::FileMode::Append);

       stream->Write(buffer,0,buffer->Length);

       stream->Close();

       return TRUE;
}

void SignLib::Signer::Verify(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ dataFileName)
{
     System::Console::WriteLine("Verify");
}