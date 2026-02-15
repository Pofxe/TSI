namespace TransportCompanyIS.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? DriverId { get; set; }
    public User? Driver { get; set; }

    public string DriverDisplay => Driver?.FullName ?? "Не закреплен";
}
