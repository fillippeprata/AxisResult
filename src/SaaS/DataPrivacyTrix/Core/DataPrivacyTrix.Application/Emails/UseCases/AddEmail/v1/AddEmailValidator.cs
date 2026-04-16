using AxisValidator.FluentValidation;
using DataPrivacyTrix.Contracts.Emails.v1.AddEmail;

namespace DataPrivacyTrix.Application.Emails.UseCases.AddEmail.v1;

public class AddEmailValidator : AxisValidatorBase<AddEmailCommand>
{
    public AddEmailValidator()
    {
        RequiredEmail(x => x.EmailAddress, "EMAIL_NULL_OR_NOT_VALID");
    }
}
