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
    public string SelectedReportType { get => _selectedReportType; set => SetProperty(ref _selectedReportType, value); }
    public DateTime FromDate { get => _fromDate; set => SetProperty(ref _fromDate, value); }
    public DateTime ToDate { get => _toDate; set => SetProperty(ref _toDate, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool ShowDateRange => SelectedReportType == "Movements";

    public ICommand GenerateCommand { get; }
    public ICommand ExportCommand { get; }

    public ReportViewModel()
    {
        GenerateCommand = new RelayCommand(_ => GenerateReport());
        ExportCommand = new RelayCommand(_ => ExportReport(), _ => !string.IsNullOrEmpty(ReportContent));
    }

    private void GenerateReport()
    {
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

    private void ExportReport()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Archivo de texto (*.txt)|*.txt|CSV (*.csv)|*.csv",
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
}
