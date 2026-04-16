using AxisValidator.FluentValidation;
using DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;

namespace DataPrivacyTrix.Application.Emails.UseCases.GetByEmailAddress.v1;

public class GetByEmailAddressValidator : AxisValidatorBase<GetByEmailAddressQuery>
{
    public GetByEmailAddressValidator()
    {
        NotNullOrEmpty(x => x.Email, "EMAIL_ADDRESS_NULL_OR_EMPTY");
    }
}
