using System;
using System.Web.Http;
using RetailFlow.Shared.Models;

namespace RetailFlow.API.Controllers
{
    /// <summary>
    /// Health check endpoint used by load balancers, Docker, and Azure App Service.
    /// </summary>
    [RoutePrefix("api/health")]
    [AllowAnonymous]
    public class HealthController : ApiController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = System.Configuration.ConfigurationManager.AppSettings["Environment"]
            }));
        }
    }
}
