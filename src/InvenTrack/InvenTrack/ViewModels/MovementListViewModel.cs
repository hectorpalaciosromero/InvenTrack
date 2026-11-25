using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class MovementListViewModel : ViewModelBase
{
    private readonly StockMovementRepository _movementRepository = new();

    private ObservableCollection<StockMovement> _movements = [];
    private DateTime _fromDate = DateTime.Today.AddDays(-30);
    private DateTime _toDate = DateTime.Today;
    private MovementType? _selectedType;
    private bool _isLoading;

    public ObservableCollection<StockMovement> Movements { get => _movements; set => SetProperty(ref _movements, value); }
    public DateTime FromDate { get => _fromDate; set { SetProperty(ref _fromDate, value); LoadData(); } }
    public DateTime ToDate { get => _toDate; set { SetProperty(ref _toDate, value); LoadData(); } }
    public MovementType? SelectedType { get => _selectedType; set { SetProperty(ref _selectedType, value); LoadData(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public ICommand RefreshCommand { get; }
    public ICommand ClearFilterCommand { get; }

    public MovementListViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadData());
        ClearFilterCommand = new RelayCommand(_ => ClearFilters());

        Subscribe(MediatorChannels.StockChanged, _ => LoadData());
        LoadData();
    }

    private void LoadData()
    {
        IsLoading = true;
        try
        {
            var data = _movementRepository.GetAll(_fromDate, _toDate.AddDays(1), type: _selectedType);
            Movements = new ObservableCollection<StockMovement>(data);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar movimientos:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearFilters()
    {
        _fromDate = DateTime.Today.AddDays(-30);
        _toDate = DateTime.Today;
        _selectedType = null;
        OnPropertyChanged(nameof(FromDate));
        OnPropertyChanged(nameof(ToDate));
        OnPropertyChanged(nameof(SelectedType));
        LoadData();
    }
}
