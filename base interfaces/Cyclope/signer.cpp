#include <uni_guiddef.h>
#include "signer.h"
#include "signer_impl.h"

#ifndef _WIN32
/// {00000000-0000-0000-C000-000000000046}
const infotecs::base_interfaces::guids::INFOTECS_GUID IID_IUnknown =
{ 0x00000000, 0x0000, 0x0000,{ 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 } };
#endif

int ITCSCALL CreateSigner(
	cyclope::crypto::IFile * inputFile, 
	cyclope::crypto::IFile * outputFile, 
	char * cn, 
	char * serialNumber, 
	cyclope::crypto::ISigner ** signer)
{
	try
	{
		if (!signer)
		{
			return 0;
		}

		*signer = new cyclope::crypto::impl::CSigner(inputFile, outputFile, cn, serialNumber);
		if (*signer) {
			return 1;
		}
		else {
			return 0;
		}
	}
	catch (...)
	{
		*signer = 0;
		return 0;
	}
}