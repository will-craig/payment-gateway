using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Validator;
using PaymentGateway.Domain.Providers;

namespace PaymentGateway.UnitTests.Application.Validator;

public class ProcessPaymentValidatorTests
{
    private readonly Mock<ITimeProvider> _timeProvider;
    private readonly ProcessPaymentValidator _service;

    public ProcessPaymentValidatorTests()
    {
        _timeProvider = new Mock<ITimeProvider>();
        _service = new ProcessPaymentValidator(_timeProvider.Object);
    }
    
    [Fact]
    public void ValidatePaymentDetails_ReturnsInvalid()
    {
        //arrange
        _timeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2026, 12, 31));
        var paymentDto = new CreatePaymentRequestDto(
            "11111111",
            13,
            2000,
            "AMD",
            100,
            "12345"
        );
        
        //act
        var result = _service.ValidatePaymentDetails(paymentDto);
        
        //assert
        // Should fail all validations
        result.IsValid.Should().BeFalse();
        result.Payment.Should().BeNull();
        result.ErrorMessages.Count.Should().Be(4);
        result.ErrorMessages.Should().Contain(e => e.StartsWith("CardNumber"));
        result.ErrorMessages.Should().Contain(e => e.StartsWith("CVV"));
        result.ErrorMessages.Should().Contain(e => e.StartsWith("ExpiryDate"));
        result.ErrorMessages.Should().Contain(e => e.StartsWith("Currency"));
    }

    [Fact]
    public void ValidatePaymentDetails_ReturnsValid()
    {
        //arrange
        _timeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2026, 12, 31));
        var paymentDto = new CreatePaymentRequestDto(
            "123456789012345",
            12,
            27,
            "USD",
            100,
            "123"
        );
        
        //act
        var result = _service.ValidatePaymentDetails(paymentDto);
        
        //assert
        result.IsValid.Should().BeTrue();
        result.Payment.Should().BeNull();
        result.ErrorMessages.Count.Should().Be(0);
    }
}