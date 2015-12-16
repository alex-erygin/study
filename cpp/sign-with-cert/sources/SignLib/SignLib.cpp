//// This is the main DLL file.
//
#include "stdafx.h"

#include "SignLib.h"

void SignLib::Signer::Sign(System::String^ signerName, System::String^ fileName, System::String^ signatureFileName)
{
	System::Console::WriteLine("Sign");
}

void SignLib::Signer::Verify(System::String^ signerName, System::String^ signatureFileName, System::String^ dataFileName)
{
	System::Console::WriteLine("Verify");
}