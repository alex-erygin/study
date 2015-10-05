// helloWorld.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "DedMoroz.h"


int _tmain(int argc, _TCHAR* argv[])
{
	auto dedMoroz = new DedMoroz();
	dedMoroz->SayHello();

	return 0;
}