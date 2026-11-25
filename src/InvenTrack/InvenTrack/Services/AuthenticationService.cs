using InvenTrack.Data;
using InvenTrack.Models;

namespace InvenTrack.Services;

public class AuthenticationService
{
    private static AuthenticationService? _instance;
    public static AuthenticationService Instance => _instance ??= new AuthenticationService();

    private readonly UserRepository _userRepository = new();

    public User? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Administrador";

    public (bool Success, string Message) Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Ingrese usuario y contraseña.");

        var user = _userRepository.GetByUsername(username);

        if (user == null)
            return (false, "Usuario no encontrado.");

        if (!user.IsActive)
            return (false, "El usuario se encuentra deshabilitado.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "Contraseña incorrecta.");

        CurrentUser = user;
        _userRepository.UpdateLastLogin(user.UserId);

        return (true, string.Empty);
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 11);
    }
}
