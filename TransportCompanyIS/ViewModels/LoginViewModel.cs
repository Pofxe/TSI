using System.Windows;
using System.Windows.Controls;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;

namespace TransportCompanyIS.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _login = string.Empty;

    public string Login
    {
        get => _login;
        set
        {
            _login = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand LoginCommand { get; }

    public event Action<User>? LoginSucceeded;

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    private void ExecuteLogin(object? parameter)
    {
        var password = string.Empty;
        if (parameter is PasswordBox passwordBox)
        {
            password = passwordBox.Password;
        }

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using var context = new AppDbContext();
        var user = context.Users.FirstOrDefault(u => u.Login == Login);
        if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        LoginSucceeded?.Invoke(user);
    }
}
