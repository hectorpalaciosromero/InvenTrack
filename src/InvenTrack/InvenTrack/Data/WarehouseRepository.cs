using System.Data;
using Dapper;

namespace InvenTrack.Data;

public class WarehouseRepository
{
    public IEnumerable<Models.Warehouse> GetAll()
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.Warehouse>(
            "sp_GetAllWarehouses",
            commandType: CommandType.StoredProcedure);
    }

    public Models.Warehouse? GetById(int warehouseId)
    {
        using var db = DatabaseConnection.Create();
        return db.QueryFirstOrDefault<Models.Warehouse>(
            "SELECT * FROM Warehouses WHERE WarehouseId = @WarehouseId",
            new { WarehouseId = warehouseId });
    }

    public int Create(Models.Warehouse warehouse)
    {
        using var db = DatabaseConnection.Create();
        return db.QuerySingle<int>(
            "sp_CreateWarehouse",
            new { warehouse.Name, warehouse.Location },
            commandType: CommandType.StoredProcedure);
    }

    public void Update(Models.Warehouse warehouse)
    {
        using var db = DatabaseConnection.Create();
        db.Execute(
            "UPDATE Warehouses SET Name = @Name, Location = @Location, IsActive = @IsActive WHERE WarehouseId = @WarehouseId",
            warehouse);
    }
}
