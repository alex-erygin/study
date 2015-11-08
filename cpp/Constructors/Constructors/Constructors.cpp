// Constructors.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>

using namespace std;

class Contained1 {
public:
	Contained1(){
		cout << "Contained1 ctor" << endl;
	}
};

class Contained2 {
public:
	Contained2(){
		cout << "Contained2 ctor" << endl;
	}
};

class Contained3 {
public:
	Contained3(){
		cout << "Contained3 ctor" << endl;
	}
};

class BaseContainer {
public:
	BaseContainer(){
		cout << "BaseContainer ctor" << endl;
	}
private:
	Contained1 c1;
	Contained2 c2;
};

class DerivedContainer : BaseContainer {
public:
	DerivedContainer(){
		cout << "DerivedContainer ctor" << endl;
	}
private:
	Contained3 c3;
};

int _tmain(int argc, _TCHAR* argv[])
{
	//see order of construction.
	DerivedContainer d;
	return 0;
}