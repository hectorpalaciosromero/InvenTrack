using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using InvenTrack.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class ScannerViewModel : ViewModelBase
{
    private readonly ScannerSimulatorService _scannerService = new();
    private readonly ProductRepository _productRepository = new();
    private readonly StockMovementRepository _movementRepository = new();
    private readonly WarehouseRepository _warehouseRepository = new();

    private ScanType _selectedScanType = ScanType.Barcode;
    private string _manualScanValue = string.Empty;
    private string _scanStatusMessage = string.Empty;
    private bool _scanStatusSuccess;
    private Product? _scannedProduct;
    private ObservableCollection<ScanResult> _scanHistory = [];
    private bool _isScanning;
    private List<Warehouse> _warehouses = [];
    private Warehouse? _selectedWarehouse;
    private int _quantity = 1;
    private MovementType _selectedMovementType = MovementType.Entry;

    public ScanType SelectedScanType { get => _selectedScanType; set => SetProperty(ref _selectedScanType, value); }
    public string ManualScanValue { get => _manualScanValue; set => SetProperty(ref _manualScanValue, value); }
    public string ScanStatusMessage { get => _scanStatusMessage; set => SetProperty(ref _scanStatusMessage, value); }
    public bool ScanStatusSuccess { get => _scanStatusSuccess; set => SetProperty(ref _scanStatusSuccess, value); }
    public Product? ScannedProduct { get => _scannedProduct; set => SetProperty(ref _scannedProduct, value); }
    public ObservableCollection<ScanResult> ScanHistory { get => _scanHistory; set => SetProperty(ref _scanHistory, value); }
    public bool IsScanning { get => _isScanning; set => SetProperty(ref _isScanning, value); }
    public List<Warehouse> Warehouses { get => _warehouses; set => SetProperty(ref _warehouses, value); }
    public Warehouse? SelectedWarehouse { get => _selectedWarehouse; set => SetProperty(ref _selectedWarehouse, value); }
    public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
    public MovementType SelectedMovementType { get => _selectedMovementType; set => SetProperty(ref _selectedMovementType, value); }

    public bool HasScannedProduct => ScannedProduct != null;

    public ICommand SimulateScanCommand { get; }
    public ICommand ManualScanCommand { get; }
    public ICommand RegisterMovementCommand { get; }
    public ICommand ClearCommand { get; }

    public ScannerViewModel()
    {
        SimulateScanCommand = new RelayCommand(_ => SimulateScan());
        ManualScanCommand = new RelayCommand(_ => ManualScan(), _ => !string.IsNullOrWhiteSpace(ManualScanValue));
        RegisterMovementCommand = new RelayCommand(_ => RegisterMovement(), _ => CanRegisterMovement());
        ClearCommand = new RelayCommand(_ => ClearScan());

        Warehouses = _warehouseRepository.GetAll().ToList();
        SelectedWarehouse = Warehouses.FirstOrDefault();
    }

    private void SimulateScan()
    {
        IsScanning = true;
        try
        {
            var result = _scannerService.SimulateScan(SelectedScanType);
            ProcessScanResult(result);
        }
        finally
        {
            IsScanning = false;
        }
    }

    private void ManualScan()
    {
        if (string.IsNullOrWhiteSpace(ManualScanValue)) return;

        IsScanning = true;
        try
        {
            var result = _scannerService.ProcessManualScan(ManualScanValue.Trim(), SelectedScanType);
            ProcessScanResult(result);
        }
        finally
        {
            IsScanning = false;
        }
    }

    private void ProcessScanResult(ScanResult result)
    {
        ScanHistory.Insert(0, result);
        ScannedProduct = result.Product;
        ScanStatusSuccess = result.WasResolved;
        ScanStatusMessage = result.WasResolved
            ? $"Producto identificado: {result.Product!.Name} (Stock: {result.Product.CurrentStock})"
            : $"No se encontró producto para el valor: {result.ScanValue}";
        OnPropertyChanged(nameof(HasScannedProduct));
    }

    private bool CanRegisterMovement()
    {
        return ScannedProduct != null && SelectedWarehouse != null && Quantity > 0;
    }

    private void RegisterMovement()
    {
        if (ScannedProduct == null || SelectedWarehouse == null) return;

        if (Quantity <= 0)
        {
            MessageBox.Show("La cantidad debe ser mayor a cero.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedMovementType == MovementType.Exit && Quantity > ScannedProduct.CurrentStock)
        {
            MessageBox.Show(
                $"Stock insuficiente. Stock actual: {ScannedProduct.CurrentStock}",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var movement = new StockMovement
            {
                ProductId = ScannedProduct.ProductId,
                WarehouseId = SelectedWarehouse.WarehouseId,
                Type = SelectedMovementType,
                Quantity = Quantity,
                CaptureMethod = SelectedScanType.ToString(),
                UserId = AuthenticationService.Instance.CurrentUser?.UserId ?? 1
            };

            _movementRepository.Create(movement);

            var updatedProduct = _productRepository.GetById(ScannedProduct.ProductId);
            ScannedProduct = updatedProduct;
            OnPropertyChanged(nameof(HasScannedProduct));

            string typeName = SelectedMovementType switch
            {
                MovementType.Entry => "Entrada",
                MovementType.Exit => "Salida",
                _ => "Movimiento"
            };

            string unitLabel = Quantity == 1 ? "unidad" : "unidades";
            ScanStatusMessage = $"{typeName} registrada: {Quantity} {unitLabel} de {ScannedProduct?.Name}";
            ScanStatusSuccess = true;

            Mediator.Publish(MediatorChannels.StockChanged);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al registrar movimiento:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ClearScan()
    {
        ScannedProduct = null;
        ScanStatusMessage = string.Empty;
        ManualScanValue = string.Empty;
        Quantity = 1;
        OnPropertyChanged(nameof(HasScannedProduct));
    }
}
