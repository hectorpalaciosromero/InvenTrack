using Microsoft.Data.SqlClient;
using System.Data;

namespace InvenTrack.Data;

public static class DatabaseConnection
{
    private static string? _connectionString;

    public static string ConnectionString
    {
        get
        {
            _connectionString ??= App.Configuration.GetSection("ConnectionStrings")["InvenTrack"]
                ?? throw new InvalidOperationException("Connection string 'InvenTrack' not found in appsettings.json");
            return _connectionString;
        }
    }

    public static IDbConnection Create() => new SqlConnection(ConnectionString);

    public static bool TestConnection()
    {
        try
        {
            using var connection = Create();
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
