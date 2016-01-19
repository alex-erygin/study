#pragma once

#ifdef _MSC_VER
#  ifdef CYCLOPE_EXPORTS
#    define CYCLOPE_API __declspec(dllexport)
#  else
#    define CYCLOPE_API __declspec(dllimport)
#  endif
#else
#  ifndef CIPHER_API
#    define CIPHER_API extern "C"
#  endif
#endif

//#include <unknown.h>
//#include <uni_call.h>
//#include <uni_guiddef.h>

//E904BF6A-AEF3-4BA1-87 F6-CF 34 15 01 32 EC
const infotecs::base_interfaces::guids::INFOTECS_GUID IID_IFile =
{ 0xE904BF6A, 0xAEF3, 0x4BA1,{ 0x87, 0xF6, 0xCF, 0x34, 0x15, 0x01, 0x32, 0xEC } };

namespace cyclope {
	namespace crypto {
		struct IFile : public IUnknown {
			virtual int32_t ITCSCALL Read(
				unsigned char* buffer,
				int32_t bufferSize,
				int32_t index,
				int32_t count) = 0;

			virtual void ITCSCALL Write(
				unsigned char* buffer,
				int32_t bufferSize,
				int32_t index,
				int32_t count) = 0;
		};
	}// namespace crypto
}// namespace cyclope

 // {8C543536-87C8-415E-AA7D-65EFE41E1ED1}
DEFINE_INFOTECS_GUID(IID_IFile,
	0xE904BF6A, 0xAEF3, 0x4BA1, 0x87, 0xF6, 0xCF, 0x34, 0x15, 0x01, 0x32, 0xEC);