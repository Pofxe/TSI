using System.Collections.ObjectModel;
using TransportCompanyIS.Data;
using TransportCompanyIS.Models;

namespace TransportCompanyIS.ViewModels;

public class ShipmentFormViewModel : ViewModelBase
{
    public Shipment Shipment { get; }

    public ObservableCollection<Vehicle> Vehicles { get; } = new();

    public ObservableCollection<string> Statuses { get; } = new()
    {
        "Запланировано",
        "В пути",
        "Доставлено"
    };

    public ShipmentFormViewModel(Shipment shipment)
    {
        Shipment = shipment;
        LoadVehicles();
        if (string.IsNullOrWhiteSpace(Shipment.Status))
        {
            Shipment.Status = Statuses[0];
        }
    }

    private void LoadVehicles()
    {
        using var context = new AppDbContext();
        Vehicles.Add(new Vehicle { Id = 0, PlateNumber = "-", Model = "Не назначен" });
        foreach (var vehicle in context.Vehicles.OrderBy(v => v.PlateNumber).ToList())
        {
            Vehicles.Add(vehicle);
        }
    }
}
