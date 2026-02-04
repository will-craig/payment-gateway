using System.Net.Http.Json;
using PaymentGateway.BankClient.Exceptions;
using PaymentGateway.BankClient.Models;

namespace PaymentGateway.BankClient;

public class AcquiringBankClient(HttpClient httpClient) : IBankClient
{
    private const string ProcessPaymentEndpoint = "payments";

    public async Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest paymentRequest)
    {
        ProcessPaymentResponse? paymentResponse;
        try
        {
            var payload = System.Text.Json.JsonSerializer.Serialize(paymentRequest);
            var response = await httpClient.PostAsync(ProcessPaymentEndpoint, new StringContent(payload));
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine("Received Response from bank: {0}", responseContent);

            response.EnsureSuccessStatusCode();
            paymentResponse = System.Text.Json.JsonSerializer.Deserialize<ProcessPaymentResponse>(responseContent);
            
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("An unexpected error occurred while processing the payment: {0}", ex.Message);
            paymentResponse = new ProcessPaymentResponse
            {
                Authorized = false,
                AuthorizationCode = string.Empty
            };
        }

        return paymentResponse ?? throw new BankPaymentException("Failed to process payment with the acquiring bank.");
    }
}