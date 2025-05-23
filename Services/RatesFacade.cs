using CurrencyTracker.Models;

namespace CurrencyTracker.Services;

public class RatesFacade
{
    private readonly RatesService _ratesService;
    private readonly ApiService _apiService;

    public RatesFacade(RatesService ratesService, ApiService apiService)
    {
        _ratesService = ratesService;
        _apiService = apiService;
    }

    public async Task<decimal> GetOrFetchRateAsync(string baseCurrency, string targetCurrency, DateTime date)
    {
        var cached = await _ratesService.GetRateAsync(baseCurrency, targetCurrency, date);
        if (cached != null)
            return cached.Rate;

        var fetchedRate = await _apiService.GetHistoricalRateAsync(
            date.ToString("yyyy-MM-dd"), 
            baseCurrency, 
            targetCurrency
        );
        if (fetchedRate != null)
        {
            var value = fetchedRate.Data[date.ToString("yyyy-MM-dd")][targetCurrency];
            await _ratesService.SaveRateAsync(new RatesHistory
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