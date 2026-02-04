using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Models.Requests;

public record PostPaymentRequest(
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string CVV
);