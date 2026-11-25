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
        if (ViewModel != null)
        {
            ViewModel.SelectedReportType = "Stock";
            ViewModel.OnPropertyChanged(nameof(ViewModel.ShowDateRange));
        }
    }

    private void Report_Movements(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.SelectedReportType = "Movements";
            ViewModel.OnPropertyChanged(nameof(ViewModel.ShowDateRange));
        }
    }

    private void Report_LowStock(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.SelectedReportType = "LowStock";
            ViewModel.OnPropertyChanged(nameof(ViewModel.ShowDateRange));
        }
    }
}
