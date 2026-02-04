using PaymentGateway.Application.DTOs;
using PaymentGateway.Models.Responses;

namespace PaymentGateway.Mappers.Responses;

public static class PostPaymentResponseMapper
{
    public static PostPaymentResponse MapToPostPaymentResponse(this PaymentResponseDto payment)
    {
        var response = new PostPaymentResponse
        {
            Id = payment.PaymentId,
            Status = payment.Status.ToString(),
            LastFourCardDigits = payment.LastFourCardDigits,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Amount = payment.Amount,
            Currency = payment.Currency
        };
        return response;
    }
}