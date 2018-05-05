// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>

using namespace std;
int main()
{
	double n,a,b,x,y, s;
	cin >> n >> a >> b >> x >> y;

	if (n > b)
	{
		s = n - n * y / 100;
		cout << s;
		return 0;
	}

	if (n > a)
	{
		s = n - n * x / 100;
		cout << s;
		return 0;
	}

	cout << n;

    return 0;
}