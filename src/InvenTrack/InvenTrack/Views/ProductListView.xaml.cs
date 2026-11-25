using InvenTrack.Services;
using System.Windows;
using System.Windows.Controls;

namespace InvenTrack.Views;

public partial class ProductListView : UserControl
{
    public ProductListView()
    {
        InitializeComponent();
        Loaded += (_, _) =>
            ActionsColumn.Visibility = AuthenticationService.Instance.IsAdmin
                ? Visibility.Visible
                : Visibility.Collapsed;
    }
}
