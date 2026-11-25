namespace InvenTrack.Models;

public class ScanEvent
{
    public int ScanEventId { get; set; }
    public string ScanValue { get; set; } = string.Empty;
    public ScanType Type { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public bool WasResolved { get; set; }
    public int UserId { get; set; }
    public DateTime ScannedAt { get; set; }
}

public enum ScanType
{
    Barcode = 1,
    QRCode = 2,
    RFID = 3
}
