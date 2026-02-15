using Microsoft.EntityFrameworkCore;
using TransportCompanyIS.Models;
using TransportCompanyIS.Utils;

namespace TransportCompanyIS.Data;

public static class DbInitializer
{
    public static void Initialize()
    {
        using var context = new AppDbContext();

        EnsureCompatibleSchema(context);
        context.Database.EnsureCreated();

        SeedUsers(context);
        SeedVehicles(context);
        SeedShipments(context);
        SeedTrips(context);
    }

    private static void EnsureCompatibleSchema(AppDbContext context)
    {
        context.Database.EnsureCreated();
        try
        {
            context.Database.ExecuteSqlRaw("SELECT Description, AssignedVehicleId FROM Shipments LIMIT 1;");
            context.Database.ExecuteSqlRaw("SELECT FullName, PhoneNumber, DriverStatus FROM Users LIMIT 1;");
            context.Database.ExecuteSqlRaw("SELECT DriverId FROM Vehicles LIMIT 1;");
        }
        catch
        {
            context.Database.EnsureDeleted();
        }
    }

    private static void SeedUsers(AppDbContext context)
    {
        var users = new List<User>
        {
            new() { Login = "admin", PasswordHash = PasswordHasher.HashPassword("admin123"), Role = UserRole.Administrator, FullName = "Администратор Системы" },
            new() { Login = "dispatcher1", PasswordHash = PasswordHasher.HashPassword("dispatcher123"), Role = UserRole.Dispatcher, FullName = "Соколова Анна Викторовна" },
            new() { Login = "dispatcher2", PasswordHash = PasswordHasher.HashPassword("dispatcher123"), Role = UserRole.Dispatcher, FullName = "Миронов Илья Сергеевич" }
        };

        var driverNames = new[]
        {
            "Иванов Сергей Петрович", "Кузнецов Артем Игоревич", "Смирнов Павел Андреевич", "Орлов Денис Максимович", "Федоров Роман Алексеевич",
            "Егоров Никита Олегович", "Васильев Кирилл Дмитриевич", "Попов Михаил Юрьевич", "Зайцев Виталий Константинович", "Тарасов Иван Евгеньевич"
        };

        for (var i = 1; i <= 10; i++)
        {
            users.Add(new User
            {
                Login = $"driver{i}",
                PasswordHash = PasswordHasher.HashPassword($"driver{i}123"),
                Role = UserRole.Driver,
                FullName = driverNames[i - 1],
                PhoneNumber = $"+7 (900) 100-{i:00}-{(i + 10):00}",
                DriverStatus = i % 3 == 0 ? "Выходной" : i % 2 == 0 ? "В рейсе" : "Свободен"
            });
        }

        var existingUsers = context.Users.Select(u => u.Login).ToHashSet();
        context.Users.AddRange(users.Where(u => !existingUsers.Contains(u.Login)));
        context.SaveChanges();
    }

    private static void SeedVehicles(AppDbContext context)
    {
        var drivers = context.Users.Where(u => u.Role == UserRole.Driver).OrderBy(u => u.Id).ToList();
        var fleet = new List<Vehicle>
        {
            new() { PlateNumber = "A451BC", Model = "Mercedes-Benz Actros", Status = "Свободен", DriverId = drivers.ElementAtOrDefault(0)?.Id },
            new() { PlateNumber = "B229KM", Model = "Volvo FH16", Status = "В рейсе", DriverId = drivers.ElementAtOrDefault(1)?.Id },
            new() { PlateNumber = "C103OP", Model = "Scania R450", Status = "Свободен", DriverId = drivers.ElementAtOrDefault(2)?.Id },
            new() { PlateNumber = "E845TX", Model = "DAF XF", Status = "На обслуживании", DriverId = drivers.ElementAtOrDefault(3)?.Id },
            new() { PlateNumber = "H515AA", Model = "MAN TGX", Status = "В рейсе", DriverId = drivers.ElementAtOrDefault(4)?.Id },
            new() { PlateNumber = "K700MP", Model = "Iveco S-Way", Status = "Свободен", DriverId = drivers.ElementAtOrDefault(5)?.Id },
            new() { PlateNumber = "M318BT", Model = "КамАЗ 54901", Status = "Свободен", DriverId = drivers.ElementAtOrDefault(6)?.Id },
            new() { PlateNumber = "O912CT", Model = "Renault T High", Status = "На обслуживании", DriverId = drivers.ElementAtOrDefault(7)?.Id },
            new() { PlateNumber = "P640EK", Model = "Газон Next", Status = "В рейсе", DriverId = drivers.ElementAtOrDefault(8)?.Id },
            new() { PlateNumber = "T773PO", Model = "Isuzu Forward", Status = "Свободен", DriverId = drivers.ElementAtOrDefault(9)?.Id }
        };

        var existingPlates = context.Vehicles.Select(v => v.PlateNumber).ToHashSet();
        context.Vehicles.AddRange(fleet.Where(v => !existingPlates.Contains(v.PlateNumber)));
        context.SaveChanges();
    }

