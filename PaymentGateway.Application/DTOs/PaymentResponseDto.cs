using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.DTOs;

public record PaymentResponseDto
{
    public Guid PaymentId { get; set; }
    public PaymentStatus Status { get; set; }
    public string LastFourCardDigits { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Amount { get; set; }
}
