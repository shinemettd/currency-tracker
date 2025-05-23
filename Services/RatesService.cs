using CurrencyTracker.Models;
using CurrencyTracker.Repositories;

public class RatesService
{
    private readonly IRatesRepository _repository;

    public RatesService(IRatesRepository repository)
    {
        _repository = repository;
    }

    public Task<RatesHistory?> GetRateAsync(string baseCurrency, string targetCurrency, DateTime date)
        => _repository.GetRateAsync(baseCurrency, targetCurrency, date);

    public Task SaveRateAsync(RatesHistory rate)
        => _repository.SaveRateAsync(rate);
}