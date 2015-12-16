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

void SignLib::Signer::Sign(System::String^ signerName, System::String^ fileName, System::String^ signatureFileName)
{
	System::Console::WriteLine("Sign");
}

void SignLib::Signer::Verify(System::String^ signerName, System::String^ signatureFileName, System::String^ dataFileName)
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
     hStorehandle = ::CertOpenStore(
          CERT_STORE_PROV_SYSTEM,
          0,
          NULL,
          CERT_SYSTEM_STORE_CURRENT_USER, CERT_PERSONAL_STORE_NAME);


}