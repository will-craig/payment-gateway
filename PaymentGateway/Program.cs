using PaymentGateway.Configuration;

WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .ConfigureMiddleware()
    .Run();

