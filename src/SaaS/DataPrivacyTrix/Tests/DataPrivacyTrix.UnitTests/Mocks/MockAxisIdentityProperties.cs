using DataPrivacyTrix.SharedKernel.AxisIdentities;
using AxisIdentityId = DataPrivacyTrix.SharedKernel.AxisIdentities.AxisIdentityId;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.UnitTests.Mocks;

internal record MockAxisIdentityProperties(
    AxisIdentityId AxisIdentityId,
    bool IsIndividual,
    string Document,
    CountryId CountryId,
    string DisplayName,
    string DefaultLanguage,
    SecurityLevel SecurityLevel) : IAxisIdentityEntityProperties;
