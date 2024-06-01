// See https://aka.ms/new-console-template for more information
using DotnetGrpcService.Client.Protos;
using Grpc.Core;
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

//var customerReply = customerClient.GetCustomersStream(new GetCustomersStreamQuery()
//{
//    PageNumber = 1,
//    PageSize = 50
//});

//Console.WriteLine(customerReply);


var customerQuery = new GetCustomersStreamQuery()
{
    PageNumber = 1,
    PageSize = 50
};

using (var serverStream = customerClient.GetCustomersStream(customerQuery))
{
    await foreach (var response in serverStream.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine("Server stream response: " + response.Name);
    }
}
Console.ReadLine();

