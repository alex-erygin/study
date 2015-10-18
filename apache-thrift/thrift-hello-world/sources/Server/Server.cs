using System;
using hellothrift;
using Server.Handlers;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace Server
{
	public class Server
	{
		public void Start()
		{
			var handler = new PingHandler();
			var processor = new MyService.Processor(handler);
			TMultiplexedProcessor multiProcessor = new TMultiplexedProcessor();
			multiProcessor.RegisterProcessor("MyService", processor);

			TServerTransport transport = new TServerSocket(9090);
			var transport2 = new TMemoryBuffer(new byte[4096]);

			TServer server = new TThreadPoolServer(multiProcessor, transport);
			server.Serve();
		}
	}
}