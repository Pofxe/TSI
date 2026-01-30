using TransportCompanyIS.Models;

namespace TransportCompanyIS.Utils;

public static class RoleHelper
{
    public static string GetRoleName(UserRole role)
    {
        return role switch
        {
            UserRole.Administrator => "Администратор",
            UserRole.Dispatcher => "Диспетчер",
            UserRole.Driver => "Водитель",
            _ => role.ToString()
        };
    }
}
