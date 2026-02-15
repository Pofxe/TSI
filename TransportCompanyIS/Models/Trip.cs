namespace TransportCompanyIS.Models;

public class Trip
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public int VehicleId { get; set; }
    public int DriverId { get; set; }
    public string Status { get; set; } = string.Empty;

    public Shipment? Shipment { get; set; }
    public Vehicle? Vehicle { get; set; }
    public User? Driver { get; set; }
}
