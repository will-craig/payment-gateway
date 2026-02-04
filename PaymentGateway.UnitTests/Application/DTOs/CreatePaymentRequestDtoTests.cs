using PaymentGateway.Application.DTOs;
namespace PaymentGateway.UnitTests.Application.DTOs;

public class CreatePaymentRequestDtoTests
{
    [Fact]
    public void MapToEntity_MapsAllProperties()
    {
        var dto = new CreatePaymentRequestDto("1234567890123456", 12, 2026, "USD", 1000, "123");
        var entity = dto.MapToEntity();
        entity.CardNumber.Should().Be(dto.CardNumber);
        entity.ExpiryMonth.Should().Be(dto.ExpiryMonth);
        entity.ExpiryYear.Should().Be(dto.ExpiryYear);
        entity.Currency.Should().Be(dto.Currency);
        entity.Amount.Should().Be(dto.Amount);
        entity.Cvv.Should().Be(dto.CVV);
        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(2026)] //4digit year
    [InlineData(26)] //2digit year
    public void MapToBankClientRequest_MapsAllProperties(int year)
    {
        var dto = new CreatePaymentRequestDto("1234567890123456", 12, year, "USD", 1000, "123");
        var bankRequest = dto.MapToBankClientRequest();
        bankRequest.CardNumber.Should().Be(dto.CardNumber);
        bankRequest.Expiry.Should().Be("12/26");
        bankRequest.Currency.Should().Be(dto.Currency);
        bankRequest.Amount.Should().Be(dto.Amount);
        bankRequest.Cvv.Should().Be(dto.CVV);
    }
    
}