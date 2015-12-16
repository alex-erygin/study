// This is the main DLL file.

#include "stdafx.h"

#include "SignLib.h"

void SignLib::Signer::Sign(wchar_t * signerName, wchar_t * fileName, wchar_t * signatureFileName)
{
	System::Console::WriteLine("Sign");
}

void SignLib::Signer::Verify(wchar_t * signerName, wchar_t * signatureFileName, wchar_t * dataFileName)
{
	System::Console::WriteLine("Verify");
}