using AxisTypes.SourceGenerator;

namespace TenantTrix.SharedKernel.ExternalApis;

[ValueObject]
public readonly partial record struct ExternalApiId
{
    public static ExternalApiId New => new(Guid.CreateVersion7().ToString());
    private ExternalApiId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ExternalApiId cannot be null or empty");

        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid ExternalApiId");

        if (guid.Version != 7)
            throw new ArgumentException($"'{value}' is not a valid ExternalApiId (version 7 expected)");

        Value = value;
    }

    private string Value { get; }
}
