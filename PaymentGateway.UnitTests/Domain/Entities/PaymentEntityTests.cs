using PaymentGateway.Domain.Entities;

namespace PaymentGateway.UnitTests.Domain.Entities;

public class PaymentEntityTests
{
    [Fact]
    public void GetLast4CardDigits_ReturnsLast4Digits()
    {
        var payment = new Payment { CardNumber = "1234567890123456" };
        payment.GetLast4CardDigits().Should().Be("3456");
    }
}