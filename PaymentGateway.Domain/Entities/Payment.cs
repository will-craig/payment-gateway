using System.ComponentModel.DataAnnotations;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Entities;

public class Payment
{
    [Key]
    public Guid Id { get; set; }
    public string CardNumber { get; set; } 
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Cvv { get; set; } 
    public int Amount { get; set; }
    public string Currency { get; set; } 
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public PaymentStatus Status { get; set; }
    public string BankAuthorizationCode { get; set; }
    
    public string GetLast4CardDigits() => CardNumber[^4..];
}