using InvenTrack.Core;
using InvenTrack.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace InvenTrack.ViewModels;

public class ReportViewModel : ViewModelBase
{
    private readonly ReportService _reportService = new();

    private string _reportContent = string.Empty;
    private string _selectedReportType = "Stock";
    private DateTime _fromDate = DateTime.Today.AddDays(-30);
    private DateTime _toDate = DateTime.Today;
    private bool _isLoading;

    public string ReportContent { get => _reportContent; set => SetProperty(ref _reportContent, value); }
    public string SelectedReportType
    {
        get => _selectedReportType;
        set
        {
            SetProperty(ref _selectedReportType, value);
            OnPropertyChanged(nameof(ShowDateRange));
        }
    }
    public DateTime FromDate { get => _fromDate; set => SetProperty(ref _fromDate, value); }
    public DateTime ToDate { get => _toDate; set => SetProperty(ref _toDate, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool ShowDateRange => SelectedReportType == "Movements";

    public ICommand GenerateCommand { get; }
    public ICommand ExportTxtCommand { get; }
    public ICommand ExportExcelCommand { get; }

    public ReportViewModel()
    {
        GenerateCommand = new RelayCommand(_ => GenerateReport());
        ExportTxtCommand = new RelayCommand(_ => ExportTxt(), _ => !string.IsNullOrEmpty(ReportContent));
        ExportExcelCommand = new RelayCommand(_ => ExportExcel(), _ => !string.IsNullOrEmpty(ReportContent));
    }

    private void GenerateReport()
    {
        if (ShowDateRange && FromDate > ToDate)
        {
            MessageBox.Show("La fecha inicial no puede ser mayor a la fecha final.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsLoading = true;
        try
        {
            ReportContent = SelectedReportType switch
            {
                "Stock" => _reportService.GenerateStockReport(),
                "Movements" => _reportService.GenerateMovementReport(FromDate, ToDate),
                "LowStock" => _reportService.GenerateLowStockReport(),
                _ => string.Empty
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al generar reporte:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ExportTxt()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Archivo de texto (*.txt)|*.txt",
            FileName = $"Reporte_{SelectedReportType}_{DateTime.Now:yyyyMMdd_HHmm}",
            DefaultExt = ".txt"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            File.WriteAllText(dialog.FileName, ReportContent);
            MessageBox.Show($"Reporte exportado exitosamente:\n{dialog.FileName}",
                "Exportación", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportExcel()
    {
        if (ShowDateRange && FromDate > ToDate)
        {
            MessageBox.Show("La fecha inicial no puede ser mayor a la fecha final.",
                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Filter = "Libro de Excel (*.xlsx)|*.xlsx",
            FileName = $"Reporte_{SelectedReportType}_{DateTime.Now:yyyyMMdd_HHmm}",
            DefaultExt = ".xlsx"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            switch (SelectedReportType)
            {
                case "Stock":
                    _reportService.ExportStockToExcel(dialog.FileName);
                    break;
                case "Movements":
                    _reportService.ExportMovementsToExcel(dialog.FileName, FromDate, ToDate);
                    break;
                case "LowStock":
                    _reportService.ExportLowStockToExcel(dialog.FileName);
                    break;
            }

            MessageBox.Show($"Reporte Excel exportado exitosamente:\n{dialog.FileName}",
                "Exportación", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar Excel:\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
