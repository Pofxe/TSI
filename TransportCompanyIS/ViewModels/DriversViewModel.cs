using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;
using TransportCompanyIS.Views;

namespace TransportCompanyIS.ViewModels;

public class DriversViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private DriverRow? _selectedDriver;
    private string _searchText = string.Empty;
    private string _selectedStatus = "Все";

    public ObservableCollection<DriverRow> Drivers { get; } = new();
    public ObservableCollection<string> StatusFilters { get; } = new() { "Все", "Свободен", "В рейсе", "Выходной" };

    public DriverRow? SelectedDriver
    {
        get => _selectedDriver;
        set
        {
            _selectedDriver = value;
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

    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand RefreshCommand { get; }

    public DriversViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AddCommand = new RelayCommand(_ => AddDriver(), _ => _mainViewModel.CanManageShipments);
        EditCommand = new RelayCommand(_ => EditDriver(), _ => _mainViewModel.CanManageShipments && SelectedDriver != null);
        DeleteCommand = new RelayCommand(_ => DeleteDriver(), _ => _mainViewModel.CanManageShipments && SelectedDriver != null);
        SearchCommand = new RelayCommand(_ => LoadDrivers());
        RefreshCommand = new RelayCommand(_ =>
        {
            SearchText = string.Empty;
            SelectedStatus = "Все";
            LoadDrivers();
        });

        LoadDrivers();
    }

    private void LoadDrivers()
    {
        using var context = new AppDbContext();
        var drivers = context.Users
            .Where(u => u.Role == UserRole.Driver)
            .OrderBy(u => u.FullName)
            .ToList();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            drivers = drivers.Where(d => d.FullName.Contains(SearchText)
                                         || d.PhoneNumber.Contains(SearchText)
                                         || d.Login.Contains(SearchText))
                .ToList();
        }

        if (SelectedStatus != "Все")
        {
            drivers = drivers.Where(d => d.DriverStatus == SelectedStatus).ToList();
        }

        var vehicles = context.Vehicles.Include(v => v.Driver).ToList();
        Drivers.Clear();
        foreach (var driver in drivers)
        {
            var vehicle = vehicles.FirstOrDefault(v => v.DriverId == driver.Id);
            Drivers.Add(new DriverRow
            {
                DriverId = driver.Id,
                Login = driver.Login,
                FullName = driver.FullName,
                PhoneNumber = driver.PhoneNumber,
                Status = driver.DriverStatus,
                VehicleDisplay = vehicle == null ? "Не назначен" : $"{vehicle.PlateNumber} ({vehicle.Model})",
                VehicleId = vehicle?.Id ?? 0
            });
        }
    }

    private void AddDriver()
    {
        var user = new User { Role = UserRole.Driver };
        var vm = new DriverEditViewModel(user, true);
        var window = new DriverEditWindow { Owner = Application.Current.MainWindow, DataContext = vm };
        if (window.ShowDialog() != true)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(user.Login) || string.IsNullOrWhiteSpace(user.FullName))
        {
            MessageBox.Show("Заполните логин и ФИО.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!vm.ValidatePasswords())
        {
            MessageBox.Show("Пароль должен совпадать, быть не короче 8 символов и содержать буквы и цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using var context = new AppDbContext();
        vm.ApplyPasswordIfChanged();
        context.Users.Add(user);
        context.SaveChanges();
        ApplyVehicleAssignment(context, user.Id, vm.SelectedVehicleId);
        context.SaveChanges();
        LoadDrivers();
    }

    private void EditDriver()
    {
        if (SelectedDriver == null)
        {
            return;
        }

        using var context = new AppDbContext();
        var driver = context.Users.FirstOrDefault(u => u.Id == SelectedDriver.DriverId && u.Role == UserRole.Driver);
        if (driver == null)
        {
            return;
        }

        var editable = new User
        {
            Id = driver.Id,
            Login = driver.Login,
            PasswordHash = driver.PasswordHash,
            Role = driver.Role,
            FullName = driver.FullName,
            PhoneNumber = driver.PhoneNumber,
            DriverStatus = driver.DriverStatus
        };

        var vm = new DriverEditViewModel(editable, false, SelectedDriver.VehicleId);
        var window = new DriverEditWindow { Owner = Application.Current.MainWindow, DataContext = vm };
        if (window.ShowDialog() != true)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(editable.Login) || string.IsNullOrWhiteSpace(editable.FullName))
        {
            MessageBox.Show("Заполните логин и ФИО.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!vm.ValidatePasswords())
        {
            MessageBox.Show("Пароль должен совпадать, быть не короче 8 символов и содержать буквы и цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        vm.ApplyPasswordIfChanged();
        context.Users.Update(editable);
        ApplyVehicleAssignment(context, editable.Id, vm.SelectedVehicleId);
        context.SaveChanges();
        LoadDrivers();
    }

    private void DeleteDriver()
    {
        if (SelectedDriver == null)
        {
            return;
        }

        if (MessageBox.Show("Удалить выбранного водителя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        using var context = new AppDbContext();
        if (context.Trips.Any(t => t.DriverId == SelectedDriver.DriverId))
        {
            MessageBox.Show("Нельзя удалить водителя, он назначен в рейсах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var driver = context.Users.FirstOrDefault(u => u.Id == SelectedDriver.DriverId && u.Role == UserRole.Driver);
        if (driver == null)
        {
            return;
        }

        var assignedVehicles = context.Vehicles.Where(v => v.DriverId == driver.Id).ToList();
        foreach (var vehicle in assignedVehicles)
        {
            vehicle.DriverId = null;
        }

        context.Users.Remove(driver);
        context.SaveChanges();
        LoadDrivers();
    }

    private static void ApplyVehicleAssignment(AppDbContext context, int driverId, int vehicleId)
    {
        var current = context.Vehicles.Where(v => v.DriverId == driverId).ToList();
        foreach (var vehicle in current)
        {
            vehicle.DriverId = null;
        }

        if (vehicleId == 0)
        {
            return;
        }

        var selectedVehicle = context.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (selectedVehicle != null)
        {
            selectedVehicle.DriverId = driverId;
        }
    }

    public class DriverRow
    {
        public int DriverId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string VehicleDisplay { get; set; } = string.Empty;
        public int VehicleId { get; set; }
    }
}
