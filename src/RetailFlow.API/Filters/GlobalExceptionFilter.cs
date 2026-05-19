using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using RetailFlow.Shared.Models;
using Serilog;

namespace RetailFlow.API.Filters
{
    /// <summary>
    /// Global exception handler — catches unhandled exceptions and returns a consistent error response.
    /// Prevents stack traces from leaking to clients.
    /// </summary>
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private static readonly ILogger _log = Log.ForContext<GlobalExceptionFilter>();

        public override void OnException(HttpActionExecutedContext context)
        {
            _log.Error(context.Exception,
                "Unhandled exception in {Controller}.{Action}",
                context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
                context.ActionContext.ActionDescriptor.ActionName);

            var error = ApiErrorResponse.Create(
                "INTERNAL_ERROR",
                "An unexpected error occurred. Please try again later.");

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError, error);
        }
    }
}
