using PaymentGateway.Domain.Helper;

namespace PaymentGateway.UnitTests.Domain.Helper;

public class DateHelperTests
{

    [Theory]
    [InlineData(2, 24, 2026, "2024-2-29")] //leap year
    [InlineData(2, 26, 2026, "2026-2-28")]
    [InlineData(1, 2000, 2026, "2000-1-31")]
    [InlineData(1,  1999, 2026, "1999-1-31")]
    [InlineData(1,  199, 2026, "2199-1-31")]
    public void CorrectlyExtractsDates(int month, int year, int actualYear, string expected)
    {
        DateHelper.ExtractDateTime(month, year, actualYear).Date.Should().Be(DateTime.Parse(expected));
    }
}
