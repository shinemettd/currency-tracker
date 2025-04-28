using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CurrencyTracker.Models;
using CurrencyTracker.Repositories;
using CurrencyTracker.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace CurrencyTracker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    private readonly RatesRepository _repository = new();
    private readonly ApiService _apiService = new();
    private readonly RatesService _ratesService;
    private Random _random = new();
    
    public MainWindow()
    {
        InitializeComponent();
        _ratesService = new RatesService(_repository, _apiService);
        LoadCurrencies();
    }
    
    private Brush GetRandomBrush()
    {
        return new SolidColorBrush(Color.FromRgb(
            (byte)_random.Next(50, 256),
            (byte)_random.Next(50, 256),
            (byte)_random.Next(50, 256)));
    }

    private SKColor GetRandomSKColor()
    {
        return new SKColor(
            (byte)_random.Next(50, 256),
            (byte)_random.Next(50, 256),
            (byte)_random.Next(50, 256));
    }

    private async void OnShowChartClick(object sender, RoutedEventArgs e)
    {
        RatesChart.Series = null;
        LegendList.ItemsSource = null;
        var baseCurrency = BaseCurrencyComboBox.SelectedValue?.ToString();
        var selectedCurrencies = TargetCurrencyListBox.SelectedItems;

        if (baseCurrency == null || selectedCurrencies.Count == 0)
        {
            MessageBox.Show("Выберите базовую валюту и хотя бы одну целевую.");
            return;
        }

        var series = new List<ISeries>();
        var legendItems = new List<LegendItem>();

        var startDate = DateTime.Now.AddDays(-7).Date;
        var endDate = DateTime.Now.Date;

        foreach (dynamic item in selectedCurrencies)
        {
            string targetCurrency = item.Key;
            var color = GetRandomSKColor();
            var brush = new SolidColorBrush(Color.FromRgb(color.Red, color.Green, color.Blue));

            var rates = new List<double>();
            var dates = new List<DateTime>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                try
                {
                    var rate = await _ratesService.GetOrFetchRateAsync(baseCurrency, targetCurrency, date);
                    rates.Add((double)rate);
                    dates.Add(date);
                }
                catch
                {
                    // если не нашёл курс — можешь пропустить день или поставить 0
                    rates.Add(0);
                    dates.Add(date);
                }
            }

            series.Add(new LineSeries<double>
            {
                Name = targetCurrency,
                Values = rates,
                Stroke = new SolidColorPaint(color, 2),
                Fill = null
            });

            legendItems.Add(new LegendItem
            {
                CurrencyCode = targetCurrency,
                Color = brush
            });
        }

        RatesChart.Series = series;
        LegendList.ItemsSource = legendItems;
    }
    
    private async void LoadCurrencies()
    {
        var currenciesResponse = await _apiService.GetAvailableCurrenciesAsync();

        if (currenciesResponse?.Data != null)
        {
            BaseCurrencyComboBox.ItemsSource = currenciesResponse.Data;
            TargetCurrencyListBox.ItemsSource = currenciesResponse.Data;
            BaseCurrencyComboBox.SelectedIndex = 0;
        }
        else
        {
            MessageBox.Show("Не удалось загрузить список валют.");
        }
    }

    private List<double> GenerateMockRates()
    {
        var rates = new List<double>();
        double value = _random.Next(50, 150);
        for (int i = 0; i < 8; i++)
        {
            value += _random.NextDouble() * 2 - 1; // немного варьируем
            rates.Add(Math.Round(value, 2));
        }
        return rates;
    }
    
    private async void TestApi()
    {
        var apiService = new ApiService();
        var baseCurrency = "USD";
        var targetCurrency = "EUR";
        var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

        // var history = await apiService.GetHistoricalRateAsync(date, baseCurrency, targetCurrency);
        //
        // if (history?.Data != null && history.Data.ContainsKey(date))
        // {
        //     var rate = history.Data[date][targetCurrency];
        //     MessageBox.Show($"{date}: {targetCurrency} = {rate}");
        // }
        // else
        // {
        //     MessageBox.Show($"Нет данных за {date}.");
        // }
    }
}