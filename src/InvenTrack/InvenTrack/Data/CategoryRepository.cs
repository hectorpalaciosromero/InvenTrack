using System.Data;
using Dapper;

namespace InvenTrack.Data;

public class CategoryRepository
{
    public IEnumerable<Models.Category> GetAll(bool includeInactive = false)
    {
        using var db = DatabaseConnection.Create();
        return db.Query<Models.Category>(
            "sp_GetAllCategories",
            new { IncludeInactive = includeInactive },
            commandType: CommandType.StoredProcedure);
    }

    public Models.Category? GetById(int categoryId)
    {
        using var db = DatabaseConnection.Create();
        return db.QueryFirstOrDefault<Models.Category>(
            "sp_GetCategoryById",
            new { CategoryId = categoryId },
            commandType: CommandType.StoredProcedure);
    }

    public int Create(Models.Category category)
    {
        using var db = DatabaseConnection.Create();
        return db.QuerySingle<int>(
            "sp_CreateCategory",
            new { category.Name, category.Description },
            commandType: CommandType.StoredProcedure);
    }

    public void Update(Models.Category category)
    {
        using var db = DatabaseConnection.Create();
        db.Execute(
            "sp_UpdateCategory",
            new { category.CategoryId, category.Name, category.Description, category.IsActive },
            commandType: CommandType.StoredProcedure);
    }
}
