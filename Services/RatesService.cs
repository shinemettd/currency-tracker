using CurrencyTracker.Models;
using CurrencyTracker.Repositories;

namespace CurrencyTracker.Services;

public class RatesService
{
    private readonly IRatesRepository _repository;
    private readonly ApiService _apiService;

    public RatesService(IRatesRepository repository, ApiService apiService)
    {
        _repository = repository;
        _apiService = apiService;
    }

    public async Task<decimal> GetOrFetchRateAsync(string baseCurrency, string targetCurrency, DateTime date)
    {
        var cachedRate = await _repository.GetRateAsync(baseCurrency, targetCurrency, date);
        if (cachedRate != null)
            return cachedRate.Rate;

        var rate = await _apiService.GetHistoricalRateAsync(date.ToString("yyyy-MM-dd"), baseCurrency, targetCurrency);
        if (rate?.Data != null && rate.Data.ContainsKey(date.ToString("yyyy-MM-dd")))
        {
            var value = rate.Data[date.ToString("yyyy-MM-dd")][targetCurrency];
            await _repository.SaveRateAsync(new RatesHistory
            {
                BaseCurrency = baseCurrency,
                TargetCurrency = targetCurrency,
                Date = date,
                Rate = value
            });
            return value;
        }

        throw new Exception("Rate not found from API.");
    }
}