using Axis;
using DataPrivacyTrix.SharedKernel.Emails;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Ports.Emails;

public interface IEmailsReaderPort
{
    Task<AxisResult<IEmailEntityProperties>> GetByIdAsync(EmailId emailId);
    Task<AxisResult<IEmailEntityProperties>> GetByEmailAddressAsync(string emailAddress);
}
