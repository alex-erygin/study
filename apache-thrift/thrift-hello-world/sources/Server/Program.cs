using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new Server();
			Console.WriteLine("Start server. Press any key to exit");
			server.Start();
			
			Console.ReadKey();
		}
	}
}
