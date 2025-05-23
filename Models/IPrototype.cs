namespace CurrencyTracker.Models;

public interface IPrototype<T>
{
    T Clone();
}