using DataPrivacyTrix.SharedKernel.Emails;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Domain.Emails.Root;

internal class EmailEntity(
    EmailId emailId,
    string emailAddress)
    : IEmailEntityProperties
{
    public EmailId EmailId { get; } = emailId;
    public string EmailAddress { get; } = emailAddress;

    internal EmailEntity(IEmailEntityProperties properties) : this(
        properties.EmailId,
        properties.EmailAddress
    ) { }
}
