namespace AxisTrix.Types.Localization;

public static class CountryIds
{
    static CountryIds()
    {
        Br = new(nameof(Br));
        Us = new(nameof(Us));

        AllCountries  = new()
        {
            { nameof(Br).ToLowerInvariant(), Br },
            { nameof(Us).ToLowerInvariant(), Us },
        };
    }

    // public static CountryId? GetById(string? id) => AllCountries.TryGetValue(id?.ToLowerInvariant() ?? "", out var countryId) ? countryId : null;

    public static readonly CountryId Br;
    public static readonly CountryId Us;

    public static readonly Dictionary<string, CountryId> AllCountries;
}
