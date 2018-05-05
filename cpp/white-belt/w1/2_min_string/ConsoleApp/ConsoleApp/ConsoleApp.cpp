// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>
#include <vector>
#include <string>
#include <algorithm>

using namespace std;
int main()
{
	vector<string> values;
	string input;

	for (int i=0; i < 3; ++i) {
		cin >> input;
		values.push_back(input);
	}

	cout << *std::min_element(values.begin(), values.end());

    return 0;
}