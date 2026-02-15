namespace TransportCompanyIS.Models;

public class Shipment
{
    public int Id { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? AssignedVehicleId { get; set; }
    public Vehicle? AssignedVehicle { get; set; }

    public string VehicleDisplay => AssignedVehicle == null ? "Не назначен" : $"{AssignedVehicle.PlateNumber} ({AssignedVehicle.Model})";
}
