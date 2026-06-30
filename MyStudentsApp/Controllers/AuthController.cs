using APIBusinessLayer.Auth.DTOs;
using APIBusinessLayer.AuthDTOs;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace StudentApi.Controllers
{
    // This controller is responsible for authentication-related actions,
    // such as logging in and issuing JWT tokens.
    [Authorize]
    [ApiController]
    [EnableRateLimiting("AuthLimiter")] //Now refresh is protected.

    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        // This endpoint handles user login.
        // It verifies credentials and returns a JWT token if login succeeds.
        [AllowAnonymous]
        [HttpPost("login")]
        //[EnableRateLimiting("AuthLimiter")] //Now refresh is protected.

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Capture caller IP once (used in all logs for tracing)
            // We store IP as a string and default to "unknown" to avoid null issues.
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Send login data to the AuthService and wait for the token.
            var UserToken = AuthToken.Login(loginRequest);

            //Security logging: record the failure safely 
            //This helps detect brute-force / credential stuffing attempts.
            if (UserToken == null)
            {
                _logger.LogWarning(
                "Failed login attempt (UserName not found). UserName={UserName}, IP={IP}",
                loginRequest.UserName,
                ip
                );

                return Unauthorized("Invalid credentials");
            }


            return Ok(new { token = UserToken });
        }

        [HttpPost("refresh-token")]
        //[EnableRateLimiting("AuthLimiter")] //Now refresh is protected.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            // Capture caller IP once (used in all logs for tracing)
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            //  We call the refresh method to get a new token.
            var result = AuthToken.RefreshToken(refreshRequest);

            if (result == null)
            {
                _logger.LogWarning(
                    "Invalid refresh attempt (UserName not found). UserName={UserName}, IP={IP}",
                    refreshRequest.UserName,
                    ip
                );

                return Unauthorized("Invalid refresh request");
            }


            return Ok(result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout([FromBody] LogoutRequest logoutRequest)
        {
            //  We stop the token so the user is logged out.
            var success = AuthToken.RevokeToken(logoutRequest);

            if (!success) return Ok(); // Do not reveal if user exists

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
