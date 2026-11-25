using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using InvenTrack.Services;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace InvenTrack.ViewModels;

public class ProductFormViewModel : ViewModelBase
{
    private readonly ProductRepository _productRepository = new();
    private readonly CategoryRepository _categoryRepository = new();
    private readonly BarcodeGeneratorService _barcodeService = new();

    private int _productId;
    private string _originalCode = string.Empty;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private Category? _selectedCategory;
    private decimal _unitPrice;
    private int _currentStock;
    private int _minimumStock = 5;
    private string _barcode = string.Empty;
    private string _qrCode = string.Empty;
    private string _rfidTag = string.Empty;
    private string _unit = "UND";
    private List<Category> _categories = [];
    private string _formTitle = "Nuevo Producto";
    private bool _isEditing;
    private BitmapSource? _barcodeImage;
    private BitmapSource? _qrCodeImage;

    public string Code { get => _code; set => SetProperty(ref _code, value); }
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public Category? SelectedCategory { get => _selectedCategory; set => SetProperty(ref _selectedCategory, value); }
    public decimal UnitPrice { get => _unitPrice; set => SetProperty(ref _unitPrice, value); }
    public int CurrentStock { get => _currentStock; set => SetProperty(ref _currentStock, value); }
    public int MinimumStock { get => _minimumStock; set => SetProperty(ref _minimumStock, value); }
    public string Unit { get => _unit; set => SetProperty(ref _unit, value); }
    public List<Category> Categories { get => _categories; set => SetProperty(ref _categories, value); }
    public string FormTitle { get => _formTitle; set => SetProperty(ref _formTitle, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public string StockFieldLabel => IsEditing ? "Stock Actual (solo lectura)" : "Stock Inicial";
    public bool HasNoPreview => BarcodeImage == null && QRCodeImage == null;

    public BitmapSource? BarcodeImage
    {
        get => _barcodeImage;
        set
        {
            SetProperty(ref _barcodeImage, value);
            OnPropertyChanged(nameof(HasNoPreview));
        }
    }

    public BitmapSource? QRCodeImage
    {
        get => _qrCodeImage;
        set
        {
            SetProperty(ref _qrCodeImage, value);
            OnPropertyChanged(nameof(HasNoPreview));
        }
    }

    public string Barcode
    {
        get => _barcode;
        set
        {
            SetProperty(ref _barcode, value);
            RefreshBarcodePreview();
        }
    }

    public string QRCode
    {
        get => _qrCode;
        set
        {
            SetProperty(ref _qrCode, value);
            RefreshQRPreview();
        }
    }

    public string RFIDTag { get => _rfidTag; set => SetProperty(ref _rfidTag, value); }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand GenerateCodeCommand { get; }

    public ProductFormViewModel()
    {
        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => Cancel());
        GenerateCodeCommand = new RelayCommand(_ => GenerateCode());

        Categories = _categoryRepository.GetAll().ToList();
    }

    public void LoadProduct(int productId)
    {
        var product = _productRepository.GetById(productId);
        if (product == null) return;

        _productId = product.ProductId;
        _originalCode = product.Code;
        Code = product.Code;
        Name = product.Name;
        Description = product.Description ?? string.Empty;
        SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
        UnitPrice = product.UnitPrice;
        CurrentStock = product.CurrentStock;
        MinimumStock = product.MinimumStock;
        Barcode = product.Barcode ?? string.Empty;
        QRCode = product.QRCode ?? string.Empty;
        RFIDTag = product.RFIDTag ?? string.Empty;
        Unit = product.Unit;

        IsEditing = true;
        FormTitle = "Editar Producto";
        OnPropertyChanged(nameof(StockFieldLabel));
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(Name) || SelectedCategory == null)
        {
            MessageBox.Show("Complete los campos obligatorios: Código, Nombre y Categoría.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Unit))
        {
            MessageBox.Show("Ingrese la unidad de medida (ej. UND, ROL, CAJ).",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (UnitPrice < 0 || CurrentStock < 0 || MinimumStock < 0)
        {
            MessageBox.Show("Precio unitario, stock y stock mínimo no pueden ser negativos.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string trimmedCode = Code.Trim();
        if (!IsEditing || !string.Equals(trimmedCode, _originalCode, StringComparison.OrdinalIgnoreCase))
        {
            var existing = _productRepository.GetByCode(trimmedCode);
            if (existing != null && existing.ProductId != _productId)
            {
                MessageBox.Show($"Ya existe un producto activo con el código '{trimmedCode}'.",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        try
        {
            var product = new Product
            {
                ProductId = _productId,
                Code = trimmedCode,
                Name = Name.Trim(),
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                CategoryId = SelectedCategory.CategoryId,
                UnitPrice = UnitPrice,
                CurrentStock = CurrentStock,
                MinimumStock = MinimumStock,
                Barcode = string.IsNullOrWhiteSpace(Barcode) ? null : Barcode.Trim(),
                QRCode = string.IsNullOrWhiteSpace(QRCode) ? null : QRCode.Trim(),
                RFIDTag = string.IsNullOrWhiteSpace(RFIDTag) ? null : RFIDTag.Trim(),
                Unit = Unit.Trim().ToUpperInvariant(),
                IsActive = true
            };

            if (IsEditing)
                _productRepository.Update(product);
            else
                _productRepository.Create(product);

            Mediator.Publish(MediatorChannels.ProductUpdated);
            Services.NavigationService.Instance.NavigateTo("Products");
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("Ya existe un producto registrado con ese código (incluyendo productos inactivos).",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel()
    {
        Services.NavigationService.Instance.NavigateTo("Products");
    }

    private void GenerateCode()
    {
        string number = Random.Shared.Next(1000, 9999).ToString();
        Code = $"PROD-{number}";
        Barcode = $"750123456{number}";
        QRCode = $"QR-{Code}";
        RFIDTag = $"RFID-TAG-{number}";
    }

    private void RefreshBarcodePreview()
    {
        BarcodeImage = _barcodeService.GenerateBarcode(_barcode);
    }

    private void RefreshQRPreview()
    {
        QRCodeImage = _barcodeService.GenerateQRCode(_qrCode);
    }
}
