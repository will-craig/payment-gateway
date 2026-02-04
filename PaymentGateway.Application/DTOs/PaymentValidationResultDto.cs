namespace PaymentGateway.Application.DTOs;

public record PaymentValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public PaymentResponseDto Payment { get; set; }
}