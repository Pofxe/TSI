using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;

namespace TransportCompanyIS.Data;

public static class DbInitializer
{
    public static void Initialize()
    {
        using var context = new AppDbContext();
        context.Database.EnsureCreated();

        if (context.Users.Any())
        {
            return;
        }

        var admin = new User
        {
            Login = "admin",
            PasswordHash = PasswordHasher.HashPassword("admin123"),
            Role = UserRole.Administrator
        };

        var dispatcher = new User
        {
            Login = "dispatcher",
            PasswordHash = PasswordHasher.HashPassword("dispatcher123"),
            Role = UserRole.Dispatcher
        };

        var driver = new User
        {
            Login = "driver",
            PasswordHash = PasswordHasher.HashPassword("driver123"),
            Role = UserRole.Driver
        };

        context.Users.AddRange(admin, dispatcher, driver);

        var vehicles = new List<Vehicle>
        {
            new() { PlateNumber = "A123BC", Model = "Газель Next", Status = "Свободен" },
            new() { PlateNumber = "B456CD", Model = "MAN TGS", Status = "В рейсе" },
            new() { PlateNumber = "C789EF", Model = "Volvo FH", Status = "На обслуживании" }
        };

        var shipments = new List<Shipment>
        {
            new() { FromAddress = "Москва, Тверская 1", ToAddress = "Санкт-Петербург, Невский 10", Status = "В пути" },
            new() { FromAddress = "Казань, Кремль 5", ToAddress = "Самара, Ленина 12", Status = "Запланировано" }
        };

        context.Vehicles.AddRange(vehicles);
        context.Shipments.AddRange(shipments);
        context.SaveChanges();

        var trip = new Trip
        {
            ShipmentId = shipments[0].Id,
            VehicleId = vehicles[1].Id,
            DriverId = driver.Id,
            Status = "В пути"
        };

        context.Trips.Add(trip);
        context.SaveChanges();
    }
}
