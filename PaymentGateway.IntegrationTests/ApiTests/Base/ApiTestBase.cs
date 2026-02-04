using Microsoft.AspNetCore.Mvc.Testing;
using PaymentGateway.Controllers;

namespace PaymentGateway.IntegrationTests.ApiTests.Base;

public abstract class ApiTestBase : IDisposable
{
    protected readonly WebApplicationFactory<PaymentsController> Factory = new();
    protected readonly HttpClient HttpClient;

    protected ApiTestBase()
    {
        HttpClient = Factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    // Additional service configuration for tests can be added here
                }))
            .CreateClient();
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        Factory.Dispose();
    }
}
