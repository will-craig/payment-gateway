using PaymentGateway.Domain.Providers;
using PaymentGateway.Domain.ValidationRules;

namespace PaymentGateway.UnitTests.Domain.ValidationRules;

public class ExpiryDateValidationTests
{
    private readonly Mock<ITimeProvider> _mockTimeProvider;

    public ExpiryDateValidationTests()
    {
        
        _mockTimeProvider = new Mock<ITimeProvider>();
        _mockTimeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2026, 12, 31));
    }
    
    [Theory]
    [InlineData(12, 2026, true)] // valid until end of month
    [InlineData(12, 26, true)] // valid until end of month
    [InlineData(1, 2027, true)] // valid - next year
    [InlineData(11, 26, false)] // invalid past month
    [InlineData(6, -2000, false)] // invalid negative year
    [InlineData(0, 2027, false)] // invalid month out of range
    [InlineData(13, 2027, false)] // invalid month out of range
    public void IsValid_ReturnsTrue_WhenNotExpired(int month, int year, bool expected)
    {
        ExpiryDateValidation.IsValid(month, year, _mockTimeProvider.Object).Should().Be(expected);
    }
}