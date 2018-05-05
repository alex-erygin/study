// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>
#include <vector>

using namespace std;
int main()
{
	vector<int> bytes;
	int x, r=0;
	cin >> x;

	while(x != 0)
	{
		r = x % 2;
		bytes.push_back(r);
		x = x / 2;
	}
	
	for(int i = bytes.size()-1; i >= 0; i--)
	{
		cout << bytes[i];
	}
    return 0;
}