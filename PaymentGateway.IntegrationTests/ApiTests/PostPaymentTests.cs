using PaymentGateway.Domain.Enums;
using PaymentGateway.IntegrationTests.ApiTests.Base;
using PaymentGateway.Models.Responses;

namespace PaymentGateway.IntegrationTests.ApiTests;

public class PostPaymentTests : ApiTestBase
{
    [Fact]
    public async Task Create_Payment_Returns_OK_Authorised()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var paymentRequest = new
        {
            Amount = 10000,
            Currency = "GBP",
            CardNumber = "2222405343248877", // authorised - odd last digit
            ExpiryMonth = date.Month,
            ExpiryYear = date.Year + 1,
            Cvv = "123"
        };
        var requestContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(paymentRequest), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "api/payments")
        {
            Content = requestContent
        };

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var paymentResponse = System.Text.Json.JsonSerializer.Deserialize<PostPaymentResponse>(responseContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        paymentResponse.Should().NotBeNull();
        paymentResponse.Status.Should().Be(nameof(PaymentStatus.Authorized));
        paymentResponse.Id.Should().NotBeEmpty();
        paymentResponse.Amount.Should().Be(paymentRequest.Amount);
        paymentResponse.Currency.Should().Be(paymentRequest.Currency);
        paymentResponse.ExpiryMonth.Should().Be(paymentRequest.ExpiryMonth);
        paymentResponse.ExpiryYear.Should().Be(paymentRequest.ExpiryYear);
        paymentResponse.LastFourCardDigits.Should().Be("8877");
    }
    
    [Fact]
    public async Task Create_Payment_Returns_OK_Declined()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var paymentRequest = new
        {
            Amount = 10000,
            Currency = "GBP",
            CardNumber = "2222405343248878", //declined - even last digit
            ExpiryMonth = date.Month,
            ExpiryYear = date.Year + 1,
            Cvv = "123"
        };
        var requestContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(paymentRequest), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "api/payments")
        {
            Content = requestContent
        };

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var paymentResponse = System.Text.Json.JsonSerializer.Deserialize<PostPaymentResponse>(responseContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        paymentResponse.Should().NotBeNull();
        paymentResponse.Id.Should().NotBeEmpty();
        paymentResponse.Status.Should().Be(nameof(PaymentStatus.Declined));
        paymentResponse.Amount.Should().Be(paymentRequest.Amount);
        paymentResponse.Currency.Should().Be(paymentRequest.Currency);
        paymentResponse.ExpiryMonth.Should().Be(paymentRequest.ExpiryMonth);
        paymentResponse.ExpiryYear.Should().Be(paymentRequest.ExpiryYear);
        paymentResponse.LastFourCardDigits.Should().Be("8878");
    }
    
    [Fact]
    public async Task Create_Payment_Returns_OK_Declined_Bank_Error()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var paymentRequest = new
        {
            Amount = 10000,
            Currency = "GBP",
            CardNumber = "2222405343248870", //Bank Error - 0 last digit
            ExpiryMonth = date.Month,
            ExpiryYear = date.Year + 1,
            Cvv = "123"
        };
        var requestContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(paymentRequest), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "api/payments")
        {
            Content = requestContent
        };

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var paymentResponse = System.Text.Json.JsonSerializer.Deserialize<PostPaymentResponse>(responseContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        paymentResponse.Should().NotBeNull();
        paymentResponse.Id.Should().NotBeEmpty();
        paymentResponse.Amount.Should().Be(paymentRequest.Amount);
        paymentResponse.Currency.Should().Be(paymentRequest.Currency);
        paymentResponse.ExpiryMonth.Should().Be(paymentRequest.ExpiryMonth);
        paymentResponse.ExpiryYear.Should().Be(paymentRequest.ExpiryYear);
        paymentResponse.Status.Should().Be(nameof(PaymentStatus.Declined));
        paymentResponse.LastFourCardDigits.Should().Be("8870");
    }
    
     
    [Fact]
    public async Task Create_Payment_Returns_BadRequest_ValidationErrors()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var paymentRequest = new
        {
            Amount = 10000,
            Currency = "JPY", //Unsupported currency
            CardNumber = "2222405343248870000000000000", 
            ExpiryMonth = date.Month,
            ExpiryYear = date.Year - 1,
            Cvv = "123456"
        };
        var requestContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(paymentRequest), System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "api/payments")
        {
            Content = requestContent
        };

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var validationResponse = System.Text.Json.JsonSerializer.Deserialize<ValidationResponse>(responseContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        validationResponse.Should().NotBeNull();
        validationResponse.Status.Should().Be(nameof(PaymentStatus.Rejected));
        validationResponse.Errors.Count.Should().Be(4);
    }
}
