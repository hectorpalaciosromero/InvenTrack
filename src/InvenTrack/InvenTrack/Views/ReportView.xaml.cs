using InvenTrack.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace InvenTrack.Views;

public partial class ReportView : UserControl
{
    public ReportView() => InitializeComponent();

    private ReportViewModel? ViewModel => DataContext as ReportViewModel;

    private void Report_Stock(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null) ViewModel.SelectedReportType = "Stock";
    }

    private void Report_Movements(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null) ViewModel.SelectedReportType = "Movements";
    }

    private void Report_LowStock(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null) ViewModel.SelectedReportType = "LowStock";
    }
}
