using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Shared.Models;
using Serilog;

namespace RetailFlow.API.Controllers
{
    /// <summary>
    /// Handles user registration, login, and token refresh.
    /// </summary>
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;
        private static readonly ILogger _log = Log.ForContext<AuthController>();

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Register a new customer account.</summary>
        [HttpPost, Route("register")]
        [AllowAnonymous]
        public IHttpActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = _authService.Register(request);
                return Content(HttpStatusCode.Created, ApiResponse<AuthResponse>.Ok(result, "Registration successful."));
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.Conflict,
                    ApiErrorResponse.Create("EMAIL_TAKEN", ex.Message));
            }
        }

        /// <summary>Authenticate and receive JWT tokens.</summary>
        [HttpPost, Route("login")]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = _authService.Login(request);
                return Ok(ApiResponse<AuthResponse>.Ok(result));
            }
            catch (UnauthorizedAccessException)
            {
                return Content(HttpStatusCode.Unauthorized,
                    ApiErrorResponse.Create("INVALID_CREDENTIALS", "Email or password is incorrect."));
            }
        }

        /// <summary>Refresh an expired access token.</summary>
        [HttpPost, Route("refresh")]
        [AllowAnonymous]
        public IHttpActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = _authService.RefreshToken(request.RefreshToken);
                return Ok(ApiResponse<AuthResponse>.Ok(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Content(HttpStatusCode.Unauthorized,
                    ApiErrorResponse.Create("INVALID_REFRESH_TOKEN", ex.Message));
            }
        }

        /// <summary>Get user profile by ID.</summary>
        [HttpGet, Route("~/api/users/{id:int}")]
        [Authorize]
        public IHttpActionResult GetUser(int id)
        {
            var user = _authService.GetUserById(id);
            if (user == null)
                return NotFound();

            return Ok(ApiResponse<UserDto>.Ok(user));
        }
    }
}
