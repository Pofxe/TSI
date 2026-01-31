namespace TransportCompanyIS.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public string RoleDisplay => Utils.RoleHelper.GetRoleName(Role);
}

public enum UserRole
{
    Administrator,
    Dispatcher,
    Driver
}
