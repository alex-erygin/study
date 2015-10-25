// Overloading.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Bender.h"

int _tmain(int argc, _TCHAR* argv[])
{
	auto bender = new Bender();
	bender->Bend();
	bender->Bend(10, 20);
	return 0;
}

