// See https://aka.ms/new-console-template for more information
using DotnetGrpcService.Client.Protos;
using Grpc.Net.Client;

GrpcChannel channel = GrpcChannel
    .ForAddress("http://localhost:5237");

Greeter.GreeterClient client = new Greeter
    .GreeterClient(channel);

var greeterReply = await client.SayHelloAsync(new HelloRequest()
{
    Name = "Sinthon Seng",
});

Console.WriteLine(greeterReply.Message);

Customer.CustomerClient customerClient = new Customer
    .CustomerClient(channel);

var customerReply = await customerClient.GetCustomersAsync(new GetCustomersQuery()
{
    PageNumber = 1,
    PageSize = 50
});

Console.WriteLine(customerReply);
Console.ReadLine();

