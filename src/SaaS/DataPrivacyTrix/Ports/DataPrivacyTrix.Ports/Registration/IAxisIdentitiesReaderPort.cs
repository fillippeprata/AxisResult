using Axis;
using DataPrivacyTrix.SharedKernel.Cellphones;
using DataPrivacyTrix.SharedKernel.Emails;
using DataPrivacyTrix.SharedKernel.Registration;
using CountryId = Axis.Localization.CountryId;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Ports.Registration;

public interface IAxisIdentitiesReaderPort
{
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByIdAsync(AxisIdentityId axisIdentityId);
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByDocumentAsync(CountryId countryId, string document);
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByCellphoneIdAsync(CellphoneId cellphoneId);
    Task<AxisResult<IAxisIdentityEntityProperties>> GetByEmailIdAsync(EmailId emailId);
}
