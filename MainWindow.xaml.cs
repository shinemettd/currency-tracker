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
    private readonly RatesFacade _ratesFacade;
    private Random _random = new();
    private Dictionary<string, CurrencyInfo> _allCurrencies;
    
    public MainWindow()
    {
        InitializeComponent();
        StartDatePicker.SelectedDate = DateTime.Today.AddDays(-7);
        EndDatePicker.SelectedDate = DateTime.Today.AddDays(-1);

        StartDatePicker.DisplayDateEnd = DateTime.Today.AddDays(-1);
        EndDatePicker.DisplayDateEnd = DateTime.Today.AddDays(-1);
        _ratesFacade = new RatesFacade(new RatesService(_repository), _apiService);
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

        var startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddDays(-7);
        var endDate = EndDatePicker.SelectedDate ?? DateTime.Now;
        
        var dates = new List<DateTime>();
        
        foreach (dynamic item in selectedCurrencies)
        {
            string targetCurrency = item.Key;
            var color = GetRandomSKColor();
            var brush = new SolidColorBrush(Color.FromRgb(color.Red, color.Green, color.Blue));

            var rates = new List<double>();

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                try
                {
                    var rate = await _ratesFacade.GetOrFetchRateAsync(baseCurrency, targetCurrency, date);
                    rates.Add((double)rate);
                    dates.Add(date);
                }
                catch
                {
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
        
        RatesChart.XAxes = new List<Axis>
        {
            new Axis
            {
                Labels = dates.Select(d => d.ToString("dd.MM")).ToList()
            }
        };

        RatesChart.Series = series;
        LegendList.ItemsSource = legendItems;
    }
    
    private async void LoadCurrencies()
    {
        var currenciesResponse = await _apiService.GetAvailableCurrenciesAsync();

        if (currenciesResponse?.Data != null)
        {
            _allCurrencies = currenciesResponse.Data;

            BaseCurrencyComboBox.ItemsSource = _allCurrencies;
            BaseCurrencyComboBox.SelectedIndex = 0;

            UpdateTargetCurrencyList();
        }
        else
        {
            MessageBox.Show("Не удалось загрузить список валют.");
        }
    }

    private void UpdateTargetCurrencyList()
    {
        var baseKey = BaseCurrencyComboBox.SelectedValue?.ToString();

        if (_allCurrencies == null || baseKey == null)
            return;

        TargetCurrencyListBox.ItemsSource = _allCurrencies
            .Where(c => c.Key != baseKey)
            .ToList();
    }
    
    private void BaseCurrencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateTargetCurrencyList();
    }
    
}