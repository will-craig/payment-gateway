using PaymentGateway.Domain.Helper;
using PaymentGateway.Domain.Providers;

namespace PaymentGateway.Domain.ValidationRules;

public static class ExpiryDateValidation
{
    public static bool IsValid(int month, int year, ITimeProvider dateTime)
    {
        if (month < 1 || month > 12)
            return false;

        if (year < 0)
            return false;

        var expiryDate = DateHelper.ExtractDateTime(month, year, dateTime.UtcNow.Year);

        // expiry date should go to the end of the month
        if(expiryDate < dateTime.UtcNow)
            return false;

        return true;
    }
}