using System.Net;
using System.Net.Http;
using System.Web.Http;
using NLog;

namespace Eon.Kiosk.Service.Controllers
{
    public class PatientsController : ApiController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public HttpResponseMessage Test()
        {
			logger.Debug("Qwerty");
            return Request.CreateResponse(HttpStatusCode.OK, "The service works!");
        }
    }
}