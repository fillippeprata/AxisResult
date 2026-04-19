using DataPrivacyTrix.Domain.Emails.Root;
using DataPrivacyTrix.SharedKernel.Emails;

namespace DataPrivacyTrix.Application.Emails;

internal interface IEmailAggregateApplication : IEmailEntityProperties;

internal class EmailAggregateApplication(
    IEmailEntityProperties properties
) : EmailEntity(properties), IEmailAggregateApplication;
