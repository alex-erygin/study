#include "signer.h"
#include "signer_impl.h"
#include "IFile.h"

//E904BF6A-AEF3-4BA1-87 F6-CF 34 15 01 32 EC
const infotecs::base_interfaces::guids::INFOTECS_GUID IID_IFile =
{ 0xE904BF6A, 0xAEF3, 0x4BA1, {0x87, 0xF6, 0xCF, 0x34, 0x15, 0x01, 0x32, 0xEC } };

namespace cyclope {
	namespace impl {
		
		CSigner::CSigner(IFile* inputFile, IFile* outputFile, char* cn, char* serialNumber) : TUnknown( 1 )
			, _inputFile(inputFile)
			, _outputFile(outputFile)
			, _cn(cn)
			, _serialNumber(serialNumber)
		{
			
		}

		CSigner::~CSigner() {

		}

		int ITCSCALL CSigner::Sign() {
			unsigned char buffer[100];
			_inputFile->Read(buffer, 100, 0, 100);
			_outputFile->Write(buffer, 100, 0, 100);
		}
	}
}