using System;
using System.Runtime.InteropServices;
using System.Security;

/* 
	Security transparent код не может:

	-утверждать (вызовом метода Assert()) какие-либо разрешения или поднимать привилегии (privilege elevation);
	-содержать небезопасный (unsafe) или неверифицируемый (unverifiable) код;
	-обращаться напрямую к critical коду;
	-вызывать неуправляемый код, а также код, декорированный атрибутом SuppressUnmanagedCodeSecurityAttribute
	-осуществлять хоть какое-то наследование от типов из critical-слоя;
	-перекрывать (override) любые виртуальные методы из critical-слоя;
	-реализовывать любые интерфейсы из critical-слоя;
	-обращаться к любому члену, защищенному флагом SecurityAction.LinkDemand
*/

// Говорим, что сборка SecurityTransparent
[assembly: SecurityTransparent]
namespace SecurityTransparentConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			//упадем, потомучто вызываем неуправляемый код.
			uint ui = GetVersion();
			Console.ReadKey(true);
		}

		[DllImport("kernel32.dll")]
		static extern uint GetVersion();
	}
}
