using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce_System.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(string userId, string userName, string role)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (int userId, string userName, string role) DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ReadJwtToken(token);

            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            var userNameClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);
            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            int userId = int.Parse(idClaim.Value);
            string userName = userNameClaim.Value;
            string role = roleClaim.Value;

            return (userId, userName, role);
        }
    }
}
