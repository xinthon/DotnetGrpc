using System.Security.Claims;

namespace DotnetGrpcService.Common.Abstractions
{
    public interface IJWTProvider
    {
        string GenerateToken(Claim[] claims);
    }
}
