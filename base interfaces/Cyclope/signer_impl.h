#pragma once

#include "signer.h"
#include "unknown_tmpl.h"
#include <unknown.h>

#include <memory>
#include <vector>
#include <iostream>
#include <iomanip>

// {3180a18e-07d5-403a-9e5c-2711527a7ee7}
const infotecs::base_interfaces::guids::INFOTECS_GUID IID_ISigner = 
{ 0x3180a18e, 0x07d5, 0x403a, { 0x9e, 0x5c, 0x27, 0x11, 0x52, 0x7a, 0x7e, 0xe7 } };

namespace cyclope {
	namespace crypto {
		namespace impl {

			class CSigner : public infotecs::common::impl::TUnknown<ISigner> {
			public:
				CSigner(IFile* inputFile, IFile* outputFile, char* cn, char* serialNumber);
				~CSigner();

				virtual int ITCSCALL Sign();
				virtual long ITCSCALL QueryInterface(REFIID riid, void** ppObject)
				{
					std::cerr << std::hex << "ICipher::QueryInterface " << std::setfill('0') <<
						std::setw(8) << riid.Data1 << "-" <<
						std::setw(4) << riid.Data2 << "-" <<
						std::setw(4) << riid.Data3 << "-";
					for (auto c : riid.Data4)
						std::cout << std::hex << std::setw(2) << std::setfill('0') << int(c);
					std::cout << std::endl;
					if (!ppObject)
					{
						return E_POINTER;
					}
					*ppObject = NULL;
					if (riid == IID_IUnknown)
					{
						*ppObject = static_cast<IUnknown*>(this);
					}
					if (riid == IID_ISigner)
					{
						*ppObject = static_cast<ISigner*>(this);
					}
					if (*ppObject)
					{
						AddRef();
						return S_OK;
					}
					return E_NOINTERFACE;
				}

			private:
				IFile* _inputFile;
				IFile* _outputFile;
				char* _cn;
				char* _serialNumber;
			};
		}
	}
}
