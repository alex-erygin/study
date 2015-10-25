#include "stdafx.h"
#include "Bender.h"
#include <iostream>
using namespace std;

Bender::Bender()
{
}


Bender::~Bender()
{
}

void Bender::Bend(){
	cout << "Just bend\n";
}

void Bender::Bend(double force, double angle){
	cout << "Bend with force " << force << " and angle " << angle << "\n";
}
