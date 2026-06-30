using APIBusinessLayer.Auth.DTOs;
using Azure.Core;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIBusinessLayer.AuthDTOs
{
    public class AuthToken
    {
        public static async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            // Find user by user name
            clsUser user = await
                clsUser.FindUserByUserNameAsync(loginRequest.UserName);

            // Check if user exists
            if (user == null)
                return null;

            // Check password
            bool isPasswordCorrect =BCrypt.Net.BCrypt.Verify(loginRequest.Password,user.PasswordHash);

            // If password is wrong return null
            if (!isPasswordCorrect)
                return null;

            // Generate access token
            var accessToken = GenerateToken(user);

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();

            // Hash refresh token before save
            string hashedRefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);

            // Check if refresh token exists
            if (user.RefreshTokenInfo == null)
            {
                // Create new refresh token object
                user.RefreshTokenInfo =
                    new clsUserRefreshToken();
            }

            // Update refresh token Info
            user.RefreshTokenInfo.UserID = user.UserID;

            user.RefreshTokenInfo.RefreshTokenHash = hashedRefreshToken;

            user.RefreshTokenInfo.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            user.RefreshTokenInfo.RefreshTokenRevokedAt = null;

            // Save new token data
            bool isSaved = await user.RefreshTokenInfo.SaveAsync();

            // Check save result
            if (!isSaved)
                return null;

            // Return tokens
            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
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

            // Build the token with expiration time (5 minutes).
            var token = new JwtSecurityToken(
                issuer: "MyStudentsApp",
                audience: "ApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),// Shorter life for Access Token
                signingCredentials: credentials
            );

            // Convert the token object to a final string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static async Task<TokenResponse> RefreshTokenAsync(RefreshRequest refreshRequest)
        {

            // Find user by user name
            clsUser user = await clsUser.FindUserByUserNameAsync(refreshRequest.UserName);

            // Check if user exists
            if (user == null)
                return null;

            // Check if refresh token exists
            if (user.RefreshTokenInfo == null)
                return null;

            // Check if token is revoked
            if (user.RefreshTokenInfo.RefreshTokenRevokedAt != null)
                return null;

            // Check token expiration
            if (user.RefreshTokenInfo.RefreshTokenExpiresAt<= DateTime.UtcNow)
            {
                return null;
            }

            // Verify refresh token hash
            bool isValidToken =BCrypt.Net.BCrypt.Verify( refreshRequest.RefreshToken,user.RefreshTokenInfo.RefreshTokenHash);
            // Check token validation
            if (!isValidToken)
                return null;

            // Generate new access token
            var newAccessToken =GenerateToken(user);

            // Generate new refresh token
            var newRefreshToken =GenerateRefreshToken();

            // Hash new refresh token
            string hashedRefreshToken = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);

            // Update refresh token data
            user.RefreshTokenInfo.RefreshTokenHash = hashedRefreshToken;

            user.RefreshTokenInfo.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            user.RefreshTokenInfo.RefreshTokenRevokedAt = null;

            // Save new token data
            bool isSaved = await user.RefreshTokenInfo.SaveAsync();

            // Check save result
            if (!isSaved)
                return null;

            // Return new tokens
            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        public static async Task<bool> RevokeTokenAsync(LogoutRequest logoutRequest)
        {
            // Step 1: Retrieve user details from the database using the provided username.
            clsUser user = await clsUser.FindUserByUserNameAsync(logoutRequest.UserName);

            // Step 2: Security Best Practice - Constant Time/Silent Failure.
            // If the user is not found, we return 'true' to prevent "User Enumeration" attacks,
            // where an attacker could probe the API to see which usernames exist.
            if (user == null)
                return true; //Don't reveal if user exists

            // Step 3: Verify the authenticity of the Refresh Token.
            bool refreshValid = BCrypt.Net.BCrypt.Verify(logoutRequest.RefreshToken, user.RefreshTokenInfo.RefreshTokenHash);

            // If the token is invalid, we exit silently for security reasons.
            if (!refreshValid)
                return true;

            // Step 4: Perform the "Revocation".
            user.RefreshTokenInfo.RefreshTokenRevokedAt = DateTime.UtcNow;

            // Save changes in database
            return await user.RefreshTokenInfo.SaveAsync();
        }
    }
}