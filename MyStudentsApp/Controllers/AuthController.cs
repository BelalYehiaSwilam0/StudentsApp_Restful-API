using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using APIBusinessLayer;


namespace StudentApi.Controllers
{
    // This controller is responsible for authentication-related actions,
    // such as logging in and issuing JWT tokens.
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // This endpoint handles user login.
        // It verifies credentials and returns a JWT token if login succeeds.
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Send login data to the AuthService and wait for the token.
            var UserToken = AuthToken.Login(loginRequest);


            // If there is no token, tell the user "Invalid credentials".
            if (UserToken == null)
                return Unauthorized("Invalid credentials");


            return Ok(new { token = UserToken });
        }
    }
}
