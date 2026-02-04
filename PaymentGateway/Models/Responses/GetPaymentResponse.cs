namespace PaymentGateway.Models.Responses;

public record GetPaymentResponse{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string LastFourCardDigits { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}