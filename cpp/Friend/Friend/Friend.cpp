#include "stdafx.h"
#include <iostream>
#include <string.h>

using namespace std;

class child;

class schoolchild
{
	char name[16];
	char surname[16];
	int clas;
public:
	schoolchild(char*, char*, int);
	void getData();
	friend child;
};

schoolchild::schoolchild(char *n, char *s, int c){
	strcpy_s(name, n);
	strcpy_s(surname, s);
	clas = c;
}

void schoolchild::getData(){
	cout << name << " " << surname << "\t" << clas << "-й класс" << endl;
}

class child {
public :
	void changeClass(schoolchild&, int);
	void getChangeData(schoolchild);
};

void child::changeClass(schoolchild& obj, int newClas) {
	obj.clas = newClas;
}

void child::getChangeData(schoolchild obj){
	cout << obj.name << " " << obj.surname << " переведен в " << obj.clas << "-й класс" << endl;
}

int _tmain(int argc, _TCHAR* argv[])
{
	setlocale(LC_ALL, "rus");
	schoolchild ivanov ("Иван", "Иванов", 3);
	schoolchild petrov ("Петр", "Петров", 4);

	cout << "Список учеников" << endl;
	ivanov.getData();
	petrov.getData();

	child transfer;
	transfer.changeClass(ivanov, 10);
	transfer.getChangeData(ivanov);

	return 0;
}