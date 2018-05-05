// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>
#include <cmath>

using namespace std;
int main()
{
	double a,b,c;
	cin >> a >> b >> c;

	if (a == 0 && b != 0)
	{
		cout << -1 * c / b;
		return 0;
	}

	if (a == 0 && b == 0)
	{
		return 0;
	}

	double d = b*b - 4 * a*c;
	
	if (d < 0)
	{
		return 0;
	}

	double x1 = (-1 * b + sqrt(d)) / (2 * a);
	double x2 = (-1 * b - sqrt(d)) / (2 * a);

	if (x1 == x2)
	{
		cout << x1;
		return 0;
	}

	cout << x1 << " " << x2;

    return 0;
}