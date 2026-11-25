using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly StockMovementRepository _movementRepository = new();

    private int _totalProducts;
    private int _lowStockProducts;
    private int _outOfStockProducts;
    private int _totalMovementsToday;
    private int _entriesToday;
    private int _exitsToday;
    private decimal _totalInventoryValue;
    private List<Product> _lowStockItems = [];
    private List<StockMovement> _recentMovements = [];
    private bool _isLoading;

    public int TotalProducts { get => _totalProducts; set => SetProperty(ref _totalProducts, value); }
    public int LowStockProducts { get => _lowStockProducts; set => SetProperty(ref _lowStockProducts, value); }
    public int OutOfStockProducts { get => _outOfStockProducts; set => SetProperty(ref _outOfStockProducts, value); }
    public int TotalMovementsToday { get => _totalMovementsToday; set => SetProperty(ref _totalMovementsToday, value); }
    public int EntriesToday { get => _entriesToday; set => SetProperty(ref _entriesToday, value); }
    public int ExitsToday { get => _exitsToday; set => SetProperty(ref _exitsToday, value); }
    public decimal TotalInventoryValue { get => _totalInventoryValue; set => SetProperty(ref _totalInventoryValue, value); }
    public List<Product> LowStockItems { get => _lowStockItems; set => SetProperty(ref _lowStockItems, value); }
    public List<StockMovement> RecentMovements { get => _recentMovements; set => SetProperty(ref _recentMovements, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public ICommand RefreshCommand { get; }
    public ICommand NavigateToProductsCommand { get; }
    public ICommand NavigateToScannerCommand { get; }

    public DashboardViewModel()
    {
        RefreshCommand = new RelayCommand(LoadData);
        NavigateToProductsCommand = new RelayCommand(() => Services.NavigationService.Instance.NavigateTo("Products"));
        NavigateToScannerCommand = new RelayCommand(() => Services.NavigationService.Instance.NavigateTo("Scanner"));
        LoadData();
    }

    private void LoadData()
    {
        IsLoading = true;
        try
        {
            var summary = _movementRepository.GetDashboardSummary();
            TotalProducts = summary.TotalProducts;
            LowStockProducts = summary.LowStockProducts;
            OutOfStockProducts = summary.OutOfStockProducts;
            TotalMovementsToday = summary.TotalMovementsToday;
            EntriesToday = summary.EntriesToday;
            ExitsToday = summary.ExitsToday;
            TotalInventoryValue = summary.TotalInventoryValue;
            LowStockItems = summary.LowStockItems;
            RecentMovements = summary.RecentMovements;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Error al cargar datos del dashboard:\n{ex.Message}",
                "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
