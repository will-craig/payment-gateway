using PaymentGateway.Domain.ValidationRules;

namespace PaymentGateway.UnitTests.Domain.ValidationRules;

public class CvvValidationTests
{
    [Theory]
    [InlineData("123", true)]
    [InlineData("1234", true)]
    [InlineData("12", false)]
    [InlineData("abcd", false)]
    public void IsValid_Works(string cvv, bool expected)
    {
        CvvValidation.IsValid(cvv).Should().Be(expected);
    }
}