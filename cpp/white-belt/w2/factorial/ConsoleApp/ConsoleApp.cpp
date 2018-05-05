// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>;
#include <string>
#include <vector>

using namespace std;
/*
int Factorial(int n)
{
	if (n < 0)
	{
		return 1;
	}

	if (n == 0)
	{
		return 1;
	}

	int result = 1;
	for (int i = 1; i <= n; i++)
	{
		result *= i;
	}

	return result;
}


bool IsPalindrom(string value)
{
	for(int i = 0; i < value.length(); i++)
	{
		if (value[i] != value[value.length() - i - 1])
		{
			return false;
		}
	}

	return true;
}

vector<string> PalindromFilter(vector<string> input, int minLength)
{
	vector<string> result;
	for(string s: input)
	{
		if (s.length() >= minLength && IsPalindrom(s))
		{
			result.push_back(s);
		}
	}
	return result;
}

void UpdateIfGreater(int a, int& b)
{
	if(a > b)
	{
		b = a;
	}
}


void MoveStrings(vector<string>& src, vector<string>& dst)
{
	for(string s: src)
	{
		dst.push_back(s);
	}
	src.clear();
}*/


vector<int> Reversed(const vector<int>& v)
{
	vector<int> result = v;
	for(int i = 0; i < result.size() / 2; i++)
	{
		const int tmp = result[i];
		result[i] = result[v.size() - i - 1];
		result[result.size() - i - 1] = tmp;
	}
	return result;
}
/*
int main()
{
	string s;
	cin >> s;
	cout << IsPalindrom(s);
    return 0;
}
*/