using Axis;
using DataPrivacyTrix.SharedKernel.AxisIdentities;
using DataPrivacyTrix.SharedKernel.Cellphones;
using AxisIdentityId = DataPrivacyTrix.SharedKernel.AxisIdentities.AxisIdentityId;
using CountryId = Axis.Localization.CountryId;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Ports.AxisIdentities;

public interface IAxisIdentitiesReaderPort
{
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByIdAsync(AxisIdentityId axisIdentityId);
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByDocumentAsync(CountryId countryId, string document);
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByCellphoneIdAsync(CellphoneId cellphoneId);
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByEmailIdAsync(EmailId emailId);
}
