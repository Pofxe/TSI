using System.Collections.ObjectModel;
using System.Windows;
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

    public ObservableCollection<Shipment> Shipments { get; } = new();

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

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand RefreshCommand { get; }

    public ShipmentsViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AddCommand = new RelayCommand(_ => AddShipment(), _ => _mainViewModel.CanManageShipments);
        EditCommand = new RelayCommand(_ => EditShipment(), _ => _mainViewModel.CanManageShipments && SelectedShipment != null);
        DeleteCommand = new RelayCommand(_ => DeleteShipment(), _ => _mainViewModel.CanManageShipments && SelectedShipment != null);
        SearchCommand = new RelayCommand(_ => LoadShipments());
        RefreshCommand = new RelayCommand(_ =>
        {
            SearchText = string.Empty;
            LoadShipments();
        });
        LoadShipments();
    }

    private void LoadShipments()
    {
        using var context = new AppDbContext();
        var query = context.Shipments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(s => s.FromAddress.Contains(SearchText) || s.ToAddress.Contains(SearchText) || s.Status.Contains(SearchText));
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
        var window = new ShipmentEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = shipment
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Shipments.Add(shipment);
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
            Status = SelectedShipment.Status
        };

        var window = new ShipmentEditWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = editable
        };

        if (window.ShowDialog() == true)
        {
            using var context = new AppDbContext();
            context.Shipments.Update(editable);
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
        context.Shipments.Remove(SelectedShipment);
        context.SaveChanges();
        LoadShipments();
    }
}
