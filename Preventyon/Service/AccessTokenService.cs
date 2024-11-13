using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Preventyon.Service
{
    public class AccessTokenService
    {
        private readonly IConfiguration _configuration;
        private string secretKey;

        public AccessTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            secretKey = _configuration.GetValue<string>("Jwt:Key");
        }
        public string GetAccessToken(string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Token;
        }
    }
}
