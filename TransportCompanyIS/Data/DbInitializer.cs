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

        context.Users.AddRange(users);

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

        context.Vehicles.AddRange(vehicles);
        context.Shipments.AddRange(shipments);
        context.SaveChanges();

        var drivers = users.Where(u => u.Role == UserRole.Driver).ToList();
        var trips = new List<Trip>();
        for (var i = 0; i < 10; i++)
        {
            trips.Add(new Trip
            {
                ShipmentId = shipments[i].Id,
                VehicleId = vehicles[i].Id,
                DriverId = drivers[i].Id,
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
