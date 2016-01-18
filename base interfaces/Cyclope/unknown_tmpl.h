#pragma once
/// @file
/// @brief Sample TUnknown template. Should not be used in a production.
/// @copyright Copyright (c) InfoTeCS. All Rights Reserved.

#ifndef __UNKNOWN_IMPL_H__
#define __UNKNOWN_IMPL_H__

#include <uni_errors.h>

#include <atomic>
#include <limits>
#include <cassert>
#include <unknown.h>

namespace infotecs {
	namespace common {
		namespace impl {

			/// @brief Sample template for implemention IUnknown inteface functions.
			/// NOTICE: this is here for example only. It is prohibited to use this as-is in the production code.
			template<typename T = IUnknown>
			class TUnknown : public T
			{
			public:
				// Set initial reference in the constructor.
				explicit TUnknown(long ref) : ref_(ref) {}
				// Destructor.
				virtual ~TUnknown() {}

				/// IUnknown implementation
				//@{
				virtual long ITCSCALL QueryInterface(REFIID /*riid*/, void** ppObject)
				{
					if (!ppObject)
					{
						return E_POINTER;
					}
					return E_NOTIMPL;
				}

				virtual unsigned long ITCSCALL AddRef(void)
				{
					return 1 + std::atomic_fetch_add(&ref_, 1L);
				}

				virtual unsigned long ITCSCALL Release(void)
				{
					const long ref = std::atomic_fetch_sub(&ref_, 1L) - 1;

					if (ref < 0)
					{
						// Double release error. Set a breakpoint here.
						assert(false);
						return std::numeric_limits<unsigned long>::max();
					}

					if (ref == 0)
						OnFinalRelease();

					// don't use result explicitly
					return ref;
				}

			protected:
				std::atomic<long> ref_;

				/// This will be called on final release.
				virtual void OnFinalRelease() { delete this; }
			};

		}//namespace impl
	}//namespace base_interfaces
}//namespace infotecs

#endif /* __UNKNOWN_IMPL_H__ */
