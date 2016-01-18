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

#include <unknown.h>
#include <uni_call.h>
#include <uni_guiddef.h>

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