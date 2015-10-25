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

bool Bender::operator==(const Bender& other){
	return other.age == this->age;
}

bool operator ==(const Bender & bender1, const Bender & bender2){
	return bender1.age == bender2.age;
}