// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>

using namespace std;
int main()
{
	int a,b;
	cin >> a >> b;

	if (b == 0)
	{
		cout << "Impossible";
		return 0;
	}

	int c = a / b;

	cout << c;

    return 0;
}