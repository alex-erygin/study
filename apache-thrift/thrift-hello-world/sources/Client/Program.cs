using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellothrift;
using Thrift.Protocol;
using Thrift.Transport;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			TTransport transport = new TSocket("localhost", 9090);
			TProtocol protocol = new TBinaryProtocol(transport);
			TMultiplexedProtocol mp = new TMultiplexedProtocol(protocol, "MyService");
			MyService.Client client = new MyService.Client(mp);

			using (transport)
			{
				transport.Open();
				client.Ping();
			}
		}
	}
}
