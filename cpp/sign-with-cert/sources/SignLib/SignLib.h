// SignLib.h

#pragma once

#include <stdio.h>
#include <conio.h>
#include <windows.h>
#include <wincrypt.h>

#define CERT_PERSONAL_STORE_NAME  LФMyФ
#define CERT_OTHER_PEOPLE_STORE_NAME LФAddressBookФ
#define MY_TYPE  (PKCS_7_ASN_ENCODING | X509_ASN_ENCODING)
#define BUFSIZE 1024

using namespace System;

namespace SignLib {

	public ref class Signer
	{
		// TODO: здесь следует добавить свои методы дл€ этого класса.
	public:
		void Sign(wchar_t* signerName, wchar_t* fileName, wchar_t* signatureFileName);
		void Verify(wchar_t* signerName, wchar_t* signatureFileName, wchar_t* dataFileName);
	};
}