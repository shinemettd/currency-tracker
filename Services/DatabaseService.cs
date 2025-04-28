using CurrencyTracker.Models;

namespace CurrencyTracker.Services;

public class DatabaseService
{
    public async Task SaveRateAsync(string baseCurrency, string targetCurrency, DateTime date, decimal rate)
    {
        using var db = new CurrencyDbContext();
        var exists = db.RatesHistories.Any(r =>
            r.BaseCurrency == baseCurrency &&
            r.TargetCurrency == targetCurrency &&
            r.Date == date);

        if (!exists)
        {
            db.RatesHistories.Add(new RatesHistory
            {
                BaseCurrency = baseCurrency,
                TargetCurrency = targetCurrency,
                Date = date,
                Rate = rate
            });
            await db.SaveChangesAsync();
        }
    }

    public List<RatesHistory> GetRates(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
    {
        using var db = new CurrencyDbContext();
        return db.RatesHistories
            .Where(r => r.BaseCurrency == baseCurrency
                        && r.TargetCurrency == targetCurrency
                        && r.Date >= startDate
                        && r.Date <= endDate)
            .OrderBy(r => r.Date)
            .ToList();
    }
}