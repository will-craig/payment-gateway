using PaymentGateway.Domain.ReferenceData;

namespace PaymentGateway.Domain.ValidationRules;

public static class CurrencyValidation
{
    public static bool IsValid(string currency) => Currencies.SupportedCurrencies.Contains(currency);
}