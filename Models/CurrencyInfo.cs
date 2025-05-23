using Newtonsoft.Json;

namespace CurrencyTracker.Models;

public class CurrencyInfo : IPrototype<CurrencyInfo>
{
    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("symbol_native")]
    public string SymbolNative { get; set; }

    [JsonProperty("decimal_digits")]
    public int DecimalDigits { get; set; }

    [JsonProperty("rounding")]
    public int Rounding { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("name_plural")]
    public string NamePlural { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    public CurrencyInfo Clone()
    {
        return (CurrencyInfo) MemberwiseClone();
    }
}