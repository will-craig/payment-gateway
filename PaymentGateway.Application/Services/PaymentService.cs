using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Validator;
using PaymentGateway.BankClient;
using PaymentGateway.DAL;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.Services;

public interface IPaymentService
{
    public Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId); 
    public Task<PaymentValidationResultDto> ProcessPaymentAsync(CreatePaymentRequestDto dto);
}

public class PaymentService(IPaymentGatewayDbContext dbContext, IBankClient bankClient, IProcessPaymentValidator validator) : IPaymentService
{
    public async Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId)
    {
        var result = await dbContext.Payments.FindAsync(paymentId);
        if (result == null)
            return null;
        return MapToPaymentResponseDto(result);
    }

    public async Task<PaymentValidationResultDto> ProcessPaymentAsync(CreatePaymentRequestDto dto)
    {
        //Validate Payment
        var validationResult = validator.ValidatePaymentDetails(dto);
        if (!validationResult.IsValid)
            return validationResult;
        
        // Bank payment processing
        var bankResponse = await bankClient.ProcessPaymentAsync(dto.MapToBankClientRequest());
        
        //Update Database
        var payment = dto.MapToEntity();
        payment.Status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
        payment.BankAuthorizationCode = bankResponse.AuthorizationCode;
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync();

        validationResult.Payment = MapToPaymentResponseDto(payment);
        
        return validationResult;
    }

    private static PaymentResponseDto MapToPaymentResponseDto(Payment payment)
    {
        return new PaymentResponseDto
        {
            PaymentId = payment.Id,
            Status = payment.Status,
            LastFourCardDigits = payment.GetLast4CardDigits(),
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
}