using System.Windows;

namespace TransportCompanyIS.Views;

public partial class VehicleEditWindow : Window
{
    public VehicleEditWindow()
    {
        InitializeComponent();
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
