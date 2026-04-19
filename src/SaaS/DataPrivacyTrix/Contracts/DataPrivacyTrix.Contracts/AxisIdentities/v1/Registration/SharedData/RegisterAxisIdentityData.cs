namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.SharedData;

public record RegisterAxisIdentityData
{
    public bool? IsIndividual { get; init; }
    public string? Document { get; init; }
    public string? CountryId { get; init; }
    public string? DisplayName { get; init; }
    public string? DefaultLanguage { get; init; }
    public string? SecurityLevel { get; init; }
}
