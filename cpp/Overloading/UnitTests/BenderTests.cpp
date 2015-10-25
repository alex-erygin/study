#include "stdafx.h"
#include "CppUnitTest.h"
#include "..\CppStudy\Bender.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace UnitTests
{		
	TEST_CLASS(BenderTests)
	{
	public:

		TEST_METHOD(Bend_NoExcepion) {
			Bender bender;
			bender.Bend();
			bender.Bend(10, 100);
		}

		TEST_METHOD(BendEquality_DifferentAges_False) {
			Bender bender1;
			bender1.age = 100;

			Bender bender2;
			bender2.age = 200;

			Assert::IsFalse(bender1 == bender2);
		}

		TEST_METHOD(BendEquality_EqualAges_True) {
			Bender bender1;
			bender1.age = 100;

			Bender bender2;
			bender2.age = 100;

			Assert::IsTrue(bender1 == bender2);
		}

	};
}