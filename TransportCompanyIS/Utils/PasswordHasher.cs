using System.Security.Cryptography;
using System.Text;

namespace TransportCompanyIS.Utils;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var salt = "transport-company-salt";
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
