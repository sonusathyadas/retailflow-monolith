using System;
using System.Web;
using System.Web.Http;
using Serilog;

namespace RetailFlow.API
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // OWIN Startup handles all configuration via Startup.cs
        }

        protected void Application_End()
        {
            Log.Information("RetailFlow API shutting down");
            Log.CloseAndFlush();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
                Log.Fatal(ex, "Unhandled application error");
        }
    }
}
