namespace PaymentGateway.Models.Responses;

public class ValidationResponse
{
    public IList<string> Errors { get; set; }
    public string Status { get; set; }
}