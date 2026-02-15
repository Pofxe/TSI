using System.Windows;
using TransportCompanyIS.ViewModels;

namespace TransportCompanyIS.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        var viewModel = new LoginViewModel();
        viewModel.LoginSucceeded += OnLoginSucceeded;
        DataContext = viewModel;
    }

    private void OnLoginSucceeded(Models.User user)
    {
        var mainWindow = new MainWindow
        {
            DataContext = new MainViewModel(user)
        };
        Application.Current.MainWindow = mainWindow;
        mainWindow.Show();
        Close();
    }
}
