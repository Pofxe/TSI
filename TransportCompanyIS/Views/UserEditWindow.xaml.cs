using System.Linq;
using System.Windows;
using TransportCompanyIS.ViewModels;

namespace TransportCompanyIS.Views;

public partial class UserEditWindow : Window
{
    public UserEditWindow()
    {
        InitializeComponent();
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not UserEditViewModel viewModel)
        {
            return;
        }

        viewModel.Password = PasswordBox.Password;
        viewModel.PasswordConfirmation = PasswordConfirmBox.Password;

        if (!viewModel.ValidatePasswords())
        {
            MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(viewModel.User.Login))
        {
            MessageBox.Show("Укажите логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (viewModel.IsNew && string.IsNullOrWhiteSpace(viewModel.Password))
        {
            MessageBox.Show("Для нового пользователя требуется пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!string.IsNullOrWhiteSpace(viewModel.Password))
        {
            if (viewModel.Password.Length < 8 || !viewModel.Password.Any(char.IsLetter) || !viewModel.Password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен быть не короче 8 символов и содержать буквы и цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        DialogResult = true;
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
