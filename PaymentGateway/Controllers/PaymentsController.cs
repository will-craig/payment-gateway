using PaymentGateway.Models.Requests;
using PaymentGateway.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Mappers.Requests;
using PaymentGateway.Mappers.Responses;

namespace PaymentGateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService) : Controller
{

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPaymentResponse>> GetPaymentAsync(Guid id)
    {
        var payment = await paymentService.GetPaymentByIdAsync(id);
        if(payment == null)
            return NotFound();
        
        return Ok(payment.MapToGetPaymentResponse());
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync(PostPaymentRequest request)
    {
        var createdPayment = await paymentService.ProcessPaymentAsync(request.MapToDto());

        if (!createdPayment.IsValid)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = createdPayment.ErrorMessages,
                Status = nameof(PaymentStatus.Rejected)
            };
            return BadRequest(validationResponse);
        }
        
        return Ok(createdPayment.Payment.MapToPostPaymentResponse());
    }
}