using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;
using Common.ThriftInterface;
using NLog;
using Thrift.Protocol;
using Thrift.Transport;

namespace Eon.Kiosk.Service.Controllers
{
    public class PatientsController : ApiController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public HttpResponseMessage Test()
        {
			logger.Debug("Qwerty");
			SayHelloToThrift();
	        return Request.CreateResponse(HttpStatusCode.OK, "The service works!");
        }

	    private void SayHelloToThrift()
	    {
			var transport = new TSocket("localhost", 9090);
			var protocol = new TBinaryProtocol(transport);
			var client = new LibraryService.Client(protocol);
			transport.Timeout = 100;

			try
			{
				transport.Open();

				var allBooks = client.GetAllBooks(); // Thrift call

				Console.WriteLine("Total number of books: {0}\n", allBooks.Count);

				if (allBooks.Count > 0)
				{
					Console.Write("Getting the first book: ");
					var firstBook = client.GetBook(allBooks.First().Id); // Thrift call

					Console.WriteLine("Id: {0}, {1} by {2}", firstBook.Id, firstBook.Title, firstBook.Author);
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine("Could not connect to the server: {0}.", e.Message);
			}
			catch (Exception e)
			{
				Console.WriteLine("An error occured: {0}", e.Message);
			}
		}
    }
}