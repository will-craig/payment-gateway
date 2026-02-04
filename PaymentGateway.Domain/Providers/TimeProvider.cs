namespace PaymentGateway.Domain.Providers;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}

public class TimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    
}