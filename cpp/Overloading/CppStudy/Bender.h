#pragma once
class __declspec(dllexport) Bender
{
public:
	Bender();
	~Bender();

	void Bend();
	void Bend(double force, double angle);
};

