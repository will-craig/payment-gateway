using PaymentGateway.Application.DTOs;
using PaymentGateway.Models.Requests;

namespace PaymentGateway.Mappers.Requests;

public static class PostPaymentRequestMapper
{
    public static CreatePaymentRequestDto MapToDto(this PostPaymentRequest request)
    {
        return new CreatePaymentRequestDto(CardNumber: request.CardNumber, 
            ExpiryMonth: request.ExpiryMonth,
            ExpiryYear: request.ExpiryYear, 
            Currency: request.Currency, 
            Amount: request.Amount, 
            CVV: request.CVV);
    }
}