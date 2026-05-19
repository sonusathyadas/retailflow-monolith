using System.Configuration;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using RetailFlow.API.App_Start;
using RetailFlow.API.Filters;
using RetailFlow.API.Middleware;

[assembly: OwinStartup(typeof(RetailFlow.API.Startup))]

namespace RetailFlow.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // ── Logging ───────────────────────────────────────────────────────
            LoggingConfig.Configure();

            // ── Web API ───────────────────────────────────────────────────────
            var config = new HttpConfiguration();

            // JWT Bearer authentication
            var jwtSecret = ConfigurationManager.AppSettings["Jwt:Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "RetailFlow",
                    ValidateAudience = true,
                    ValidAudience = "RetailFlow",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true
                }
            });

            // Global filters
            config.Filters.Add(new GlobalExceptionFilter());
            config.Filters.Add(new ValidationFilter());

            // Request logging handler
            config.MessageHandlers.Add(new RequestLoggingHandler());

            // DI
            AutofacConfig.Configure(config);

            // Routes & formatting
            WebApiConfig.Register(config);

            // Swagger
            SwaggerConfig.Register(config);

            // Hangfire
            HangfireConfig.Configure();

            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireServer();

            app.UseWebApi(config);
        }
    }
}
