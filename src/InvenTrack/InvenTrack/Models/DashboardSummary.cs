namespace InvenTrack.Models;

public class DashboardSummary
{
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public int TotalMovementsToday { get; set; }
    public int EntriesToday { get; set; }
    public int ExitsToday { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<Product> LowStockItems { get; set; } = [];
    public List<StockMovement> RecentMovements { get; set; } = [];
}
