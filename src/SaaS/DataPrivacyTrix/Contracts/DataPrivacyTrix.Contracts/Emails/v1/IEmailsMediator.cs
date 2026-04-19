using Axis;
using DataPrivacyTrix.Contracts.Emails.v1.AddEmail;
using DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;

namespace DataPrivacyTrix.Contracts.Emails.v1;

public interface IEmailsMediator
{
    Task<AxisResult<AddEmailResponse>> AddAsync(AddEmailCommand command);
    Task<AxisResult<GetByEmailAddressResponse>> GetByEmailAddressAsync(GetByEmailAddressQuery query);
}
