using System.Windows;
using TransportCompanyIS.Data;

namespace TransportCompanyIS;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DbInitializer.Initialize();
    }
}
