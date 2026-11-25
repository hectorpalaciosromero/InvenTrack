using InvenTrack.Data;
using InvenTrack.Models;
using System.Text;

namespace InvenTrack.Services;

public class ReportService
{
    private readonly ProductRepository _productRepository = new();
    private readonly StockMovementRepository _movementRepository = new();

    public string GenerateStockReport()
    {
        var products = _productRepository.GetAll();
        var sb = new StringBuilder();

        sb.AppendLine("REPORTE DE STOCK ACTUAL");
        sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine(new string('=', 100));
        sb.AppendLine($"{"Código",-12} {"Producto",-30} {"Categoría",-18} {"Stock",8} {"Mín.",6} {"Precio",12} {"Valor Total",14} {"Estado",-12}");
        sb.AppendLine(new string('-', 100));

        decimal totalValue = 0;
        foreach (var p in products)
        {
            decimal value = p.UnitPrice * p.CurrentStock;
            totalValue += value;
            string status = p.IsOutOfStock ? "SIN STOCK" : p.IsLowStock ? "BAJO" : "OK";

            sb.AppendLine($"{p.Code,-12} {Truncate(p.Name, 28),-30} {Truncate(p.CategoryName, 16),-18} {p.CurrentStock,8} {p.MinimumStock,6} {p.UnitPrice,12:N2} {value,14:N2} {status,-12}");
        }

        sb.AppendLine(new string('=', 100));
        sb.AppendLine($"{"TOTAL",-78} {totalValue,14:N2}");

        return sb.ToString();
    }

    public string GenerateMovementReport(DateTime from, DateTime to)
    {
        var movements = _movementRepository.GetAll(from, to.Date.AddDays(1));
        var sb = new StringBuilder();

        sb.AppendLine("REPORTE DE MOVIMIENTOS");
        sb.AppendLine($"Período: {from:dd/MM/yyyy} al {to:dd/MM/yyyy}");
        sb.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine(new string('=', 120));
        sb.AppendLine($"{"Fecha",-18} {"Código",-12} {"Producto",-25} {"Tipo",-15} {"Cant.",7} {"Ant.",7} {"Nuevo",7} {"Método",-10} {"Documento",-15} {"Usuario",-15}");
        sb.AppendLine(new string('-', 120));

        foreach (var m in movements)
        {
            string typeName = m.Type switch
            {
                MovementType.Entry => "Entrada",
                MovementType.Exit => "Salida",
                MovementType.Transfer => "Transferencia",
                MovementType.Adjustment => "Ajuste",
                _ => "Desconocido"
            };

            sb.AppendLine($"{m.CreatedAt,-18:dd/MM/yyyy HH:mm} {m.ProductCode,-12} {Truncate(m.ProductName, 23),-25} {typeName,-15} {m.Quantity,7} {m.PreviousStock,7} {m.NewStock,7} {m.CaptureMethod,-10} {m.ReferenceDocument ?? "",-15} {Truncate(m.UserFullName, 13),-15}");
        }

        sb.AppendLine(new string('=', 120));
        return sb.ToString();
    }

    public string GenerateLowStockReport()
    {
        var products = _productRepository.GetLowStock();
        var sb = new StringBuilder();

        sb.AppendLine("ALERTA DE STOCK BAJO / AGOTADO");
        sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine(new string('=', 80));
        sb.AppendLine($"{"Código",-12} {"Producto",-30} {"Stock Actual",14} {"Stock Mínimo",14} {"Estado",-12}");
        sb.AppendLine(new string('-', 80));

        foreach (var p in products)
        {
            string status = p.IsOutOfStock ? "AGOTADO" : "BAJO";
            sb.AppendLine($"{p.Code,-12} {Truncate(p.Name, 28),-30} {p.CurrentStock,14} {p.MinimumStock,14} {status,-12}");
        }

        sb.AppendLine(new string('=', 80));
        return sb.ToString();
    }

    private static string Truncate(string text, int maxLength)
    {
        return text.Length <= maxLength ? text : text[..(maxLength - 2)] + "..";
    }
}
