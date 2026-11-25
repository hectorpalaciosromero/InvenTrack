using System.Data;
using Dapper;

namespace InvenTrack.Data;

public class UserRepository
{
    public Models.User? GetByUsername(string username)
    {
        using var db = DatabaseConnection.Create();
        return db.QueryFirstOrDefault<Models.User>(
            "sp_GetUserByUsername",
            new { Username = username },
            commandType: CommandType.StoredProcedure);
    }

    public Models.User? GetById(int userId)
    {
        using var db = DatabaseConnection.Create();
        return db.QueryFirstOrDefault<Models.User>(
            "sp_GetUserById",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<Models.User> GetAll()
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.User>(
            "sp_GetAllUsers",
            commandType: CommandType.StoredProcedure);
    }

    public int Create(Models.User user)
    {
        using var db = DatabaseConnection.Create();
        return db.QuerySingle<int>(
            "sp_CreateUser",
            new
            {
                user.Username,
                user.PasswordHash,
                user.FullName,
                user.Email,
                user.Role
            },
            commandType: CommandType.StoredProcedure);
    }

    public void Update(Models.User user)
    {
        using var db = DatabaseConnection.Create();
        db.Execute(
            "sp_UpdateUser",
            new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.Role,
                user.IsActive
            },
            commandType: CommandType.StoredProcedure);
    }

    public void UpdateLastLogin(int userId)
    {
        using var db = DatabaseConnection.Create();
        db.Execute(
            "UPDATE Users SET LastLogin = GETDATE() WHERE UserId = @UserId",
            new { UserId = userId });
    }

    public void UpdatePassword(int userId, string passwordHash)
    {
        using var db = DatabaseConnection.Create();
        db.Execute(
            "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId",
            new { UserId = userId, PasswordHash = passwordHash });
    }
}
