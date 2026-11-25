using System.Data;
using Dapper;

namespace InvenTrack.Data;

public class StockMovementRepository
{
    public IEnumerable<Models.StockMovement> GetAll(DateTime? from = null, DateTime? to = null,
        int? productId = null, Models.MovementType? type = null)
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.StockMovement>(
            "sp_GetStockMovements",
            new
            {
                FromDate = from,
                ToDate = to,
                ProductId = productId,
                MovementType = type.HasValue ? (int)type.Value : (int?)null
            },
            commandType: CommandType.StoredProcedure);
    }

    public int Create(Models.StockMovement movement)
    {
        using var db = DatabaseConnection.Create();
        return db.QuerySingle<int>(
            "sp_CreateStockMovement",
            new
            {
                movement.ProductId,
                movement.WarehouseId,
                Type = (int)movement.Type,
                movement.Quantity,
                movement.ReferenceDocument,
                movement.Notes,
                movement.CaptureMethod,
                movement.UserId
            },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<Models.StockMovement> GetRecent(int count = 10)
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.StockMovement>(
            "sp_GetRecentMovements",
            new { Count = count },
            commandType: CommandType.StoredProcedure);
    }

    public Models.DashboardSummary GetDashboardSummary()
    {
        using var db = DatabaseConnection.Create();
        db.Open();

        var summary = db.QueryFirstOrDefault<Models.DashboardSummary>(
            "sp_GetDashboardSummary",
            commandType: CommandType.StoredProcedure) ?? new Models.DashboardSummary();

        summary.LowStockItems = db.Query<Models.Product>(
            "sp_GetLowStockProducts",
            commandType: CommandType.StoredProcedure).ToList();

        summary.RecentMovements = db.Query<Models.StockMovement>(
            "sp_GetRecentMovements",
            new { Count = 10 },
            commandType: CommandType.StoredProcedure).ToList();

        return summary;
    }
}
