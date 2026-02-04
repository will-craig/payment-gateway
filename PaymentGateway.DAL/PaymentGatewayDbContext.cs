using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL;

public class PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options) : DbContext(options), IPaymentGatewayDbContext
{
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>().HasKey(p => p.Id);
        base.OnModelCreating(modelBuilder);
    }
}