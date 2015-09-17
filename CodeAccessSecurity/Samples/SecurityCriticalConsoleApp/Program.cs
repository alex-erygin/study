using System;
using System.Runtime.InteropServices;
using System.Security;

//Коду SecurityCritical можно все
[assembly: SecurityCritical]
namespace SecurityCriticalConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			//все сработает, потому что мы - SecurityCritical 
			uint ui = GetVersion();
			Console.ReadKey(true);
		}

		[DllImport("kernel32.dll")]
		static extern uint GetVersion();
	}
}
