using System.Data;

namespace DotnetGrpcService.Common.Abstractions
{
    public interface IDatabaseFactory
    {
        IDbConnection CreateConnection();
    }
}
