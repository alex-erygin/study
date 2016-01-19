#include "signer.h"
#include "signer_impl.h"
#include "IFile.h"
#include "unknown_tmpl.h"

namespace cyclope {
	namespace crypto {
		namespace impl {

			CSigner::CSigner(IFile* inputFile, IFile* outputFile, char* cn, char* serialNumber) : TUnknown(1)
				, _inputFile(inputFile)
				, _outputFile(outputFile)
				, _cn(cn)
				, _serialNumber(serialNumber)
			{

			}

			CSigner::~CSigner() {

			}

			int ITCSCALL CSigner::Sign()
			{
				unsigned char buffer[100];
				_inputFile->Read(buffer, 100, 0, 100);
				_outputFile->Write(buffer, 100, 0, 100);

				return 0;
			}
		}
	}
}