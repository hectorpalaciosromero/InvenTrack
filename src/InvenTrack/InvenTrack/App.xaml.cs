using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace InvenTrack;

public partial class App : Application
{
    public static IConfiguration Configuration { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var loginWindow = new Views.LoginWindow();
        loginWindow.Show();
    }
}
