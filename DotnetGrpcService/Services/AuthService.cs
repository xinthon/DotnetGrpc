using DotnetGrpcService.Common.Abstractions;
using DotnetGrpcService.Protos;
using Grpc.Core;
using System.Security.Claims;

namespace DotnetGrpcService.Services
{
    public class AuthService : Auth.AuthBase
    {
        private IJWTProvider _jwt;

        public AuthService(IJWTProvider jwt)
        {
            _jwt = jwt;
        }

        public override Task<LoginResponse> Login(LoginQuery request, ServerCallContext context)
        {
            Claim[] claims = [
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Email, "admin@example.com"),
                new Claim(ClaimTypes.MobilePhone, "+855 88 999 777"),
                new Claim(ClaimTypes.Role, "Administrator"),
            ];

            string token = _jwt
                .GenerateToken(claims);

            LoginResponse loginResponse = new LoginResponse()
            {
                Token = token
            };
            
            return Task.FromResult(loginResponse);
        }
    }
}
