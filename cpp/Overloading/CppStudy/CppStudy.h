// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the CPPSTUDY_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// CPPSTUDY_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef CPPSTUDY_EXPORTS
#define CPPSTUDY_API __declspec(dllexport)
#else
#define CPPSTUDY_API __declspec(dllimport)
#endif

// This class is exported from the CppStudy.dll
class CPPSTUDY_API CCppStudy {
public:
	CCppStudy(void);
	// TODO: add your methods here.
	void Bend();
	void Bend(double force, double angle);
};