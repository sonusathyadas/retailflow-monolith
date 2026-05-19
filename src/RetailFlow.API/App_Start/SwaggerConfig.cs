using System.Web.Http;
using Swashbuckle.Application;

namespace RetailFlow.API.App_Start
{
    public static class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "RetailFlow API")
                 .Description("Monolithic Retail Order Fulfillment Platform — ASP.NET Web API (.NET 4.8)")
                 .Contact(cc => cc.Name("RetailFlow Team"));

                c.IncludeXmlComments(GetXmlCommentsPath());
                c.DescribeAllEnumsAsStrings();
                c.UseFullTypeNameInSchemaIds();
            })
            .EnableSwaggerUi(c =>
            {
                c.DocumentTitle("RetailFlow API Explorer");
                c.EnableDiscoveryUrlSelector();
            });
        }

        private static string GetXmlCommentsPath()
        {
            return System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                "bin", "RetailFlow.API.xml");
        }
    }
}
