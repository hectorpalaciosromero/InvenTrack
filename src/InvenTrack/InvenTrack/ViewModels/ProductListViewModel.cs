using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class ProductListViewModel : ViewModelBase
{
    private readonly ProductRepository _productRepository = new();
    private readonly CategoryRepository _categoryRepository = new();

    private ObservableCollection<Product> _products = [];
    private List<Category> _categories = [];
    private string _searchText = string.Empty;
    private Category? _selectedCategory;
    private Product? _selectedProduct;
    private bool _isLoading;

    public ObservableCollection<Product> Products { get => _products; set => SetProperty(ref _products, value); }
    public List<Category> Categories { get => _categories; set => SetProperty(ref _categories, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); Search(); } }
    public Category? SelectedCategory { get => _selectedCategory; set { SetProperty(ref _selectedCategory, value); Search(); } }
    public Product? SelectedProduct { get => _selectedProduct; set => SetProperty(ref _selectedProduct, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ClearFilterCommand { get; }

    public ProductListViewModel()
    {
        AddCommand = new RelayCommand(_ => OpenForm(null));
        EditCommand = new RelayCommand(_ => OpenForm(SelectedProduct), _ => SelectedProduct != null);
        DeleteCommand = new RelayCommand(_ => DeleteProduct(), _ => SelectedProduct != null);
        RefreshCommand = new RelayCommand(_ => LoadData());
        ClearFilterCommand = new RelayCommand(_ => ClearFilters());

        LoadCategories();
        LoadData();

        Mediator.Subscribe(MediatorChannels.ProductUpdated, _ => LoadData());
    }

    private void LoadCategories()
    {
        var allCategory = new Category { CategoryId = 0, Name = "Todas las categorías" };
        Categories = [allCategory, .. _categoryRepository.GetAll()];
    }

    private void LoadData()
    {
        IsLoading = true;
        try
        {
            int? catId = _selectedCategory?.CategoryId > 0 ? _selectedCategory.CategoryId : null;
            string? search = string.IsNullOrWhiteSpace(_searchText) ? null : _searchText;

            Products = new ObservableCollection<Product>(_productRepository.GetAll(search, catId));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar productos:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Search() => LoadData();

    private void ClearFilters()
    {
        _searchText = string.Empty;
        OnPropertyChanged(nameof(SearchText));
        _selectedCategory = Categories.FirstOrDefault();
        OnPropertyChanged(nameof(SelectedCategory));
        LoadData();
    }

    private void OpenForm(Product? product)
    {
        var formViewModel = new ProductFormViewModel();
        if (product != null)
            formViewModel.LoadProduct(product.ProductId);

        Mediator.Publish(MediatorChannels.NavigateTo, formViewModel);
    }

    private void DeleteProduct()
    {
        if (SelectedProduct == null) return;

        var result = MessageBox.Show(
            $"¿Está seguro que desea eliminar el producto '{SelectedProduct.Name}'?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            _productRepository.Delete(SelectedProduct.ProductId);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
