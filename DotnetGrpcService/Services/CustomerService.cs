using Azure;
using Dapper;
using DotnetGrpcService.Common.Abstractions;
using DotnetGrpcService.Protos;
using Grpc.Core;

namespace DotnetGrpcService.Services
{
    public class CustomerService : Customer.CustomerBase
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly IDatabaseFactory _factory;

        public CustomerService(ILogger<CustomerService> logger, IDatabaseFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public override async Task<CustomerList> GetCustomers(GetCustomersQuery request, ServerCallContext context)
        {
            _logger.LogInformation("GetCustomers called with query: {Query}", request);

            var response = new CustomerList();

            using(var connection = _factory.CreateConnection())
            {
                IEnumerable<CustomerResponse> customers = await connection
                    .QueryAsync<CustomerResponse>("SELECT * FROM Customers");

                foreach(CustomerResponse customer in customers)
                {
                    response.Customers.Add(customer);
                }
            }

            _logger.LogInformation("Returning {Count} customers", response.Customers.Count);

            return response;
        }

        public override async Task GetCustomersStream(
            GetCustomersStreamQuery request,
            IServerStreamWriter<CustomerResponse> responseStream,
            ServerCallContext context)
        {
            using(var connection = _factory.CreateConnection())
            {
                IEnumerable<CustomerResponse> customers = await connection
                    .QueryAsync<CustomerResponse>("SELECT * FROM Customers");

                foreach(CustomerResponse customer in customers)
                {
                    await responseStream.WriteAsync(customer);
                    await Task.Delay(1000);
                }
            }
        }

        public override async Task<CustomerResponse> GetCustomerById(
            GetCustomerByIdQuery request,
            ServerCallContext context)
        {
            using(var connection = _factory.CreateConnection())
            {
                CustomerResponse? customer = await connection
                     .QueryFirstOrDefaultAsync<CustomerResponse>(
                         "SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                         new { request.CustomerId });

                if(customer == null)
                {
                    throw new Exception("Customer was not found");
                }

                return customer;
            }
        }

        public override async Task<CustomerResponse> CreateCustomer(
            CreateCustomerRequest request,
            ServerCallContext context)
        {
            using(var connection = _factory.CreateConnection())
            {
                await connection.ExecuteAsync(
                    "INSERT INTO Customers(CustomerId, Name, LastName, Email, Address) VALUES(@CustomerId, @Name, @LastName, @Email, @Address)");

                return new CustomerResponse()
                {
                    CustomerId = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    LastName = request.LastName,
                    Email = request.Email,
                    Address = request.Address,
                };
            }
        }

        public override async Task<CustomerResponse> DeleteCustomer(
            DeleteCustomerRequest request,
            ServerCallContext context)
        {
            using(var connection = _factory.CreateConnection())
            {
                CustomerResponse? customer = await connection.QueryFirstOrDefaultAsync<CustomerResponse>(
                    "SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                    new { request.CustomerId });

                if(customer == null)
                {
                    throw new Exception("Faild to create the customer");
                }

                await connection.ExecuteAsync(
                    "INSERT INTO Customers(CustomerId, Name, LastName, Email, Address) VALUES(@CustomerId, @Name, @LastName, @Email, @Address)");

                return customer;
            }
        }

        public override async Task<CustomerResponse> UpdateCustomer(
            UpdateCustomerRequest request,
            ServerCallContext context)
        {
            using(var connection = _factory.CreateConnection())
            {
                await connection.ExecuteAsync(
                    "UPDATE Customers SET Name = @Name, LastName = @LastName, Email = @Email, Address = @Address WHERE CustomerId = @CustomerId",
                    request);

                return new CustomerResponse()
                {
                    CustomerId = request.CustomerId,
                    Name = request.Name,
                    LastName = request.LastName,
                    Email = request.Email,
                    Address = request.Address,
                };
            }
        }
    }
}
