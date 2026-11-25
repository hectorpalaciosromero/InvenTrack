using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using InvenTrack.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class UserManagementViewModel : ViewModelBase
{
    private readonly UserRepository _userRepository = new();

    private ObservableCollection<User> _users = [];
    private User? _selectedUser;
    private string _newUsername = string.Empty;
    private string _newFullName = string.Empty;
    private string _newEmail = string.Empty;
    private string _newRole = "Operario";
    private bool _isFormVisible;
    private bool _isLoading;

    public ObservableCollection<User> Users { get => _users; set => SetProperty(ref _users, value); }
    public User? SelectedUser { get => _selectedUser; set => SetProperty(ref _selectedUser, value); }
    public string NewUsername { get => _newUsername; set => SetProperty(ref _newUsername, value); }
    public string NewFullName { get => _newFullName; set => SetProperty(ref _newFullName, value); }
    public string NewEmail { get => _newEmail; set => SetProperty(ref _newEmail, value); }
    public string NewRole { get => _newRole; set => SetProperty(ref _newRole, value); }
    public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public List<string> Roles { get; } = ["Operario", "Administrador"];

    public ICommand ShowFormCommand { get; }
    public ICommand SaveUserCommand { get; }
    public ICommand CancelFormCommand { get; }
    public ICommand ToggleActiveCommand { get; }
    public ICommand ResetPasswordCommand { get; }

    public UserManagementViewModel()
    {
        ShowFormCommand = new RelayCommand(_ => ShowForm());
        SaveUserCommand = new RelayCommand(SaveUser);
        CancelFormCommand = new RelayCommand(_ => HideForm());
        ToggleActiveCommand = new RelayCommand(_ => ToggleActive(), _ => SelectedUser != null);
        ResetPasswordCommand = new RelayCommand(_ => ResetPassword(), _ => SelectedUser != null);

        LoadData();
    }

    private void LoadData()
    {
        IsLoading = true;
        try
        {
            Users = new ObservableCollection<User>(_userRepository.GetAll());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar usuarios:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ShowForm()
    {
        NewUsername = string.Empty;
        NewFullName = string.Empty;
        NewEmail = string.Empty;
        NewRole = "Operario";
        IsFormVisible = true;
    }

    private void HideForm()
    {
        IsFormVisible = false;
    }

    private void SaveUser(object? parameter)
    {
        if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewFullName))
        {
            MessageBox.Show("Complete los campos obligatorios: Usuario y Nombre completo.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string password = (parameter as PasswordBox)?.Password ?? string.Empty;
        if (string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Ingrese una contraseña.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var user = new User
            {
                Username = NewUsername.Trim(),
                PasswordHash = AuthenticationService.Instance.HashPassword(password),
                FullName = NewFullName.Trim(),
                Email = NewEmail.Trim(),
                Role = NewRole
            };

            _userRepository.Create(user);
            HideForm();
            LoadData();

            MessageBox.Show("Usuario creado exitosamente.", "Éxito",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al crear usuario:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ToggleActive()
    {
        if (SelectedUser == null) return;

        string action = SelectedUser.IsActive ? "deshabilitar" : "habilitar";
        var result = MessageBox.Show(
            $"¿Está seguro que desea {action} al usuario '{SelectedUser.Username}'?",
            "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var updated = new User
            {
                UserId = SelectedUser.UserId,
                FullName = SelectedUser.FullName,
                Email = SelectedUser.Email,
                Role = SelectedUser.Role,
                IsActive = !SelectedUser.IsActive
            };
            _userRepository.Update(updated);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ResetPassword()
    {
        if (SelectedUser == null) return;

        var result = MessageBox.Show(
            $"¿Resetear contraseña de '{SelectedUser.Username}' a 'Reset123!'?",
            "Confirmar Reset", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            string hash = AuthenticationService.Instance.HashPassword("Reset123!");
            _userRepository.UpdatePassword(SelectedUser.UserId, hash);

            MessageBox.Show("Contraseña reseteada exitosamente.\nNueva contraseña: Reset123!",
                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
