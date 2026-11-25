using ClosedXML.Excel;
using InvenTrack.Data;
using InvenTrack.Models;
using System.Text;

namespace InvenTrack.Services;

public class ReportService
{
    private readonly ProductRepository _productRepository = new();
    private readonly StockMovementRepository _movementRepository = new();

    private const string HeaderColor = "#1E3A5F";
    private const string LowStockColor = "#FFF3CD";
    private const string OutOfStockColor = "#F8D7DA";

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
            string typeName = GetMovementTypeName(m.Type);
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

    public void ExportStockToExcel(string filePath)
    {
        var products = _productRepository.GetAll().ToList();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Stock Actual");

        ws.Cell(1, 1).Value = "REPORTE DE STOCK ACTUAL";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Range(1, 1, 1, 8).Merge();

        ws.Cell(2, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Cell(2, 1).Style.Font.Italic = true;
        ws.Range(2, 1, 2, 8).Merge();

        int row = 4;
        string[] headers = ["Código", "Producto", "Categoría", "Stock Actual", "Stock Mínimo", "Precio Unit.", "Valor Total", "Estado"];
        WriteHeaderRow(ws, row, headers);

        decimal totalValue = 0;
        row++;

        foreach (var p in products)
        {
            decimal value = p.UnitPrice * p.CurrentStock;
            totalValue += value;
            string status = p.IsOutOfStock ? "SIN STOCK" : p.IsLowStock ? "BAJO" : "OK";

            ws.Cell(row, 1).Value = p.Code;
            ws.Cell(row, 2).Value = p.Name;
            ws.Cell(row, 3).Value = p.CategoryName;
            ws.Cell(row, 4).Value = p.CurrentStock;
            ws.Cell(row, 5).Value = p.MinimumStock;
            ws.Cell(row, 6).Value = p.UnitPrice;
            ws.Cell(row, 7).Value = value;
            ws.Cell(row, 8).Value = status;

            ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";

            if (p.IsOutOfStock)
                ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml(OutOfStockColor);
            else if (p.IsLowStock)
                ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml(LowStockColor);

            row++;
        }

        ws.Cell(row, 6).Value = "TOTAL:";
        ws.Cell(row, 6).Style.Font.Bold = true;
        ws.Cell(row, 7).Value = totalValue;
        ws.Cell(row, 7).Style.Font.Bold = true;
        ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";

        ApplyTableBorders(ws, 4, 1, row - 1, headers.Length);
        ws.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }

    public void ExportMovementsToExcel(string filePath, DateTime from, DateTime to)
    {
        var movements = _movementRepository.GetAll(from, to.Date.AddDays(1)).ToList();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Movimientos");

        ws.Cell(1, 1).Value = "REPORTE DE MOVIMIENTOS DE INVENTARIO";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Range(1, 1, 1, 10).Merge();

        ws.Cell(2, 1).Value = $"Período: {from:dd/MM/yyyy} al {to:dd/MM/yyyy} — Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Cell(2, 1).Style.Font.Italic = true;
        ws.Range(2, 1, 2, 10).Merge();

        int row = 4;
        string[] headers = ["Fecha", "Código", "Producto", "Almacén", "Tipo", "Cantidad", "Stock Ant.", "Stock Nuevo", "Documento", "Usuario"];
        WriteHeaderRow(ws, row, headers);

        row++;
        foreach (var m in movements)
        {
            ws.Cell(row, 1).Value = m.CreatedAt;
            ws.Cell(row, 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            ws.Cell(row, 2).Value = m.ProductCode;
            ws.Cell(row, 3).Value = m.ProductName;
            ws.Cell(row, 4).Value = m.WarehouseName;
            ws.Cell(row, 5).Value = GetMovementTypeName(m.Type);
            ws.Cell(row, 6).Value = m.Quantity;
            ws.Cell(row, 7).Value = m.PreviousStock;
            ws.Cell(row, 8).Value = m.NewStock;
            ws.Cell(row, 9).Value = m.ReferenceDocument ?? "";
            ws.Cell(row, 10).Value = m.UserFullName;

            if (m.Type == MovementType.Exit)
                ws.Cell(row, 5).Style.Font.FontColor = XLColor.FromHtml("#DC3545");
            else if (m.Type == MovementType.Entry)
                ws.Cell(row, 5).Style.Font.FontColor = XLColor.FromHtml("#28A745");

            row++;
        }

        ws.Cell(row, 1).Value = $"Total de movimientos: {movements.Count}";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Range(row, 1, row, 3).Merge();

        ApplyTableBorders(ws, 4, 1, row - 1, headers.Length);
        ws.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }

    public void ExportLowStockToExcel(string filePath)
    {
        var products = _productRepository.GetLowStock().ToList();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Stock Bajo");

        ws.Cell(1, 1).Value = "ALERTA DE STOCK BAJO / AGOTADO";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#DC3545");
        ws.Range(1, 1, 1, 6).Merge();

        ws.Cell(2, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Cell(2, 1).Style.Font.Italic = true;
        ws.Range(2, 1, 2, 6).Merge();

        int row = 4;
        string[] headers = ["Código", "Producto", "Categoría", "Stock Actual", "Stock Mínimo", "Estado"];
        WriteHeaderRow(ws, row, headers);

        row++;
        foreach (var p in products)
        {
            string status = p.IsOutOfStock ? "AGOTADO" : "BAJO";

            ws.Cell(row, 1).Value = p.Code;
            ws.Cell(row, 2).Value = p.Name;
            ws.Cell(row, 3).Value = p.CategoryName;
            ws.Cell(row, 4).Value = p.CurrentStock;
            ws.Cell(row, 5).Value = p.MinimumStock;
            ws.Cell(row, 6).Value = status;

            if (p.IsOutOfStock)
                ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml(OutOfStockColor);
            else
                ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml(LowStockColor);

            row++;
        }

        ApplyTableBorders(ws, 4, 1, row - 1, headers.Length);
        ws.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }

    private static void WriteHeaderRow(IXLWorksheet ws, int row, string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(row, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(HeaderColor);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
    }

    private static void ApplyTableBorders(IXLWorksheet ws, int startRow, int startCol, int endRow, int colCount)
    {
        if (endRow < startRow) return;
        var range = ws.Range(startRow, startCol, endRow, colCount);
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D1D5DB");
        range.Style.Border.InsideBorderColor = XLColor.FromHtml("#E5E7EB");
    }

    private static string GetMovementTypeName(MovementType type) => type switch
    {
        MovementType.Entry => "Entrada",
        MovementType.Exit => "Salida",
        MovementType.Transfer => "Transferencia",
        MovementType.Adjustment => "Ajuste",
        _ => "Desconocido"
    };

    private static string Truncate(string text, int maxLength)
    {
        return text.Length <= maxLength ? text : text[..(maxLength - 2)] + "..";
    }
}
