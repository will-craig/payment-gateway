namespace PaymentGateway.Domain.ValidationRules;

public static class CardNumberValidation
{
    public static bool IsValid(string cardNumber)
    {
        return cardNumber.All(char.IsDigit) && cardNumber.Length >= 14 && cardNumber.Length <= 19;
    }
}