using Axis;
using DataPrivacyTrix.Contracts.Emails.v1.AddEmail;
using DataPrivacyTrix.Contracts.Emails.v1.GetEmailByAddress;

namespace DataPrivacyTrix.Contracts.Emails.v1;

public interface IEmailsMediator
{
    Task<AxisResult<AddEmailResponse>> AddAsync(AddEmailCommand command);
    Task<AxisResult<GetEmailByAddressResponse>> GetByEmailAddressAsync(GetEmailByAddressQuery query);
}
