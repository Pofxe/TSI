using System.Collections.ObjectModel;
using System.Windows;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;
using TransportCompanyIS.Views;

namespace TransportCompanyIS.ViewModels;

public class UsersViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private User? _selectedUser;
    private string _searchText = string.Empty;

    public ObservableCollection<User> Users { get; } = new();

    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            _selectedUser = value;
            OnPropertyChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand RefreshCommand { get; }

    public UsersViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AddCommand = new RelayCommand(_ => AddUser(), _ => _mainViewModel.IsAdministrator);
        EditCommand = new RelayCommand(_ => EditUser(), _ => _mainViewModel.IsAdministrator && SelectedUser != null);
        DeleteCommand = new RelayCommand(_ => DeleteUser(), _ => _mainViewModel.IsAdministrator && SelectedUser != null);
        SearchCommand = new RelayCommand(_ => LoadUsers());
        RefreshCommand = new RelayCommand(_ =>
        {
            SearchText = string.Empty;
            LoadUsers();
        });
        LoadUsers();
    }

    private void LoadUsers()
    {
        using var context = new AppDbContext();
        var query = context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(u => u.Login.Contains(SearchText) || u.Role.ToString().Contains(SearchText));
        }

        Users.Clear();
        foreach (var user in query.OrderBy(u => u.Id).ToList())
        {
            Users.Add(user);
        }
    }

    private void AddUser()
    {
        var viewModel = new UserEditViewModel(new User(), isNew: true);
        var window = new UserEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = viewModel
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            viewModel.ApplyPasswordIfChanged();
            context.Users.Add(viewModel.User);
            context.SaveChanges();
            LoadUsers();
        }
    }

    private void EditUser()
    {
        if (SelectedUser == null)
        {
            return;
        }

        var editable = new User
        {
            Id = SelectedUser.Id,
            Login = SelectedUser.Login,
            PasswordHash = SelectedUser.PasswordHash,
            Role = SelectedUser.Role
        };

        var viewModel = new UserEditViewModel(editable, isNew: false);
        var window = new UserEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = viewModel
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            viewModel.ApplyPasswordIfChanged();
            context.Users.Update(viewModel.User);
            context.SaveChanges();
            LoadUsers();
        }
    }

    private void DeleteUser()
    {
        if (SelectedUser == null)
        {
            return;
        }

        if (SelectedUser.Id == _mainViewModel.CurrentUser.Id)
        {
            MessageBox.Show("Нельзя удалить текущего пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show("Удалить выбранного пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        using var context = new AppDbContext();
        var user = context.Users.FirstOrDefault(u => u.Id == SelectedUser.Id);
        if (user == null)
        {
            return;
        }

        context.Users.Remove(user);
        context.SaveChanges();
        LoadUsers();
    }
}
