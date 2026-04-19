using AxisValidator;
using DataPrivacyTrix.Contracts.Emails.v1.GetEmailByAddress;

namespace DataPrivacyTrix.Application.Emails.UseCases.GetEmailByAddress.v1;

internal class GetEmailByAddressValidator : AxisValidatorBase<GetEmailByAddressQuery>
{
    public GetEmailByAddressValidator()
    {
        NotNullOrEmpty(x => x.Email, "EMAIL_ADDRESS_NULL_OR_EMPTY");
    }
}
