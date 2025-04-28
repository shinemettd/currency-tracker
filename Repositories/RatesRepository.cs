using CurrencyTracker.Models;
using CurrencyTracker.Services;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Repositories;

public class RatesRepository : IRatesRepository
{
    public async Task<RatesHistory?> GetRateAsync(string baseCurrency, string targetCurrency, DateTime date)
    {
        using var db = new CurrencyDbContext();
        return await db.RatesHistories
            .FirstOrDefaultAsync(r =>
                r.BaseCurrency == baseCurrency &&
                r.TargetCurrency == targetCurrency &&
                r.Date == date.Date);
    }

    public async Task SaveRateAsync(RatesHistory rate)
    {
        using var db = new CurrencyDbContext();
        db.RatesHistories.Add(rate);
        await db.SaveChangesAsync();
    }

    public async Task<List<RatesHistory>> GetRatesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
    {
        using var db = new CurrencyDbContext();
        return await db.RatesHistories
            .Where(r => r.BaseCurrency == baseCurrency
                        && r.TargetCurrency == targetCurrency
                        && r.Date >= startDate.Date
                        && r.Date <= endDate.Date)
            .OrderBy(r => r.Date)
            .ToListAsync();
    }
}