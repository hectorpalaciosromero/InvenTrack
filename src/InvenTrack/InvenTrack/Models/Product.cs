namespace InvenTrack.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string? Barcode { get; set; }
    public string? QRCode { get; set; }
    public string? RFIDTag { get; set; }
    public string Unit { get; set; } = "UND";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public bool IsLowStock => CurrentStock <= MinimumStock && CurrentStock > 0;
    public bool IsOutOfStock => CurrentStock == 0;
}
