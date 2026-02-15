using System.Collections.ObjectModel;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;

namespace TransportCompanyIS.ViewModels;

public class DriverEditViewModel : ViewModelBase
{
    private string _password = string.Empty;
    private string _passwordConfirmation = string.Empty;
    private int _selectedVehicleId;

    public User Driver { get; }
    public bool IsNew { get; }

    public ObservableCollection<string> Statuses { get; } = new() { "Свободен", "В рейсе", "Выходной" };
    public ObservableCollection<VehicleOption> Vehicles { get; } = new();

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

    public int SelectedVehicleId
    {
        get => _selectedVehicleId;
        set
        {
            _selectedVehicleId = value;
            OnPropertyChanged();
        }
    }

    public DriverEditViewModel(User driver, bool isNew, int assignedVehicleId = 0)
    {
        Driver = driver;
        IsNew = isNew;
        if (string.IsNullOrWhiteSpace(Driver.DriverStatus))
        {
            Driver.DriverStatus = Statuses[0];
        }

        LoadVehicles();
        SelectedVehicleId = assignedVehicleId;
    }

    public bool ValidatePasswords()
    {
        if (string.IsNullOrWhiteSpace(Password) && string.IsNullOrWhiteSpace(PasswordConfirmation))
        {
            return !IsNew;
        }

        return Password == PasswordConfirmation && Password.Length >= 8 && Password.Any(char.IsLetter) && Password.Any(char.IsDigit);
    }

    public void ApplyPasswordIfChanged()
    {
        if (!string.IsNullOrWhiteSpace(Password))
        {
            Driver.PasswordHash = PasswordHasher.HashPassword(Password);
        }
    }

    private void LoadVehicles()
    {
        using var context = new AppDbContext();
        Vehicles.Add(new VehicleOption(0, "Не назначен"));
        foreach (var vehicle in context.Vehicles.OrderBy(v => v.PlateNumber).ToList())
        {
            Vehicles.Add(new VehicleOption(vehicle.Id, $"{vehicle.PlateNumber} ({vehicle.Model})"));
        }
    }

    public record VehicleOption(int Id, string DisplayName);
}
