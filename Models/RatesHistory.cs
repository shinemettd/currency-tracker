using System.ComponentModel.DataAnnotations;

namespace CurrencyTracker.Models;

public class RatesHistory
{
    [Key]
    public int Id { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string TargetCurrency { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Rate { get; set; }
}