using APIBusinessLayer.Auth.DTOs;
using APIBusinessLayer.AuthDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;


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
        // This endpoint handles user login.
        // It verifies credentials and returns a JWT token if login succeeds.
        [AllowAnonymous]
        [HttpPost("login")]
        //[EnableRateLimiting("AuthLimiter")] //Now refresh is protected.

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Send login data to the AuthService and wait for the token.
            var UserToken = AuthToken.Login(loginRequest);


            // If there is no token, tell the user "Invalid credentials".
            if (UserToken == null)
                return Unauthorized("Invalid credentials");


            return Ok(new { token = UserToken });
        }

        [HttpPost("refresh-token")]
        //[EnableRateLimiting("AuthLimiter")] //Now refresh is protected.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            //  We call the refresh method to get a new token.
            var result = AuthToken.RefreshToken(refreshRequest);

            if (result == null)
                return Unauthorized("Invalid refresh token");

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
