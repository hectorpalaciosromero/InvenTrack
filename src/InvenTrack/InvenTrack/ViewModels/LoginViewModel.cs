using InvenTrack.Core;
using InvenTrack.Services;
using System.Windows;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _username = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    private void ExecuteLogin(object? parameter)
    {
        ErrorMessage = string.Empty;

        string password = parameter as string ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Ingrese usuario y contraseña.";
            return;
        }

        IsLoading = true;

        try
        {
            var (success, message) = AuthenticationService.Instance.Login(Username, password);

            if (success)
            {
                var mainWindow = new Views.MainWindow();
                mainWindow.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Views.LoginWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
            else
            {
                ErrorMessage = message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error de conexión: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
