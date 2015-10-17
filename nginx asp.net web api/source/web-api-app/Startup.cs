using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using Owin;

namespace Eon.Kiosk.Service
{
    public class Startup
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        /// <param name="appBuilder"><see cref="IAppBuilder"/>.</param>
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureRoutes(config);

            ConfigureCors(config);

            appBuilder.UseWebApi(config); 
        }


        /// <summary>
        /// Configure Cors.
        /// </summary>
        /// <param name="config"><see cref="HttpConfiguration"/>.</param>
        private static void ConfigureCors(HttpConfiguration config)
        {
            var allowedOrigins = ConfigurationManager.AppSettings["AllowedOriginAddresses"];
            var attribute = new EnableCorsAttribute(origins: allowedOrigins, exposedHeaders: "*", headers: "*",
                methods: "*") {SupportsCredentials = true};

            config.EnableCors(attribute);
            config.MapHttpAttributeRoutes();
        }


        /// <summary>
        /// Configure Routes
        /// </summary>
        /// <param name="config"><see cref="HttpConfiguration"/>.</param>
        private static void ConfigureRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Patient-TestAction",
                routeTemplate: "api/Patients/Test",
                defaults: new {controller = "Patients", action = "Test"}
                );

            config.Routes.MapHttpRoute(
                name: "Api-3",
                routeTemplate: "api/{controller}/{action}"
                );
        }
    }
}