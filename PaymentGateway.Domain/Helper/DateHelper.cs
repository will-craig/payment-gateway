namespace PaymentGateway.Domain.Helper;

public static class DateHelper
{
    public static DateTime ExtractDateTime(int month, int year, int currentYear)
    {
        // expiry date goes to the end of the month
        return new DateTime(NormalizeYear(year, currentYear), month, 1)
            .AddMonths(1)
            .AddTicks(-1);
    }
    
    
    private static int NormalizeYear(int year, int currentYear)
    {
        // 4 digit year
        if (year >= 1000)
            return year;
        // 2 digit year
       return (currentYear / 100) * 100 + year;
    }
}