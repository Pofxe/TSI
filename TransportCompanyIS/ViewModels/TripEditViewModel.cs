using System.Collections.ObjectModel;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;

namespace TransportCompanyIS.ViewModels;

public class TripEditViewModel : ViewModelBase
{
    public Trip Trip { get; }
    public ObservableCollection<Shipment> Shipments { get; } = new();
    public ObservableCollection<Vehicle> Vehicles { get; } = new();
    public ObservableCollection<User> Drivers { get; } = new();

    public TripEditViewModel(Trip trip)
    {
        Trip = trip;
        LoadData();
    }

    private void LoadData()
    {
        using var context = new AppDbContext();
        foreach (var shipment in context.Shipments.OrderBy(s => s.Id).ToList())
        {
            Shipments.Add(shipment);
        }

        foreach (var vehicle in context.Vehicles.OrderBy(v => v.Id).ToList())
        {
            Vehicles.Add(vehicle);
        }

        foreach (var driver in context.Users.Where(u => u.Role == UserRole.Driver).OrderBy(u => u.Id).ToList())
        {
            Drivers.Add(driver);
        }
    }
}
