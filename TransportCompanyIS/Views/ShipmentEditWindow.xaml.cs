using System.Windows;

namespace TransportCompanyIS.Views;

public partial class ShipmentEditWindow : Window
{
    public ShipmentEditWindow()
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
