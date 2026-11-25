using System.Windows;
using System.Windows.Input;

namespace InvenTrack.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        MouseLeftButtonDown += (_, _) =>
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        };
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
