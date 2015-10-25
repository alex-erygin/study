#include "stdafx.h"
#include "Bender.h"
#include <iostream>

Bender::Bender()
{
}


Bender::~Bender()
{
}

void Bender::Bend() {
	std::cout << "Just bend";
}

void Bender::Bend(double force, double angle) {
	std::cout << "Bend with force " << force << " and angle " << angle << "\n";
}