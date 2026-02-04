using PaymentGateway.Domain.ValidationRules;

namespace PaymentGateway.UnitTests.Domain.ValidationRules;

public class CurrencyValidationTests
{
    [Theory]
    [InlineData("USD", true)]
    [InlineData("GBP", true)]
    [InlineData("EUR", true)]
    [InlineData("JPY", false)]
    [InlineData("", false)]
    public void IsValid_Works(string currency, bool expected)
    {
        CurrencyValidation.IsValid(currency).Should().Be(expected);
    }
}