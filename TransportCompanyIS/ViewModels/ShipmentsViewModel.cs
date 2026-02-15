using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;
using TransportCompanyIS.Views;

namespace TransportCompanyIS.ViewModels;

public class ShipmentsViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private Shipment? _selectedShipment;
    private string _searchText = string.Empty;
    private string _selectedStatus = "Все";
    private int _selectedVehicleId;

    public ObservableCollection<Shipment> Shipments { get; } = new();
    public ObservableCollection<string> StatusFilters { get; } = new() { "Все", "Запланировано", "В пути", "Доставлено" };
    public ObservableCollection<VehicleOption> VehicleFilters { get; } = new();

    public Shipment? SelectedShipment
    {
        get => _selectedShipment;
        set
        {
            _selectedShipment = value;
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

    public int SelectedVehicleId
    {
        get => _selectedVehicleId;
        set
        {
            _selectedVehicleId = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand CreateOrderCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand RefreshCommand { get; }

    public ShipmentsViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AddCommand = new RelayCommand(_ => AddShipment(), _ => _mainViewModel.CanManageShipments);
        CreateOrderCommand = new RelayCommand(_ => CreateOrder(), _ => _mainViewModel.CanManageShipments);
        EditCommand = new RelayCommand(_ => EditShipment(), _ => _mainViewModel.CanManageShipments && SelectedShipment != null);
        DeleteCommand = new RelayCommand(_ => DeleteShipment(), _ => _mainViewModel.CanManageShipments && SelectedShipment != null);
        SearchCommand = new RelayCommand(_ => LoadShipments());
        RefreshCommand = new RelayCommand(_ =>
        {
            SearchText = string.Empty;
            SelectedStatus = "Все";
            SelectedVehicleId = 0;
            LoadShipments();
        });

        LoadVehicleFilters();
        LoadShipments();
    }

    private void LoadVehicleFilters()
    {
        using var context = new AppDbContext();
        VehicleFilters.Clear();
        VehicleFilters.Add(new VehicleOption(0, "Все машины"));
        foreach (var vehicle in context.Vehicles.OrderBy(v => v.PlateNumber).ToList())
        {
            VehicleFilters.Add(new VehicleOption(vehicle.Id, $"{vehicle.PlateNumber} ({vehicle.Model})"));
        }
    }

    private void LoadShipments()
    {
        using var context = new AppDbContext();
        var query = context.Shipments
            .Include(s => s.AssignedVehicle)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(s => s.FromAddress.Contains(SearchText)
                                     || s.ToAddress.Contains(SearchText)
                                     || s.Status.Contains(SearchText)
                                     || s.Description.Contains(SearchText));
        }

        if (SelectedStatus != "Все")
        {
            query = query.Where(s => s.Status == SelectedStatus);
        }

        if (SelectedVehicleId != 0)
        {
            query = query.Where(s => s.AssignedVehicleId == SelectedVehicleId);
        }

        Shipments.Clear();
        foreach (var shipment in query.OrderBy(s => s.Id).ToList())
        {
            Shipments.Add(shipment);
        }
    }

    private void AddShipment()
    {
        var shipment = new Shipment();
        var viewModel = new ShipmentFormViewModel(shipment);
        var window = new ShipmentEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = viewModel
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Shipments.Add(viewModel.Shipment);
            context.SaveChanges();
            LoadVehicleFilters();
            LoadShipments();
        }
    }

    private void CreateOrder()
    {
        var shipment = new Shipment();
        var viewModel = new ShipmentFormViewModel(shipment);
        var window = new OrderCreateWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = viewModel
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Shipments.Add(viewModel.Shipment);
            context.SaveChanges();
            LoadShipments();
        }
    }

    private void EditShipment()
    {
        if (SelectedShipment == null)
        {
            return;
        }

        var editable = new Shipment
        {
            Id = SelectedShipment.Id,
            FromAddress = SelectedShipment.FromAddress,
            ToAddress = SelectedShipment.ToAddress,
            Status = SelectedShipment.Status,
            Description = SelectedShipment.Description,
            AssignedVehicleId = SelectedShipment.AssignedVehicleId
        };

        var viewModel = new ShipmentFormViewModel(editable);
        var window = new ShipmentEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = viewModel
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Shipments.Update(viewModel.Shipment);
            context.SaveChanges();
            LoadShipments();
        }
    }

    private void DeleteShipment()
    {
        if (SelectedShipment == null)
        {
            return;
        }

        if (MessageBox.Show("Удалить выбранную перевозку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        using var context = new AppDbContext();
        if (context.Trips.Any(t => t.ShipmentId == SelectedShipment.Id))
        {
            MessageBox.Show("Нельзя удалить перевозку, она используется в рейсах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var shipment = context.Shipments.FirstOrDefault(s => s.Id == SelectedShipment.Id);
        if (shipment == null)
        {
            return;
        }

        context.Shipments.Remove(shipment);
        context.SaveChanges();
        LoadShipments();
    }

    public record VehicleOption(int Id, string DisplayName);
}
