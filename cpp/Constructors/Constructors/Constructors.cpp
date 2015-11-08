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


class Box {
public:
	//direct member initialization, which is more efficient than using assignment operators inside the constructor body
	Box(int width, int length, int height) 
		: _width(width), _height(height), _length(length) 
	{ }

	//explicit constructor;
	Box(int size) : _width(size), _height(size), _length(size)
	{ }

	int volume() {
		return _width*_height*_length;
	}
private:
	int _width;
	int _height;
	int _length;
};

int _tmain(int argc, _TCHAR* argv[])
{
	//see order of construction.
	DerivedContainer d;

	Box b(2, 2, 2);
	cout << "Box volume is " << b.volume() << endl;

	//explicit constructor call
	Box b2 = 3;
	cout << "Box volume is " << b2.volume() << endl;
	return 0;
}