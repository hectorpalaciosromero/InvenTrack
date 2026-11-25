namespace InvenTrack.Models;

public class Warehouse
{
    public int WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
}
