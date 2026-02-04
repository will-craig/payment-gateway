using PaymentGateway.BankClient.Models;

namespace PaymentGateway.BankClient;

public interface IBankClient
{
    public Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest paymentRequest);
}