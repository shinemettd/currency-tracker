using System.Net.Http;
using CurrencyTracker.Models;
using Newtonsoft.Json;

namespace CurrencyTracker.Services;

public class ApiService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiKey = "fca_live_FDcFTUDK5ZsWbeqlClxxXetr4YUADxxbGEqxWYKp";

    public async Task<CurrenciesResponse?> GetAvailableCurrenciesAsync()
    {
        string url = $"https://api.freecurrencyapi.com/v1/currencies?apikey={ApiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CurrenciesResponse>(content);
    }

    public async Task<HistoricalResponse?> GetHistoricalRateAsync(string date, string baseCurrency, string targetCurrencies)
    {
        string url = $"https://api.freecurrencyapi.com/v1/historical?apikey={ApiKey}&date={date}&base_currency={baseCurrency}&currencies={targetCurrencies}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<HistoricalResponse>(content);
    }
}