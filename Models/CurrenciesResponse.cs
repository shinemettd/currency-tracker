using Newtonsoft.Json;

namespace CurrencyTracker.Models;

public class CurrenciesResponse
{
    [JsonProperty("data")]
    public Dictionary<string, CurrencyInfo> Data { get; set; }
}