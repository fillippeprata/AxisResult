using AxisTypes.SourceGenerator;

namespace Axis;

[ValueObject]
public readonly partial record struct TenantId
{
    public static TenantId New => new(Guid.CreateVersion7().ToString());

    private TenantId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "TenantId cannot be null");

        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid TenantId");

        if (guid.Version != 7)
            throw new ArgumentException($"'{value}' is not a valid TenantId (version 7 expected)");

        Value = value;
    }

    private string Value { get; }
}
