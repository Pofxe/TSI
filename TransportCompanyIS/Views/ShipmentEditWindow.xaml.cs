using System.Windows;
using TransportCompanyIS.ViewModels;

namespace TransportCompanyIS.Views;

public partial class ShipmentEditWindow : Window
{
    public ShipmentEditWindow()
    {
        InitializeComponent();
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ShipmentFormViewModel viewModel)
        {
            return;
        }

        if (viewModel.Shipment.AssignedVehicleId == 0)
        {
            viewModel.Shipment.AssignedVehicleId = null;
        }

        DialogResult = true;
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
