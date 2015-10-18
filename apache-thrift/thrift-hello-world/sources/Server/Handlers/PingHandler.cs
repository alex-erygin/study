using System;
using hellothrift;

namespace Server.Handlers
{
	class PingHandler : MyService.Iface
	{
		public void Ping()
		{
			Console.WriteLine("Ping received.");
		}

		public SampleResponseDto SendRequest(SampleRequestDto request)
		{
			return new SampleResponseDto() {Data = "Привет."};
		}
	}
}
