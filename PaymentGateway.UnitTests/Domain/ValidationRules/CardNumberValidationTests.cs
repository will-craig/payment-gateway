using PaymentGateway.Domain.ValidationRules;

namespace PaymentGateway.UnitTests.Domain.ValidationRules;

public class CardNumberValidationTests
{
    [Theory]
    [InlineData("12345678901234", true)] //14 chars
    [InlineData("1234567890123456789", true)] //19 chars
    [InlineData("1234", false)] //too short
    [InlineData("12345678901234567890", false)] //too long
    [InlineData("abc1234567890", false)]
    public void IsValid_Works(string card, bool expected)
    {
        CardNumberValidation.IsValid(card).Should().Be(expected);
    }
}