using System.Collections.ObjectModel;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;

namespace TransportCompanyIS.ViewModels;

public class UserEditViewModel : ViewModelBase
{
    private string _password = string.Empty;
    private string _passwordConfirmation = string.Empty;

    public User User { get; }
    public bool IsNew { get; }

    public ObservableCollection<UserRole> Roles { get; } = new()
    {
        UserRole.Administrator,
        UserRole.Dispatcher,
        UserRole.Driver
    };

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public string PasswordConfirmation
    {
        get => _passwordConfirmation;
        set
        {
            _passwordConfirmation = value;
            OnPropertyChanged();
        }
    }

    public UserEditViewModel(User user, bool isNew)
    {
        User = user;
        IsNew = isNew;
    }

    public bool ValidatePasswords()
    {
        if (string.IsNullOrWhiteSpace(Password) && string.IsNullOrWhiteSpace(PasswordConfirmation))
        {
            return true;
        }

        return Password == PasswordConfirmation;
    }

    public void ApplyPasswordIfChanged()
    {
        if (!string.IsNullOrWhiteSpace(Password))
        {
            User.PasswordHash = PasswordHasher.HashPassword(Password);
        }
    }
}
