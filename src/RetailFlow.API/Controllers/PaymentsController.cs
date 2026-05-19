using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Shared.Models;

namespace RetailFlow.API.Controllers
{
    /// <summary>
    /// Payment processing. Uses a mock gateway with 90% success rate.
    /// </summary>
    [RoutePrefix("api/payments")]
    [Authorize]
    public class PaymentsController : ApiController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>Process payment for an order.</summary>
        [HttpPost, Route("process")]
        public IHttpActionResult ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var payment = _paymentService.ProcessPayment(request);
                return Ok(ApiResponse<PaymentDto>.Ok(payment));
            }
            catch (KeyNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound,
                    ApiErrorResponse.Create("ORDER_NOT_FOUND", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.Conflict,
                    ApiErrorResponse.Create("PAYMENT_ERROR", ex.Message));
            }
        }

        /// <summary>Get payment details by ID.</summary>
        [HttpGet, Route("{id:int}")]
        public IHttpActionResult GetPayment(int id)
        {
            var payment = _paymentService.GetById(id);
            if (payment == null) return NotFound();
            return Ok(ApiResponse<PaymentDto>.Ok(payment));
        }

        /// <summary>Retry a failed payment. Max 3 attempts.</summary>
        [HttpPost, Route("{id:int}/retry")]
        [Authorize(Roles = "Admin,FinanceManager")]
        public IHttpActionResult RetryPayment(int id)
        {
            try
            {
                var payment = _paymentService.RetryPayment(id);
                return Ok(ApiResponse<PaymentDto>.Ok(payment, "Payment retried."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.Conflict,
                    ApiErrorResponse.Create("RETRY_ERROR", ex.Message));
            }
        }
    }
}
