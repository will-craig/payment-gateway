using System.Net;
using Moq.Protected;
using PaymentGateway.BankClient;
using PaymentGateway.BankClient.Models;

namespace PaymentGateway.UnitTests.BankClient;

public class AcquiringBankClientTests
{
    [Fact]
    public async Task ProcessPaymentAsync_ReturnsResponse_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new ProcessPaymentResponse
        {
            Authorized = true,
            AuthorizationCode = "AUTH123"
        };
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expectedResponse))
        };
        var httpClient = CreateMockedHttpClient(httpResponse);
        var client = new AcquiringBankClient(httpClient);
        var request = new ProcessPaymentRequest("1234567890123456", "12/26", "USD", 1000, "123");

        // Act
        var result = await client.ProcessPaymentAsync(request);

        // Assert
        result.Authorized.Should().BeTrue();
        result.AuthorizationCode.Should().Be(expectedResponse.AuthorizationCode);
    }
    
    [Fact]
    public async Task ProcessPaymentAsync_ReturnsDefaultResponse_Declined_On5xx_Response()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.ServiceUnavailable,
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize("{}"))
        };

        var httpClient = CreateMockedHttpClient(httpResponse);
        
        var client = new AcquiringBankClient(httpClient);
        var request = new ProcessPaymentRequest("1234567890123456", "12/26", "USD", 1000, "123");

        // Act
        var result = await client.ProcessPaymentAsync(request);

        // Assert
        result.Authorized.Should().BeFalse();
        result.AuthorizationCode.Should().BeEmpty();
    }
    
    //Same scenario as above, but if the handler throws an exception rather than a safe 503 http response
    [Fact]
    public async Task ProcessPaymentAsync_ReturnsDefaultResponse_OnException()
    {
        // Arrange
        var httpRequestError = new HttpRequestException(HttpRequestError.HttpProtocolError, "Simulated network error",
            null, HttpStatusCode.ServiceUnavailable);
        var httpClient = CreateMockedHttpClient(null, httpRequestError);
        var client = new AcquiringBankClient(httpClient);
        var request = new ProcessPaymentRequest("1234567890123456", "12/26", "USD", 1000, "123");

        // Act
        var result = await client.ProcessPaymentAsync(request);

        // Assert
        result.Authorized.Should().BeFalse();
        result.AuthorizationCode.Should().BeEmpty();
    }
    
    private static HttpClient CreateMockedHttpClient(HttpResponseMessage responseMessage, Exception? exception = null)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        if (exception == null)
        {
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);
        }
        else
        {
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(exception);
        }
        var client = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://mocked-bank/")
        };
        return client;
    }

}
