#pragma once
class __declspec(dllexport) Bender
{
public:
	int age;

	Bender();
	~Bender();

	void Bend();
	void Bend(double force, double angle);

	bool operator == (const Bender & other);
};

bool operator ==(const Bender & bender1, const Bender & bender2);