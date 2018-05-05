// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>;
#include <vector>;
using namespace std;
int main()
{
	int n;
	cin >> n;
	vector<int> v;

	int sum = 0;
	for(int i=0; i < n; ++i)
	{
		int x = 0;
		cin >> x;
		sum += x;
		v.push_back(x);
	}

	int avg = sum / n;
	
	vector<int> positions;
	int big = 0;
	for (int i = 0; i < n; ++i)
	{
		if (v[i] > avg)
		{
			++big;
			positions.push_back(i);
		}
	}

	cout << big << endl;
	for(int p:positions)
	{
		cout << p << " ";
	}
    return 0;
}