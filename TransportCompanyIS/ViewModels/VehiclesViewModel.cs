using System.Collections.ObjectModel;
using System.Windows;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;
using TransportCompanyIS.Views;

namespace TransportCompanyIS.ViewModels;

public class VehiclesViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private Vehicle? _selectedVehicle;
    private string _searchText = string.Empty;

    public ObservableCollection<Vehicle> Vehicles { get; } = new();

    public Vehicle? SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            _selectedVehicle = value;
            OnPropertyChanged();
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

    public VehiclesViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AddCommand = new RelayCommand(_ => AddVehicle(), _ => _mainViewModel.CanManageVehicles);
        EditCommand = new RelayCommand(_ => EditVehicle(), _ => (_mainViewModel.CanManageVehicles || _mainViewModel.CanManageShipments) && SelectedVehicle != null);
        DeleteCommand = new RelayCommand(_ => DeleteVehicle(), _ => (_mainViewModel.CanManageVehicles || _mainViewModel.CanManageShipments) && SelectedVehicle != null);
        SearchCommand = new RelayCommand(_ => LoadVehicles());
        RefreshCommand = new RelayCommand(_ =>
        {
            SearchText = string.Empty;
            LoadVehicles();
        });
        LoadVehicles();
    }

    private void LoadVehicles()
    {
        using var context = new AppDbContext();
        var query = context.Vehicles.AsQueryable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(v => v.PlateNumber.Contains(SearchText) || v.Model.Contains(SearchText));
        }

        Vehicles.Clear();
        foreach (var vehicle in query.OrderBy(v => v.Id).ToList())
        {
            Vehicles.Add(vehicle);
        }
    }

    private void AddVehicle()
    {
        var vehicle = new Vehicle();
        var window = new VehicleEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = vehicle
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Vehicles.Add(vehicle);
            context.SaveChanges();
            LoadVehicles();
        }
    }

    private void EditVehicle()
    {
        if (SelectedVehicle == null)
        {
            return;
        }

        var editable = new Vehicle
        {
            Id = SelectedVehicle.Id,
            PlateNumber = SelectedVehicle.PlateNumber,
            Model = SelectedVehicle.Model,
            Status = SelectedVehicle.Status
        };

        var window = new VehicleEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = editable
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Vehicles.Update(editable);
            context.SaveChanges();
            LoadVehicles();
        }
    }

    private void DeleteVehicle()
    {
        if (SelectedVehicle == null)
        {
            return;
        }

        if (MessageBox.Show("Удалить выбранную машину?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        using var context = new AppDbContext();
        context.Vehicles.Remove(SelectedVehicle);
        context.SaveChanges();
        LoadVehicles();
    }
}
