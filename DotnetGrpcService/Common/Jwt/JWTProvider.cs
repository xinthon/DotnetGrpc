using DotnetGrpcService.Common.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotnetGrpcService.Common.Jwt
{

    public class JWTProvider : IJWTProvider
    {
        private readonly string _secretKey;

        public JWTProvider(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("SecretKey");
        }

        public string GenerateToken(Claim[] claims)
        {
            JwtSecurityTokenHandler tokenHandler
                = new JwtSecurityTokenHandler();

            byte[] key = Encoding
                .UTF8
                .GetBytes(_secretKey);

            SigningCredentials credentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials
            };

            SecurityToken token = tokenHandler
                .CreateToken(tokenDescriptor);

            string tokenValue = tokenHandler.WriteToken(token);

            return tokenValue;
        }
    }
}
