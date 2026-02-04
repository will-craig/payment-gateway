using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Services;
using PaymentGateway.Application.Validator;
using PaymentGateway.BankClient;
using PaymentGateway.DAL;
using PaymentGateway.Domain.Providers;
using TimeProvider = PaymentGateway.Domain.Providers.TimeProvider;

namespace PaymentGateway.Configuration;

public static class ServicesConfigurator
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
         builder.Services.AddOpenApi();
         builder.Services.AddControllers();
         builder.Services.AddSwaggerGen();
         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddDbContext<IPaymentGatewayDbContext, PaymentGatewayDbContext>(options => options.UseInMemoryDatabase("PaymentGatewayDb"));
         
         builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
         builder.Services.AddScoped<IPaymentService, PaymentService>();
         builder.Services.AddScoped<IProcessPaymentValidator, ProcessPaymentValidator>();
         
         // Configure HttpClient for Acquiring Bank API
         var bankUrl = builder.Configuration.GetSection("BankClientSettings")["BaseUrl"] 
                       ?? throw new InvalidOperationException("BankClientSettings:BaseUrl is not configured.");
         
         builder.Services.AddHttpClient<IBankClient, AcquiringBankClient>(client =>
         {
             client.BaseAddress = new Uri(bankUrl);
             client.Timeout = TimeSpan.FromSeconds(30);
         });
         
         return builder;
    }
}