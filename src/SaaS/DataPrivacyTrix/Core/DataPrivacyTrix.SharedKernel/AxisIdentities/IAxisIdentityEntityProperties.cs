using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.SharedKernel.AxisIdentities;

public interface IAxisIdentityEntityProperties
{
    AxisIdentityId AxisIdentityId { get; }
    bool IsIndividual { get; }
    string Document { get; }
    CountryId CountryId { get; }
    string DisplayName { get; }
    string DefaultLanguage { get; }
    SecurityLevel SecurityLevel { get; }
}
