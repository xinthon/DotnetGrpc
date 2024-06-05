// See https://aka.ms/new-console-template for more information
using DotnetGrpcService.Client.Protos;
using Grpc.Core;
using Grpc.Net.Client;

CallCredentials credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
{
    var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlNpbnRob24iLCJzdWIiOiJTaW50aG9uIiwianRpIjoiOGMyMmYxYTIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo1MjM3IiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI0NSJdLCJuYmYiOjE3MTc1MTU4NTksImV4cCI6MTcyNTQ2NDY1OSwiaWF0IjoxNzE3NTE1ODYxLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.CPm3IB9DDziO0I_aDIAm-mye6G1oDP6tA0jjKZln048";
    metadata.Add("Authorization", $"Bearer {token}");

    await Task.CompletedTask;
});

GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:7245", new GrpcChannelOptions
{
    Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
});

Greeter.GreeterClient client = new Greeter
    .GreeterClient(channel);


Auth.AuthClient authClient = new Auth
    .AuthClient(channel);

LoginResponse loginResponse = await authClient.LoginAsync(new LoginQuery()
{
    Email = "",
    Passwrod = ""
});

Console.WriteLine($"Token : {loginResponse.Token}");

HelloReply greeterReply = await client.SayHelloAsync(new HelloRequest()
{
    Name = "Sinthon Seng",
});

Console.WriteLine(greeterReply.Message);

Customer.CustomerClient customerClient = new Customer
    .CustomerClient(channel);

try
{
    var customerQuery = new GetCustomersStreamQuery()
    {
        PageNumber = 1,
        PageSize = 50
    };

    using (var serverStream = customerClient.GetCustomersStream(customerQuery))
    {
        int number = 1;
        await foreach (var response in serverStream.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine("Server stream response: " + number.ToString("0000000") + " : " + response.Name);
            number++;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.ReadLine();

