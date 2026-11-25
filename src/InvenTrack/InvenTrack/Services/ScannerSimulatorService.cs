using InvenTrack.Data;
using InvenTrack.Models;

namespace InvenTrack.Services;

public class ScannerSimulatorService
{
    private readonly ProductRepository _productRepository = new();
    private readonly ScanEventRepository _scanEventRepository = new();
    private readonly Random _random = new();

    private readonly Dictionary<ScanType, string[]> _prefixes = new()
    {
        [ScanType.Barcode] = ["750", "849", "890", "471"],
        [ScanType.QRCode]  = ["QR-PROD-", "QR-INV-", "QR-WH-"],
        [ScanType.RFID]    = ["RFID-TAG-", "RFID-EPC-", "E200"]
    };

    public ScanResult SimulateScan(ScanType scanType)
    {
        string scanValue = GenerateScanValue(scanType);
        var product = _productRepository.GetByScanValue(scanValue);

        var scanEvent = new ScanEvent
        {
            ScanValue = scanValue,
            Type = scanType,
            ProductId = product?.ProductId,
            WasResolved = product != null,
            UserId = AuthenticationService.Instance.CurrentUser?.UserId ?? 1
        };

        _scanEventRepository.Create(scanEvent);

        return new ScanResult
        {
            ScanValue = scanValue,
            ScanType = scanType,
            Product = product,
            WasResolved = product != null,
            Timestamp = DateTime.Now
        };
    }

    public ScanResult ProcessManualScan(string scanValue, ScanType scanType)
    {
        var product = _productRepository.GetByScanValue(scanValue);

        var scanEvent = new ScanEvent
        {
            ScanValue = scanValue,
            Type = scanType,
            ProductId = product?.ProductId,
            WasResolved = product != null,
            UserId = AuthenticationService.Instance.CurrentUser?.UserId ?? 1
        };

        _scanEventRepository.Create(scanEvent);

        return new ScanResult
        {
            ScanValue = scanValue,
            ScanType = scanType,
            Product = product,
            WasResolved = product != null,
            Timestamp = DateTime.Now
        };
    }

    private string GenerateScanValue(ScanType scanType)
    {
        var prefixList = _prefixes[scanType];
        string prefix = prefixList[_random.Next(prefixList.Length)];
        string suffix = _random.Next(1, 999).ToString("D3");
        return $"{prefix}{suffix}";
    }
}

public class ScanResult
{
    public string ScanValue { get; set; } = string.Empty;
    public ScanType ScanType { get; set; }
    public Product? Product { get; set; }
    public bool WasResolved { get; set; }
    public DateTime Timestamp { get; set; }

    public string StatusText => WasResolved ? "Producto identificado" : "Producto no encontrado";
}
