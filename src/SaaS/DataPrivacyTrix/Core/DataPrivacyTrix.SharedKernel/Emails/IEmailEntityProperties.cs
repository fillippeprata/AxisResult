using IdentityTrix.SharedKernel.DataPrivacy;

namespace DataPrivacyTrix.SharedKernel.Emails;

public interface IEmailEntityProperties
{
    EmailId EmailId { get; }
    string Email { get;}
}
