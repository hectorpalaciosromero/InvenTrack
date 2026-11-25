using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using System.Windows;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class ProductFormViewModel : ViewModelBase
{
    private readonly ProductRepository _productRepository = new();
    private readonly CategoryRepository _categoryRepository = new();

    private int _productId;
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

    public string Code { get => _code; set => SetProperty(ref _code, value); }
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public Category? SelectedCategory { get => _selectedCategory; set => SetProperty(ref _selectedCategory, value); }
    public decimal UnitPrice { get => _unitPrice; set => SetProperty(ref _unitPrice, value); }
    public int CurrentStock { get => _currentStock; set => SetProperty(ref _currentStock, value); }
    public int MinimumStock { get => _minimumStock; set => SetProperty(ref _minimumStock, value); }
    public string Barcode { get => _barcode; set => SetProperty(ref _barcode, value); }
    public string QRCode { get => _qrCode; set => SetProperty(ref _qrCode, value); }
    public string RFIDTag { get => _rfidTag; set => SetProperty(ref _rfidTag, value); }
    public string Unit { get => _unit; set => SetProperty(ref _unit, value); }
    public List<Category> Categories { get => _categories; set => SetProperty(ref _categories, value); }
    public string FormTitle { get => _formTitle; set => SetProperty(ref _formTitle, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public bool ShowStockField => !IsEditing;

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
        OnPropertyChanged(nameof(ShowStockField));
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(Name) || SelectedCategory == null)
        {
            MessageBox.Show("Complete los campos obligatorios: Código, Nombre y Categoría.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var product = new Product
            {
                ProductId = _productId,
                Code = Code.Trim(),
                Name = Name.Trim(),
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                CategoryId = SelectedCategory.CategoryId,
                UnitPrice = UnitPrice,
                CurrentStock = CurrentStock,
                MinimumStock = MinimumStock,
                Barcode = string.IsNullOrWhiteSpace(Barcode) ? null : Barcode.Trim(),
                QRCode = string.IsNullOrWhiteSpace(QRCode) ? null : QRCode.Trim(),
                RFIDTag = string.IsNullOrWhiteSpace(RFIDTag) ? null : RFIDTag.Trim(),
                Unit = Unit,
                IsActive = true
            };

            if (IsEditing)
                _productRepository.Update(product);
            else
                _productRepository.Create(product);

            Mediator.Publish(MediatorChannels.ProductUpdated);
            Services.NavigationService.Instance.NavigateTo("Products");
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
        string prefix = "PROD";
        string number = new Random().Next(1000, 9999).ToString();
        Code = $"{prefix}-{number}";

        Barcode = $"750{new Random().Next(1000000, 9999999)}";
        QRCode = $"QR-{Code}";
        RFIDTag = $"RFID-TAG-{number}";
    }
}
