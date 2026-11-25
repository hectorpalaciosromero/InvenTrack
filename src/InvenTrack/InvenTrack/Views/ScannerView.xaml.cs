using InvenTrack.Models;
using InvenTrack.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace InvenTrack.Views;

public partial class ScannerView : UserControl
{
    public ScannerView() => InitializeComponent();

    private ScannerViewModel? ViewModel => DataContext as ScannerViewModel;

    private void ScanType_Barcode(object sender, RoutedEventArgs e) =>
        SetScanType(ScanType.Barcode);

    private void ScanType_QR(object sender, RoutedEventArgs e) =>
        SetScanType(ScanType.QRCode);

    private void ScanType_RFID(object sender, RoutedEventArgs e) =>
        SetScanType(ScanType.RFID);

    private void MovType_Entry(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null) ViewModel.SelectedMovementType = MovementType.Entry;
    }

    private void MovType_Exit(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null) ViewModel.SelectedMovementType = MovementType.Exit;
    }

    private void SetScanType(ScanType type)
    {
        if (ViewModel != null) ViewModel.SelectedScanType = type;
    }
}
