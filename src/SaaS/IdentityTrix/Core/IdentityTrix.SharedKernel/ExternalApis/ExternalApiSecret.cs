using System.Security.Cryptography;
using System.Text;

namespace IdentityTrix.SharedKernel.ExternalApis;

public static class ExternalApiSecret
{
    public static string Generate()
        => $"{Guid.NewGuid()}-{Guid.NewGuid()}";

    public static string Hash(string plainSecret)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainSecret));
        return Convert.ToHexStringLower(bytes);
    }

    public static bool Verify(string plainSecret, string hashedSecret)
        => string.Equals(Hash(plainSecret), hashedSecret, StringComparison.OrdinalIgnoreCase);
}
