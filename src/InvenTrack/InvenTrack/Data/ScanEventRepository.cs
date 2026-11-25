using System.Data;
using Dapper;

namespace InvenTrack.Data;

public class ScanEventRepository
{
    public int Create(Models.ScanEvent scanEvent)
    {
        using var db = DatabaseConnection.Create();
        return db.QuerySingle<int>(
            "sp_CreateScanEvent",
            new
            {
                scanEvent.ScanValue,
                Type = (int)scanEvent.Type,
                scanEvent.ProductId,
                scanEvent.WasResolved,
                scanEvent.UserId
            },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<Models.ScanEvent> GetRecent(int count = 20)
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.ScanEvent>(
            "sp_GetRecentScanEvents",
            new { Count = count },
            commandType: CommandType.StoredProcedure);
    }
}