    private static void SeedShipments(AppDbContext context)
    {
        var vehicles = context.Vehicles.OrderBy(v => v.Id).ToList();
        var shipments = new List<Shipment>
        {
            new() { FromAddress = "Москва, Склад Южный", ToAddress = "Тула, Логистический центр", Status = "В пути", Description = "Стройматериалы: кирпич и сухие смеси", AssignedVehicleId = vehicles.ElementAtOrDefault(0)?.Id },
            new() { FromAddress = "Санкт-Петербург, Порт", ToAddress = "Великий Новгород, Склад №2", Status = "Запланировано", Description = "Контейнер с бытовой техникой", AssignedVehicleId = vehicles.ElementAtOrDefault(1)?.Id },
            new() { FromAddress = "Казань, Технопарк", ToAddress = "Самара, Промзона", Status = "Доставлено", Description = "Комплектующие для производства", AssignedVehicleId = vehicles.ElementAtOrDefault(2)?.Id },
            new() { FromAddress = "Екатеринбург, Склад Восток", ToAddress = "Пермь, База ПМ", Status = "В пути", Description = "Паллеты с лакокрасочной продукцией", AssignedVehicleId = vehicles.ElementAtOrDefault(3)?.Id },
            new() { FromAddress = "Нижний Новгород, Терминал", ToAddress = "Ярославль, Склад", Status = "Запланировано", Description = "Автозапчасти", AssignedVehicleId = vehicles.ElementAtOrDefault(4)?.Id },
            new() { FromAddress = "Ростов-на-Дону, Хаб", ToAddress = "Краснодар, ТЦ Южный", Status = "В пути", Description = "Партия продуктов питания", AssignedVehicleId = vehicles.ElementAtOrDefault(5)?.Id },
            new() { FromAddress = "Уфа, Нефтебаза", ToAddress = "Челябинск, Индустриальный парк", Status = "Запланировано", Description = "Технические масла в бочках", AssignedVehicleId = vehicles.ElementAtOrDefault(6)?.Id },
            new() { FromAddress = "Новосибирск, Склад Север", ToAddress = "Омск, Склад Гранд", Status = "Доставлено", Description = "Электроинструмент", AssignedVehicleId = vehicles.ElementAtOrDefault(7)?.Id },
            new() { FromAddress = "Воронеж, Распределительный центр", ToAddress = "Липецк, Сервисный центр", Status = "В пути", Description = "Бытовая химия", AssignedVehicleId = vehicles.ElementAtOrDefault(8)?.Id },
            new() { FromAddress = "Сочи, Морской порт", ToAddress = "Ставрополь, Склад", Status = "Запланировано", Description = "Офисная мебель", AssignedVehicleId = vehicles.ElementAtOrDefault(9)?.Id },
            new() { FromAddress = "Калуга, Завод", ToAddress = "Рязань, Автокластер", Status = "Запланировано", Description = "Металлические профили", AssignedVehicleId = vehicles.ElementAtOrDefault(0)?.Id },
            new() { FromAddress = "Ижевск, Склад", ToAddress = "Киров, Торговая база", Status = "В пути", Description = "Сантехника", AssignedVehicleId = vehicles.ElementAtOrDefault(1)?.Id },
            new() { FromAddress = "Тверь, Логоцентр", ToAddress = "Смоленск, РЦ", Status = "Доставлено", Description = "Канцелярские товары", AssignedVehicleId = vehicles.ElementAtOrDefault(2)?.Id },
            new() { FromAddress = "Брянск, Холодильный склад", ToAddress = "Орел, Рынок", Status = "В пути", Description = "Охлажденная продукция", AssignedVehicleId = vehicles.ElementAtOrDefault(3)?.Id },
            new() { FromAddress = "Пенза, Фармацентр", ToAddress = "Саратов, Аптечный склад", Status = "Запланировано", Description = "Медицинские товары", AssignedVehicleId = vehicles.ElementAtOrDefault(4)?.Id },
            new() { FromAddress = "Белгород, Склад", ToAddress = "Курск, Логоцентр", Status = "Доставлено", Description = "Корма для животных", AssignedVehicleId = vehicles.ElementAtOrDefault(5)?.Id },
            new() { FromAddress = "Архангельск, Порт", ToAddress = "Вологда, Склад", Status = "В пути", Description = "Лесоматериалы", AssignedVehicleId = vehicles.ElementAtOrDefault(6)?.Id },
            new() { FromAddress = "Тюмень, База", ToAddress = "Курган, Склад", Status = "Запланировано", Description = "Трубы и фитинги", AssignedVehicleId = vehicles.ElementAtOrDefault(7)?.Id },
            new() { FromAddress = "Мурманск, Порт", ToAddress = "Петрозаводск, РЦ", Status = "В пути", Description = "Рыбная продукция", AssignedVehicleId = vehicles.ElementAtOrDefault(8)?.Id },
            new() { FromAddress = "Владимир, Производство", ToAddress = "Иваново, Склад", Status = "Доставлено", Description = "Текстильная продукция", AssignedVehicleId = vehicles.ElementAtOrDefault(9)?.Id }
        };

        var existingKeys = context.Shipments.Select(s => s.FromAddress + "->" + s.ToAddress).ToHashSet();
        context.Shipments.AddRange(shipments.Where(s => !existingKeys.Contains(s.FromAddress + "->" + s.ToAddress)));
        context.SaveChanges();
    }

    private static void SeedTrips(AppDbContext context)
    {
        var drivers = context.Users.Where(u => u.Role == UserRole.Driver).OrderBy(u => u.Id).ToList();
        var shipments = context.Shipments.OrderBy(s => s.Id).Take(10).ToList();
        var vehicles = context.Vehicles.OrderBy(v => v.Id).Take(10).ToList();
        var existingTripPairs = context.Trips.Select(t => new { t.ShipmentId, t.VehicleId, t.DriverId }).ToList();

        var trips = new List<Trip>();
        for (var i = 0; i < 10 && i < drivers.Count && i < shipments.Count && i < vehicles.Count; i++)
        {
            if (existingTripPairs.Any(t => t.ShipmentId == shipments[i].Id && t.VehicleId == vehicles[i].Id && t.DriverId == drivers[i].Id))
            {
                continue;
            }

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
