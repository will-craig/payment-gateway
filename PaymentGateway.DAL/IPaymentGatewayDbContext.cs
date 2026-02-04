using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL;


public interface IPaymentGatewayDbContext
{
    DbSet<Payment> Payments { get; set; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}