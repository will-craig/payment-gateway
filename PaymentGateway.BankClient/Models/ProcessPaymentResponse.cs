using System.Text.Json.Serialization;

namespace PaymentGateway.BankClient.Models;

public record ProcessPaymentResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
};