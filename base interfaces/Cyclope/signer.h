#pragma once

#ifndef __PROCESSING_H__
#define __PROCESSING_H__

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

		struct ISigner : public IUnknown
		{
			virtual int ITCSCALL Sign();
		};

	} //namespace cyclope
} //namespace crypto

#endif /* __PROCESSING_H__ */

// {3180a18e-07d5-403a-9e5c-2711527a7ee7}
DEFINE_INFOTECS_GUID(IID_Signer, 0x3180a18e, 0x07d5, 0x403a, 0x9e5c, 0x2711527a7ee7);