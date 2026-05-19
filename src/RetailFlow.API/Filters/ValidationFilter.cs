using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using RetailFlow.Shared.Models;

namespace RetailFlow.API.Filters
{
    /// <summary>
    /// Automatically returns 400 Bad Request when ModelState is invalid.
    /// Eliminates boilerplate if(!ModelState.IsValid) checks in controllers.
    /// </summary>
    public class ValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var error = ApiErrorResponse.Create(
                    "VALIDATION_ERROR",
                    "One or more validation errors occurred.");

                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest, error);
            }
        }
    }
}
