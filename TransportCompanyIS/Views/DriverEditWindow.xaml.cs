using System.Windows;
using TransportCompanyIS.ViewModels;

namespace TransportCompanyIS.Views;

public partial class DriverEditWindow : Window
{
    public DriverEditWindow()
    {
        InitializeComponent();
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not DriverEditViewModel viewModel)
        {
            return;
        }

        viewModel.Password = PasswordBox.Password;
        viewModel.PasswordConfirmation = ConfirmPasswordBox.Password;
        DialogResult = true;
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
