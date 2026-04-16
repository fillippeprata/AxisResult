using AxisValidator.FluentValidation;
using DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;

namespace DataPrivacyTrix.Application.Emails.UseCases.GetByEmailAddress.v1;

internal class GetByEmailAddressValidator : AxisValidatorBase<GetByEmailAddressQuery>
{
    public GetByEmailAddressValidator()
    {
        NotNullOrEmpty(x => x.Email, "EMAIL_ADDRESS_NULL_OR_EMPTY");
    }
}
