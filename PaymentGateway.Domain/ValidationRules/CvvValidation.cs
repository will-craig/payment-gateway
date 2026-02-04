namespace PaymentGateway.Domain.ValidationRules;

public class CvvValidation 
{
    public static bool IsValid(string cvv)
    {
        return cvv.All(char.IsDigit) && cvv.Length >= 3 && cvv.Length <= 4;
    }
}