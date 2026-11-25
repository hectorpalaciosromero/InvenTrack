namespace InvenTrack.Models;

public class StockMovement
{
    public int MovementId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string? ReferenceDocument { get; set; }
    public string? Notes { get; set; }
    public string CaptureMethod { get; set; } = "Manual";
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public enum MovementType
{
    Entry = 1,
    Exit = 2,
    Transfer = 3,
    Adjustment = 4
}
