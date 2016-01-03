// Vector.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <iostream>
#include <ostream>
#include <algorithm>
#include <string>

using namespace std;

int main()
{
	vector<string> vector;

	vector.push_back("Hulio");
	vector.push_back("Garsia");
	vector.push_back("Ivanovich");
	vector.push_back("Beer");

	sort(vector.begin(), vector.end(), 
		[](const string& left, const string& right) {
			return left.size() < right.size();
		});

	for (auto i = vector.begin(); i != vector.end(); ++i) {
		cout << *i << endl;
	}
	cin.get();
    return 0;
}

