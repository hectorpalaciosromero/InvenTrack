using InvenTrack.Core;
using InvenTrack.Services;
using System.Windows;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentView = null!;
    private string _currentViewTitle = "Dashboard";
    private string _userDisplayName = string.Empty;
    private string _userRole = string.Empty;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set
        {
            SetProperty(ref _currentView, value);
            OnPropertyChanged(nameof(IsDashboard));
            OnPropertyChanged(nameof(IsProducts));
            OnPropertyChanged(nameof(IsScanner));
            OnPropertyChanged(nameof(IsMovements));
            OnPropertyChanged(nameof(IsReports));
            OnPropertyChanged(nameof(IsUsers));
        }
    }

    public string CurrentViewTitle
    {
        get => _currentViewTitle;
        set => SetProperty(ref _currentViewTitle, value);
    }

    public string UserDisplayName
    {
        get => _userDisplayName;
        set => SetProperty(ref _userDisplayName, value);
    }

    public string UserRole
    {
        get => _userRole;
        set => SetProperty(ref _userRole, value);
    }

    public bool IsAdmin => AuthenticationService.Instance.IsAdmin;

    public bool IsDashboard  => CurrentView is DashboardViewModel;
    public bool IsProducts   => CurrentView is ProductListViewModel;
    public bool IsScanner    => CurrentView is ScannerViewModel;
    public bool IsMovements  => CurrentView is MovementListViewModel;
    public bool IsReports    => CurrentView is ReportViewModel;
    public bool IsUsers      => CurrentView is UserManagementViewModel;

    public ICommand NavigateCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        UserDisplayName = user?.FullName ?? "Usuario";
        UserRole = user?.Role ?? "Operario";

        NavigateCommand = new RelayCommand(ExecuteNavigate);
        LogoutCommand = new RelayCommand(ExecuteLogout);

        Mediator.Subscribe(MediatorChannels.NavigateTo, data => ExecuteNavigate(data));

        CurrentView = new DashboardViewModel();
    }

    private void ExecuteNavigate(object? parameter)
    {
        if (parameter is ViewModelBase viewModel)
        {
            CurrentView = viewModel;
            CurrentViewTitle = viewModel switch
            {
                DashboardViewModel => "Dashboard",
                ProductListViewModel => "Productos",
                ProductFormViewModel => "Producto",
                ScannerViewModel => "Simulador de Captura",
                MovementListViewModel => "Movimientos de Almacén",
                ReportViewModel => "Reportes",
                UserManagementViewModel => "Gestión de Usuarios",
                _ => "InvenTrack"
            };
            return;
        }

        string viewName = parameter as string ?? "Dashboard";

        (CurrentView, CurrentViewTitle) = viewName switch
        {
            "Dashboard"  => ((ViewModelBase)new DashboardViewModel(), "Dashboard"),
            "Products"   => (new ProductListViewModel(), "Productos"),
            "Scanner"    => (new ScannerViewModel(), "Simulador de Captura"),
            "Movements"  => (new MovementListViewModel(), "Movimientos de Almacén"),
            "Reports"    => (new ReportViewModel(), "Reportes"),
            "Users"      => (new UserManagementViewModel(), "Gestión de Usuarios"),
            _ => (new DashboardViewModel(), "Dashboard")
        };
    }

    private void ExecuteLogout(object? parameter)
    {
        var result = MessageBox.Show(
            "¿Está seguro que desea cerrar sesión?",
            "Cerrar Sesión",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        AuthenticationService.Instance.Logout();

        var loginWindow = new Views.LoginWindow();
        loginWindow.Show();

        foreach (Window window in Application.Current.Windows)
        {
            if (window is Views.MainWindow)
            {
                window.Close();
                break;
            }
        }
    }
}
