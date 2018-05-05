// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>
#include <string>
using namespace std;
int main()
{
	string s;
	
	cin >> s;

	int entriCount = 0;

	for(int i = 0; i < s.length(); ++i)
	{
		if (s[i] == 'f')
		{
			entriCount++;
		}

		if(entriCount == 2)
		{
			cout << i;
			return 0;
		}
	}

	if (entriCount == 0)
	{
		cout << -2;
	}
	else 
	{
		cout << -1;
	}

    return 0;
}