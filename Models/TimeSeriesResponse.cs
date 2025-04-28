using Newtonsoft.Json;

namespace CurrencyTracker.Models;

public class TimeSeriesResponse
{
    [JsonProperty("rates")]
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
}