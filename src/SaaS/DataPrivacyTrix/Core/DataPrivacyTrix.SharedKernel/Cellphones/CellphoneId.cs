
using AxisTypes.SourceGenerator;

namespace DataPrivacyTrix.SharedKernel.Cellphones;

[ValueObject]
public readonly partial record struct CellphoneId
{
    public static CellphoneId New => new(Guid.CreateVersion7().ToString());

    private CellphoneId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CellphoneId cannot be null or empty");

        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid CellphoneId");

        if (guid.Version != 7)
            throw new ArgumentException($"'{value}' is not a valid CellphoneId (version 7 expected)");

        Value = value;
    }

    private string Value { get; }
}
