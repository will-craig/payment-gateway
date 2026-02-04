using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Services;
using PaymentGateway.Application.Validator;
using PaymentGateway.BankClient;
using PaymentGateway.BankClient.Models;
using PaymentGateway.DAL;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.UnitTests.Application.Services;

public class PaymentServiceTests
{
    private readonly Mock<IProcessPaymentValidator> _validator;
    private readonly Mock<IPaymentGatewayDbContext> _dbContext;
    private readonly Mock<IBankClient> _bankClient;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _validator = new Mock<IProcessPaymentValidator>();
        _dbContext = new Mock<IPaymentGatewayDbContext>();
        _bankClient = new Mock<IBankClient>();
        _service = new PaymentService(_dbContext.Object, _bankClient.Object, _validator.Object);
        
        _dbContext.Setup(db => db.Payments).Returns(Mock.Of<DbSet<Payment>>());
    }
    #region GetPaymentByIdAsync Tests
    [Fact]
    public async Task GetPaymentByIdAsync_ReturnsNull_WhenNotFound()
    {
        //arrange
        _dbContext.Setup(db => db.Payments.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Payment?)null);
        //act
        var result = await _service.GetPaymentByIdAsync(Guid.NewGuid());
        //assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetPaymentByIdAsync_returns_PaymentDto()
    {
        //arrange
        var queryId = Guid.NewGuid();
        var cardNumber = "1234567812345678";
        var expiryMonth = 12;
        var expiryYear = 2025;
        var currency = "USD";
        var amount = 1000;
        var status = PaymentStatus.Authorized;
        
        _dbContext.Setup(db => db.Payments.FindAsync(queryId))
            .ReturnsAsync(new Payment
            {
                Id = queryId,
                CardNumber = cardNumber,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                Currency = currency,
                Amount = amount,
                Status = status
            });
        
        //act
        var result = await _service.GetPaymentByIdAsync(queryId);
        
        //assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PaymentResponseDto>();
        result.PaymentId.Should().Be(queryId);
        result.LastFourCardDigits.Should().Be("5678");
        result.ExpiryMonth.Should().Be(expiryMonth);
        result.ExpiryYear.Should().Be(expiryYear);
        result.Currency.Should().Be(currency);
        result.Amount.Should().Be(amount);
        result.Status.Should().Be(status);
    }
    #endregion
    #region ProcessPaymentAsync Tests
    [Fact]
    public async Task ProcessPaymentAsync_ReturnsInvalid()
    {
        //arrange
        var createPaymentDto = new CreatePaymentRequestDto(
            "1234567812345678",
            12,
            2025,
            "USD",
            1000,
            "123"
        );
        var errorList = new List<string>
        {
            "CardNumber: Card number is invalid.",
            "ExpiryDate: Expiry date is invalid."
        };
        _validator.Setup(v => v.ValidatePaymentDetails(createPaymentDto))
            .Returns(new PaymentValidationResultDto { IsValid = false, ErrorMessages = errorList });
        
        //act
        var result = await _service.ProcessPaymentAsync(createPaymentDto);
        
        //assert
        result.IsValid.Should().BeFalse();
        result.Payment.Should().BeNull();
        result.ErrorMessages.Should().BeEquivalentTo(errorList);
        
        _validator.Verify(v => v.ValidatePaymentDetails(createPaymentDto), Times.Once);
        
        //validate database and bank client were not called
        _bankClient.Verify(b => b.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Never);
        _dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(true, PaymentStatus.Authorized)]
    [InlineData(false, PaymentStatus.Declined)]
    public async Task ProcessPaymentAsync_ProcessesPaymentSuccessfully(bool bankAuthorized, PaymentStatus expectedStatus)
    {
        //arrange
        var createPaymentDto = new CreatePaymentRequestDto(
            "1234567812345678",
            12,
            2025,
            "USD",
            1000,
            "123");
        
        _validator.Setup(v => v.ValidatePaymentDetails(createPaymentDto))
            .Returns(new PaymentValidationResultDto { IsValid = true });
        
        _bankClient.Setup(b => b.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()))
            .ReturnsAsync(new ProcessPaymentResponse
            {
                    Authorized = bankAuthorized,
                    AuthorizationCode = "TransactionId123"
            });
        
        _dbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        //act
        var result = await _service.ProcessPaymentAsync(createPaymentDto);
        
        //assert
        result.IsValid.Should().BeTrue();
        result.Payment.Should().NotBeNull();
        result.Payment.Status.Should().Be(expectedStatus);
        
        //verify call to all dependencies
        _validator.Verify(v => v.ValidatePaymentDetails(createPaymentDto), Times.Once);
        _bankClient.Verify(b => b.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Once);
        _dbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion
}