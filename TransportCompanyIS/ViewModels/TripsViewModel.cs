using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;
using TransportCompanyIS.Views;

namespace TransportCompanyIS.ViewModels;

public class TripsViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private Trip? _selectedTrip;
    private string _searchText = string.Empty;

    public ObservableCollection<Trip> Trips { get; } = new();

    public Trip? SelectedTrip
    {
        get => _selectedTrip;
        set
        {
            _selectedTrip = value;
            OnPropertyChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            ChangeStatusCommand.RaiseCanExecuteChanged();
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
    public RelayCommand ChangeStatusCommand { get; }

    public bool IsDriver => _mainViewModel.IsDriver;

    public TripsViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AddCommand = new RelayCommand(_ => AddTrip(), _ => _mainViewModel.CanManageTrips);
        EditCommand = new RelayCommand(_ => EditTrip(), _ => (_mainViewModel.CanManageTrips || _mainViewModel.CanManageShipments) && SelectedTrip != null);
        DeleteCommand = new RelayCommand(_ => DeleteTrip(), _ => (_mainViewModel.CanManageTrips || _mainViewModel.CanManageShipments) && SelectedTrip != null);
        ChangeStatusCommand = new RelayCommand(_ => ChangeStatus(), _ => _mainViewModel.IsDriver && SelectedTrip != null);
        SearchCommand = new RelayCommand(_ => LoadTrips());
        RefreshCommand = new RelayCommand(_ =>
        {
            SearchText = string.Empty;
            LoadTrips();
        });
        LoadTrips();
    }

    private void LoadTrips()
    {
        using var context = new AppDbContext();
        var query = context.Trips
            .Include(t => t.Shipment)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .AsQueryable();

        if (_mainViewModel.IsDriver)
        {
            query = query.Where(t => t.DriverId == _mainViewModel.CurrentUser.Id);
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(t => t.Status.Contains(SearchText)
                                     || (t.Shipment != null && (t.Shipment.FromAddress.Contains(SearchText) || t.Shipment.ToAddress.Contains(SearchText)))
                                     || (t.Vehicle != null && t.Vehicle.Model.Contains(SearchText)));
        }

        Trips.Clear();
        foreach (var trip in query.OrderBy(t => t.Id).ToList())
        {
            Trips.Add(trip);
        }
    }

    private void AddTrip()
    {
        var viewModel = new TripEditViewModel(new Trip());
        var window = new TripEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = viewModel
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Trips.Add(viewModel.Trip);
            context.SaveChanges();
            LoadTrips();
        }
    }

    private void EditTrip()
    {
        if (SelectedTrip == null)
        {
            return;
        }

        var editable = new Trip
        {
            Id = SelectedTrip.Id,
            ShipmentId = SelectedTrip.ShipmentId,
            VehicleId = SelectedTrip.VehicleId,
            DriverId = SelectedTrip.DriverId,
            Status = SelectedTrip.Status
        };

        var viewModel = new TripEditViewModel(editable);
        var window = new TripEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = viewModel
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Trips.Update(viewModel.Trip);
            context.SaveChanges();
            LoadTrips();
        }
    }

    private void DeleteTrip()
    {
        if (SelectedTrip == null)
        {
            return;
        }

        if (MessageBox.Show("Удалить выбранный рейс?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        using var context = new AppDbContext();
        context.Trips.Remove(SelectedTrip);
        context.SaveChanges();
        LoadTrips();
    }

    private void ChangeStatus()
    {
        if (SelectedTrip == null)
        {
            return;
        }

        var window = new StatusEditWindow
        {
            Owner = Application.Current.MainWindow,
            StatusText = SelectedTrip.Status
        };

        if (window.ShowDialog() == true)
        {
            var newStatus = window.StatusText;
            using var context = new AppDbContext();
            var trip = context.Trips.FirstOrDefault(t => t.Id == SelectedTrip.Id);
            if (trip == null)
            {
                return;
            }

            trip.Status = newStatus;
            context.SaveChanges();
            LoadTrips();
        }
    }
}
