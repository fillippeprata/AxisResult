using System.Security.Cryptography;
using System.Text;
using Axis.Localization;
using AxisTypes.SourceGenerator;

namespace Axis;

[ValueObject]
public readonly partial record struct AxisIdentity
{
    public static AxisIdentity New(bool isIndividual) => new($"{(isIndividual ? 1 : 0 )}|{Guid.CreateVersion7()}");

    private AxisIdentity(string? value)
    {
        var msg = $"'{value}' is not a valid AxisIdentity";
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "AxisIdentity cannot be empty");

        var split = value.Split('|');
        if(split.Length != 2 || split[0] != "0" &&  split[0] != "1" || !Guid.TryParse(split[1], out var guid) || guid.Version != 7)
            throw new ArgumentException(msg);

        var isIndividual =  split[0] == "1";
        if (isIndividual)
        {
            IsIndividual = true;
            IsLegalEntity = false;
        }
        else
        {
            IsIndividual = false;
            IsLegalEntity = true;
        }

        Value = value;
    }

    public static string Hash(CountryId countryId, string documentId)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(countryId + documentId));
        return Convert.ToHexStringLower(bytes).Replace("-", string.Empty);
    }

    private string Value { get; }
    public bool IsIndividual { get; }
    public bool IsLegalEntity { get; }
}
