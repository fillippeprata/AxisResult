using DataPrivacyTrix.SharedKernel.AxisIdentities;
using AxisIdentityId = DataPrivacyTrix.SharedKernel.AxisIdentities.AxisIdentityId;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Domain.AxisIdentities.Root;

internal partial class AxisIdentityEntity(
    AxisIdentityId axisIdentityId,
    bool isIndividual,
    string document,
    CountryId countryId,
    string displayName,
    string defaultLanguage,
    SecurityLevel securityLevel)
    : IAxisIdentityEntityProperties
{
    public AxisIdentityId AxisIdentityId { get; } = axisIdentityId;
    public bool IsIndividual { get; } = isIndividual;
    public string Document { get; } = document;
    public CountryId CountryId { get; } = countryId;
    public string DisplayName { get; } = displayName;
    public string DefaultLanguage { get; } = defaultLanguage;
    public SecurityLevel SecurityLevel { get; } = securityLevel;

    internal AxisIdentityEntity(IAxisIdentityEntityProperties properties) : this(
        properties.AxisIdentityId,
        properties.IsIndividual,
        properties.Document,
        properties.CountryId,
        properties.DisplayName,
        properties.DefaultLanguage,
        properties.SecurityLevel
    ) { }
}
