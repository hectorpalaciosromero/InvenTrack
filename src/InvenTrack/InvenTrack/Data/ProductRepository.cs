using System.Data;
using Dapper;

namespace InvenTrack.Data;

public class ProductRepository
{
    public IEnumerable<Models.Product> GetAll(string? search = null, int? categoryId = null)
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.Product>(
            "sp_GetAllProducts",
            new { Search = search, CategoryId = categoryId },
            commandType: CommandType.StoredProcedure);
    }

    public Models.Product? GetById(int productId)
    {
        using var db = DatabaseConnection.Create();
        return db.QueryFirstOrDefault<Models.Product>(
            "sp_GetProductById",
            new { ProductId = productId },
            commandType: CommandType.StoredProcedure);
    }

    public Models.Product? GetByCode(string code)
    {
        using var db = DatabaseConnection.Create();
        return db.QueryFirstOrDefault<Models.Product>(
            "sp_GetProductByCode",
            new { Code = code },
            commandType: CommandType.StoredProcedure);
    }

    public Models.Product? GetByScanValue(string scanValue)
    {
        using var db = DatabaseConnection.Create();
        return db.QueryFirstOrDefault<Models.Product>(
            "sp_GetProductByScanValue",
            new { ScanValue = scanValue },
            commandType: CommandType.StoredProcedure);
    }

    public int Create(Models.Product product)
    {
        using var db = DatabaseConnection.Create();
        return db.QuerySingle<int>(
            "sp_CreateProduct",
            new
            {
                product.Code,
                product.Name,
                product.Description,
                product.CategoryId,
                product.UnitPrice,
                product.CurrentStock,
                product.MinimumStock,
                product.Barcode,
                product.QRCode,
                product.RFIDTag,
                product.Unit
            },
            commandType: CommandType.StoredProcedure);
    }

    public void Update(Models.Product product)
    {
        using var db = DatabaseConnection.Create();
        db.Execute(
            "sp_UpdateProduct",
            new
            {
                product.ProductId,
                product.Code,
                product.Name,
                product.Description,
                product.CategoryId,
                product.UnitPrice,
                product.MinimumStock,
                product.Barcode,
                product.QRCode,
                product.RFIDTag,
                product.Unit,
                product.IsActive
            },
            commandType: CommandType.StoredProcedure);
    }

    public void Delete(int productId)
    {
        using var db = DatabaseConnection.Create();
        db.Execute(
            "UPDATE Products SET IsActive = 0, UpdatedAt = GETDATE() WHERE ProductId = @ProductId",
            new { ProductId = productId });
    }

    public IEnumerable<Models.Product> GetLowStock()
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.Product>(
            "sp_GetLowStockProducts",
            commandType: CommandType.StoredProcedure);
    }
}
