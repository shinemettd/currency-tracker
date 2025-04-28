using CurrencyTracker.Models;

namespace CurrencyTracker.Repositories;

public interface IRatesRepository
{
    Task<RatesHistory?> GetRateAsync(string baseCurrency, string targetCurrency, DateTime date);
    Task SaveRateAsync(RatesHistory rate);
    Task<List<RatesHistory>> GetRatesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate);
}