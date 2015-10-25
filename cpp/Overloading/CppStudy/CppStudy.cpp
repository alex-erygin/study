// CppStudy.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "CppStudy.h"
#include <iostream>

// This is the constructor of a class that has been exported.
// see CppStudy.h for the class definition
CCppStudy::CCppStudy()
{
	return;
}

void CCppStudy::Bend() {
	std::cout << "Just bend";
}

void CCppStudy::Bend(double force, double angle) {
	std::cout << "Bend with force " << force << " and angle " << angle << "\n";
}
