using System.Text.Json.Serialization;

namespace PaymentGateway.BankClient.Models;

public record ProcessPaymentRequest
{
    public ProcessPaymentRequest(string CardNumber,
        string Expiry,
        string Currency,
        decimal Amount,
        string Cvv)
    {
        this.CardNumber = CardNumber;
        this.Expiry = Expiry;
        this.Currency = Currency;
        this.Amount = Amount;
        this.Cvv = Cvv;
    }

    [JsonPropertyName("card_number")]
    public string CardNumber { get; }
    [JsonPropertyName("expiry_date")]
    public string Expiry { get; }
    [JsonPropertyName("currency")]
    public string Currency { get; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; }
    [JsonPropertyName("cvv")]
    public string Cvv { get; }
}