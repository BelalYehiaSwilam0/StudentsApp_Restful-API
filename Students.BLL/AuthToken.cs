using APIBusinessLayer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace APIBusinessLayer
{
    public class AuthToken
    {
        // This method checks the user and returns a token.
        public static string Login(LoginRequest loginRequest)
        {
            // Find the user in the database by their name.
            APIBusinessLayer.clsUser user = clsUser.FindUserByUserName(loginRequest.UserName);

            // If the user does not exist, return null.
            if (user == null) 
                return null;

            // Compare the secret password with the user's input.
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);


            if (isPasswordCorrect)
            {
                // If the password is correct, create a new JWT token.
                return GenerateToken(user);
            }

            // If the password is wrong, return null.
            return null;
        }

        private static string GenerateToken(clsUser user)
        {
            // Get the secret key from EnvironmentVariable.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")));
            // Create the digital signature (signing credentials).
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Standard claims for the user
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Build the token with expiration time (30 minutes).
            var token = new JwtSecurityToken(
                issuer: "MyStudentsApp",
                audience: "ApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            // Convert the token object to a final string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}