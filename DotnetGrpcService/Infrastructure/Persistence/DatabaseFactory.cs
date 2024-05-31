using DotnetGrpcService.Common.Abstractions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DotnetGrpcService.Infrastructure.Persistence
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly IConfiguration _configuration;
        public DatabaseFactory(IConfiguration configuration) 
        { 
            _configuration = configuration; 
        }

        public IDbConnection CreateConnection()
        {
            if(_configuration.GetConnectionString("Database") is null)
            {
                throw new ArgumentNullException("database connection string");
            }

            return new SqlConnection(_configuration.GetConnectionString("Database"));
        }
    }
}
