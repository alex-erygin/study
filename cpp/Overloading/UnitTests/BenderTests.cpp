#include "stdafx.h"
#include "CppUnitTest.h"
#include "..\CppStudy\CppStudy.h"
#include "..\CppStudy\Bender.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace UnitTests
{		
	TEST_CLASS(BenderTests)
	{
	public:

		TEST_METHOD(BendTest) {
			Bender bender;
			bender.Bend();
			bender.Bend(10, 100);
		}

	};
}