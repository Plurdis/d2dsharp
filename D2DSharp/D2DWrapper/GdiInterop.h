#pragma once


#include "ComWrapper.h"
#include "DWCommon.h"

using namespace System;
using namespace System::Runtime::InteropServices;


namespace Managed { namespace Graphics { namespace DirectWrite 
{
	public ref class GdiInterop: ComWrapper
	{
	internal:
		GdiInterop(IDWriteGdiInterop* native): ComWrapper(native)
		{
		}

	internal:
		IDWriteGdiInterop* GetNative() new 
		{
			return (IDWriteGdiInterop*)ComWrapper::GetNative();
		}
	};
}}}