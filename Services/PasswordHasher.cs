using System.Security.Cryptography;
using System.Text;

namespace DziennikPodrozy.Services;

public static class PasswordHasher
{
    public static string Hash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.ASCII.GetBytes(input);
        return Convert.ToHexString(md5.ComputeHash(bytes));
    }
}
