using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce_System.Services
{
    public class JwtService : IJwtService
    {
        // The IConfiguration instance is injected through the constructor to read configuration settings.
        private readonly IConfiguration _config;

        // Constructor accepts IConfiguration to access configuration values like the JWT secret key and expiry time.
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Generates a JWT token for the user with provided details (userId, userName, and role).
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="userName">The user's name.</param>
        /// <param name="role">The user's role (e.g., Admin, User).</param>
        /// <returns>A string containing the JWT token.</returns>
        public string GenerateJwtToken(string userId, string userName, string role)
        {
            // Fetch JWT settings from configuration (e.g., secret key, token expiry time).
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];  // The secret key used for signing the JWT token.

            // Define the claims (payload data) to be included in the JWT.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId), // Subject: the unique identifier of the user
                new Claim(JwtRegisteredClaimNames.Name, userName), // Name: the username of the user
                new Claim(ClaimTypes.Role, role), // Role: the role of the user (e.g., Admin, User)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID: unique identifier for the token
            };

            // Create a security key from the secret key, using UTF-8 encoding.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Create signing credentials using the security key and the HMAC SHA-256 algorithm.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create a JwtSecurityToken object containing the claims, expiration time, and signing credentials.
            var token = new JwtSecurityToken(
                claims: claims, // Claims to be included in the token
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])), // Expiration time of the token
                signingCredentials: creds // The signing credentials (used to sign the token)
            );

            // Return the generated JWT as a string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Decodes the provided JWT token and extracts the user's information (userId, userName, role).
        /// </summary>
        /// <param name="token">The JWT token string to be decoded.</param>
        /// <returns>A tuple containing the userId, userName, and role extracted from the token.</returns>
        public (int userId, string userName, string role) DecodeToken(string token)
        {
            // Initialize a JwtSecurityTokenHandler to read and parse the JWT token.
            var tokenHandler = new JwtSecurityTokenHandler();

            // Read the JWT token and parse its claims into a JwtSecurityToken object.
            var principal = tokenHandler.ReadJwtToken(token);

            // Extract individual claims from the token.
            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub); // User ID claim
            var userNameClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name); // User name claim
            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role); // User role claim

            // Convert the user ID claim value from string to integer.
            int userId = int.Parse(idClaim.Value);

            // Extract the user name and role from the claims.
            string userName = userNameClaim.Value;
            string role = roleClaim.Value;

            // Return the decoded user information as a tuple.
            return (userId, userName, role);
        }
    }
}
