namespace TransportCompanyIS.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DriverStatus { get; set; } = string.Empty;

    public string RoleDisplay => Utils.RoleHelper.GetRoleName(Role);
    public string DriverShortName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return Login;
            }

            var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return FullName;
            }

            var firstInitial = parts.Length > 1 ? $" {parts[1][0]}." : string.Empty;
            var middleInitial = parts.Length > 2 ? $"{parts[2][0]}." : string.Empty;
            return $"{parts[0]}{firstInitial}{middleInitial}";
        }
    }
}

public enum UserRole
{
    Administrator,
    Dispatcher,
    Driver
}
