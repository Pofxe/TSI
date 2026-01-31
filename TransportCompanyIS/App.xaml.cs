using System.Windows;
using TransportCompanyIS.Data;

namespace TransportCompanyIS;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        DbInitializer.Initialize();
    }

    private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"Произошла ошибка: {e.Exception.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
