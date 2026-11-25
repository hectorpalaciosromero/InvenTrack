using InvenTrack.Core;
using InvenTrack.Data;
using InvenTrack.Models;
using InvenTrack.Services;
using Microsoft.Data.SqlClient;
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
    private bool _isEditing;
    private int _editingUserId;
    private string _formTitle = "Nuevo Usuario";

    public ObservableCollection<User> Users { get => _users; set => SetProperty(ref _users, value); }
    public User? SelectedUser { get => _selectedUser; set => SetProperty(ref _selectedUser, value); }
    public string NewUsername { get => _newUsername; set => SetProperty(ref _newUsername, value); }
    public string NewFullName { get => _newFullName; set => SetProperty(ref _newFullName, value); }
    public string NewEmail { get => _newEmail; set => SetProperty(ref _newEmail, value); }
    public string NewRole { get => _newRole; set => SetProperty(ref _newRole, value); }
    public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsEditing { get => _isEditing; set { SetProperty(ref _isEditing, value); OnPropertyChanged(nameof(IsCreating)); OnPropertyChanged(nameof(SaveButtonText)); } }
    public bool IsCreating => !IsEditing;
    public string FormTitle { get => _formTitle; set => SetProperty(ref _formTitle, value); }
    public string SaveButtonText => IsEditing ? "💾 Guardar Cambios" : "💾 Crear Usuario";
    public List<string> Roles { get; } = ["Operario", "Administrador"];

    public ICommand ShowFormCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand SaveUserCommand { get; }
    public ICommand CancelFormCommand { get; }
    public ICommand ToggleActiveCommand { get; }
    public ICommand ResetPasswordCommand { get; }

    public UserManagementViewModel()
    {
        ShowFormCommand = new RelayCommand(_ => ShowForm());
        EditUserCommand = new RelayCommand(_ => EditUser(), _ => SelectedUser != null);
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
        _editingUserId = 0;
        IsEditing = false;
        FormTitle = "Nuevo Usuario";
        NewUsername = string.Empty;
        NewFullName = string.Empty;
        NewEmail = string.Empty;
        NewRole = "Operario";
        IsFormVisible = true;
    }

    private void EditUser()
    {
        if (SelectedUser == null) return;

        _editingUserId = SelectedUser.UserId;
        IsEditing = true;
        FormTitle = $"Editar Usuario — {SelectedUser.Username}";
        NewUsername = SelectedUser.Username;
        NewFullName = SelectedUser.FullName;
        NewEmail = SelectedUser.Email ?? string.Empty;
        NewRole = SelectedUser.Role;
        IsFormVisible = true;
    }

    private void HideForm()
    {
        IsFormVisible = false;
        IsEditing = false;
        _editingUserId = 0;
    }

    private void SaveUser(object? parameter)
    {
        if (string.IsNullOrWhiteSpace(NewFullName))
        {
            MessageBox.Show("Ingrese el nombre completo.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IsEditing)
        {
            UpdateExistingUser();
        }
        else
        {
            CreateNewUser(parameter);
        }
    }

    private void CreateNewUser(object? parameter)
    {
        if (string.IsNullOrWhiteSpace(NewUsername))
        {
            MessageBox.Show("Ingrese el nombre de usuario.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string trimmedUsername = NewUsername.Trim();
        if (_userRepository.GetByUsername(trimmedUsername) != null)
        {
            MessageBox.Show($"El nombre de usuario '{trimmedUsername}' ya está registrado.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string password = (parameter as PasswordBox)?.Password ?? string.Empty;
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var user = new User
            {
                Username = trimmedUsername,
                PasswordHash = AuthenticationService.Instance.HashPassword(password),
                FullName = NewFullName.Trim(),
                Email = (NewEmail ?? string.Empty).Trim(),
                Role = NewRole
            };

            _userRepository.Create(user);
            HideForm();
            LoadData();

            MessageBox.Show("Usuario creado exitosamente.", "Éxito",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("El nombre de usuario ya existe en el sistema.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al crear usuario:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateExistingUser()
    {
        if (_editingUserId == AuthenticationService.Instance.CurrentUser?.UserId && NewRole != "Administrador")
        {
            MessageBox.Show("No puede cambiar su propio rol de Administrador mientras tiene la sesión activa.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var original = _userRepository.GetById(_editingUserId);
            if (original == null) return;

            var user = new User
            {
                UserId = _editingUserId,
                FullName = NewFullName.Trim(),
                Email = (NewEmail ?? string.Empty).Trim(),
                Role = NewRole,
                IsActive = original.IsActive
            };

            _userRepository.Update(user);
            HideForm();
            LoadData();

            MessageBox.Show("Usuario actualizado exitosamente.", "Éxito",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al actualizar usuario:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ToggleActive()
    {
        if (SelectedUser == null) return;

        if (SelectedUser.UserId == AuthenticationService.Instance.CurrentUser?.UserId)
        {
            MessageBox.Show("No puede deshabilitar su propio usuario mientras tiene la sesión activa.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

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
