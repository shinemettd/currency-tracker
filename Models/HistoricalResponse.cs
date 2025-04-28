using Newtonsoft.Json;

namespace CurrencyTracker.Models;

public class HistoricalResponse
{
    [JsonProperty("data")]
    public Dictionary<string, Dictionary<string, decimal>> Data { get; set; }
}