using PaymentGateway.BankClient.Models;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.DTOs;

public record CreatePaymentRequestDto(
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string CVV)
{
    public Payment MapToEntity()
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            CardNumber = CardNumber,
            ExpiryMonth = ExpiryMonth,
            ExpiryYear = ExpiryYear,
            Currency = Currency,
            Amount = Amount,
            Cvv = CVV
        };
    }
    
    public ProcessPaymentRequest MapToBankClientRequest()
    {
        return new ProcessPaymentRequest(
            CardNumber,
            $"{ExpiryMonth:D2}/{ExpiryYear % 100:D2}",
            Currency,
            Amount,
            CVV);
    }
}