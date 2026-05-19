using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RetailFlow.API.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ── CORS ──────────────────────────────────────────────────────────
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // ── Routes ────────────────────────────────────────────────────────
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // ── JSON formatting ───────────────────────────────────────────────
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // Remove XML formatter — JSON only
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
