using DataPrivacyTrix.SharedKernel.Emails;
using IdentityTrix.SharedKernel.DataPrivacy;

namespace DataPrivacyTrix.Domain.Emails.Root;

internal class EmailEntity(
    EmailId emailId,
    string emailAddress)
    : IEmailEntityProperties
{
    public EmailId EmailId { get; } = emailId;
    public string Email { get; } = emailAddress;

    internal EmailEntity(IEmailEntityProperties properties) : this(
        properties.EmailId,
        properties.Email
    ) { }
}
