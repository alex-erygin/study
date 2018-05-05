// ConsoleApp.cpp : Defines the entry point for the console application.
//
#include <iostream>
#include <vector>
#include <string>

using namespace std;
int main()
{
	int n;
	cin >> n;

	vector<int> people;
	for(int i=0; i < n; i++)
	{
		string cmd;
		cin >> cmd;
		if(cmd == "COME")
		{
			int newComes;
			cin >> newComes;
			people.resize(people.size() + newComes, 0);
		}
		else if(cmd == "WORRY")
		{
			int worry = 0;
			cin >> worry;
			people[worry] = 1;
		}
		else if(cmd == "WORRY_COUNT")
		{
			int count = 0;
			for(int i=0; i < people.size(); i++)
			{
				if(people[i] != 0)
				{
					++count;
				}
			}

			cout << count << endl;
		}
		else if(cmd == "QUIET")
		{
			int quiet = 0;
			cin >> quiet;
			people[quiet] = 0;
		}
	}

    return 0;
}