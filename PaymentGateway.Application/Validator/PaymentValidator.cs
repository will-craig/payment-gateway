using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Providers;
using PaymentGateway.Domain.ValidationRules;

namespace PaymentGateway.Application.Validator;

public interface IProcessPaymentValidator
{
    public PaymentValidationResultDto ValidatePaymentDetails(CreatePaymentRequestDto paymentRequestDto);
}

public class ProcessPaymentValidator(ITimeProvider dateTime) : IProcessPaymentValidator
{
    public PaymentValidationResultDto ValidatePaymentDetails(CreatePaymentRequestDto paymentRequestDto)
    {
        var validationResult = new PaymentValidationResultDto { IsValid = true };

        if (CardNumberValidation.IsValid(paymentRequestDto.CardNumber) == false)
        {
            validationResult.IsValid = false;
            validationResult.ErrorMessages.Add("CardNumber: Is invalid.");
        }
        if (CvvValidation.IsValid(paymentRequestDto.CVV) == false)
        {
            validationResult.IsValid = false;
            validationResult.ErrorMessages.Add("CVV: Is invalid.");
        }
        if (ExpiryDateValidation.IsValid(paymentRequestDto.ExpiryMonth, paymentRequestDto.ExpiryYear, dateTime) == false)
        {
            validationResult.IsValid = false;
            validationResult.ErrorMessages.Add("ExpiryDate: Is invalid or card is expired.");
        }

        if (CurrencyValidation.IsValid(paymentRequestDto.Currency) == false)
        {
            validationResult.IsValid = false;
            validationResult.ErrorMessages.Add("Currency: Is not supported.");
        }

        return validationResult;
    }
}