namespace PaymentGateway.Domain.ReferenceData;

public static class Currencies
{
    public static readonly HashSet<string> SupportedCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        "GBP",
        "EUR",
        "USD"
    };
}