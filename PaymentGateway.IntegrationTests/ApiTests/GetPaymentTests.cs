using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.DAL;
using PaymentGateway.IntegrationTests.ApiTests.Base;
using PaymentGateway.Models.Responses;

namespace PaymentGateway.IntegrationTests.ApiTests;

public class GetPaymentTests : ApiTestBase
{
    private readonly Guid _id;

    public GetPaymentTests()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentGatewayDbContext>();
        //initialize in memory database with a payment
        var payment = new Domain.Entities.Payment
        {
            Id = Guid.NewGuid(),
            Amount = 10000, //100 usd in cents
            Currency = "USD",
            CreatedAt = DateTime.UtcNow,
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Status = Domain.Enums.PaymentStatus.Authorized,
            BankAuthorizationCode = Guid.NewGuid().ToString(),
            CardNumber = "12345678910123456",
            Cvv = "123"
        };
        dbContext.Payments.Add(payment);
        dbContext.SaveChanges();
        _id = payment.Id;
    }
    
    [Fact]
    public async Task Get_payments_returns_ok_response()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/payments/{_id}");

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            var paymentResponse = System.Text.Json.JsonSerializer.Deserialize<GetPaymentResponse>(responseContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            paymentResponse.Should().NotBeNull();
            paymentResponse.Id.Should().Be(_id);
            paymentResponse.Amount.Should().Be(10000);
            paymentResponse.Currency.Should().Be("USD");
            paymentResponse.ExpiryMonth.Should().Be(12);
            paymentResponse.ExpiryYear.Should().Be(2028);
            paymentResponse.LastFourCardDigits.Should().Be("3456");
    }

    [Fact]
    public async Task Get_payments_returns_notfound_response()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/payments/{Guid.NewGuid()}");

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
