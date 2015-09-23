using System;
using Microsoft.Win32;

namespace DangerousLib
{
	public class RegistryReader
	{
		public void ReadRegistry()
		{
			RegistryKey rk = Registry.CurrentConfig;
			string[] names = rk.GetSubKeyNames();
			Console.WriteLine("Ваша песенка спета!");
		}
	}
}
