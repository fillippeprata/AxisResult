using AxisTypes.SourceGenerator;

namespace DataPrivacyTrix.SharedKernel.Registration;

[ValueObject]
public readonly partial record struct AxisIdentityId
{
    public static AxisIdentityId New => new(Guid.CreateVersion7().ToString());

    private AxisIdentityId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("AxisIdentityId cannot be null or empty");

        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid AxisIdentityId");

        if (guid.Version != 7)
            throw new ArgumentException($"'{value}' is not a valid AxisIdentityId (version 7 expected)");

        Value = value;
    }

    private string Value { get; }
}
