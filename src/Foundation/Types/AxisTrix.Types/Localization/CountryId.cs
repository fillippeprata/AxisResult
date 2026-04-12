using AxisTrix.SourceGen;

namespace AxisTrix.Types;

[ValueObject]
public readonly partial record struct CountryId
{
    public string Value { get; }

    //todo: Cannot validate via BadRequest because the CountryIds dictionary is not yet initialized at construction time — consider lazy validation
    public CountryId(string? value)
    {
        value = value?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CountryId cannot be null or empty");
        Value = value;
    }
}
