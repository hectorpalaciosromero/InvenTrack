using System.Windows;
using System.Windows.Controls;

namespace InvenTrack.Views;

public partial class UserManagementView : UserControl
{
    public UserManagementView() => InitializeComponent();

    private void NewPasswordBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PasswordBox box && e.NewValue is false)
            box.Clear();
    }
}
