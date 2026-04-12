using AxisTrix.SourceGen;

namespace DataPrivacyTrix.SharedKernel.Emails;

[ValueObject(UseInvariantCulture = true)]
public readonly partial record struct EmailId
{
    public static EmailId New => new(Guid.CreateVersion7().ToString());

    private EmailId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("EmailId cannot be null or empty");

        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid EmailId");

        if (guid.Version != 7)
            throw new ArgumentException($"'{value}' is not a valid EmailId (version 7 expected)");

        Value = value;
    }

    private string Value { get; }
}
