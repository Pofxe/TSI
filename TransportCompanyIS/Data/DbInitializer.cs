using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;

namespace TransportCompanyIS.Data;

public static class DbInitializer
{
    public static void Initialize()
    {
        using var context = new AppDbContext();
        context.Database.EnsureCreated();

        var admin = new User
        {
            Login = "admin",
            PasswordHash = PasswordHasher.HashPassword("admin123"),
            Role = UserRole.Administrator
        };

        var users = new List<User> { admin };
        users.AddRange(new[]
        {
            new User { Login = "dispatcher1", PasswordHash = PasswordHasher.HashPassword("dispatcher123"), Role = UserRole.Dispatcher },
            new User { Login = "dispatcher2", PasswordHash = PasswordHasher.HashPassword("dispatcher123"), Role = UserRole.Dispatcher }
        });

        for (var i = 1; i <= 10; i++)
        {
            users.Add(new User
            {
                Login = $"driver{i}",
                PasswordHash = PasswordHasher.HashPassword($"driver{i}123"),
                Role = UserRole.Driver
            });
        }

        var existingUsers = context.Users.Select(u => u.Login).ToHashSet();
        context.Users.AddRange(users.Where(u => !existingUsers.Contains(u.Login)));

        var vehicles = new List<Vehicle>();
        for (var i = 1; i <= 10; i++)
        {
            vehicles.Add(new Vehicle
            {
                PlateNumber = $"A{i:000}BC",
                Model = $"Грузовик №{i}",
                Status = i % 3 switch
                {
                    0 => "На обслуживании",
                    1 => "Свободен",
                    _ => "В рейсе"
                }
            });
        }

        var shipments = new List<Shipment>();
        for (var i = 1; i <= 20; i++)
        {
            shipments.Add(new Shipment
            {
                FromAddress = $"Город отправки {i}",
                ToAddress = $"Город доставки {i}",
                Status = i % 3 switch
                {
                    0 => "Доставлено",
                    1 => "Запланировано",
                    _ => "В пути"
                }
            });
        }

        var existingVehicles = context.Vehicles.Select(v => v.PlateNumber).ToHashSet();
        var existingShipments = context.Shipments.Select(s => s.FromAddress).ToHashSet();
        context.Vehicles.AddRange(vehicles.Where(v => !existingVehicles.Contains(v.PlateNumber)));
        context.Shipments.AddRange(shipments.Where(s => !existingShipments.Contains(s.FromAddress)));
        context.SaveChanges();

        var drivers = context.Users.Where(u => u.Role == UserRole.Driver).OrderBy(u => u.Id).ToList();
        var currentVehicles = context.Vehicles.OrderBy(v => v.Id).ToList();
        var currentShipments = context.Shipments.OrderBy(s => s.Id).ToList();
        var existingTripPairs = context.Trips.Select(t => new { t.ShipmentId, t.VehicleId, t.DriverId }).ToList();
        var trips = new List<Trip>();
        for (var i = 0; i < 10 && i < drivers.Count && i < currentVehicles.Count && i < currentShipments.Count; i++)
        {
            var shipmentId = currentShipments[i].Id;
            var vehicleId = currentVehicles[i].Id;
            var driverId = drivers[i].Id;
            if (existingTripPairs.Any(t => t.ShipmentId == shipmentId && t.VehicleId == vehicleId && t.DriverId == driverId))
            {
                continue;
            }

            trips.Add(new Trip
            {
                ShipmentId = shipmentId,
                VehicleId = vehicleId,
                DriverId = driverId,
                Status = i % 3 switch
                {
                    0 => "Завершен",
                    1 => "Запланирован",
                    _ => "В пути"
                }
            });
        }

        context.Trips.AddRange(trips);
        context.SaveChanges();
    }
}
