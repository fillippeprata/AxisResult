using Axis;
using DataPrivacyTrix.Domain.Emails.Root;
using DataPrivacyTrix.Ports.Emails;
using DataPrivacyTrix.SharedKernel.Emails;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Application.Emails;

internal interface IEmailAggregateApplicationFactory
{
    Task<AxisResult<IEmailAggregateApplication>> GetByIdAsync(EmailId id);
    Task<AxisResult<IEmailAggregateApplication>> GetByEmailAddressAsync(string emailAddress);
    Task<AxisResult<IEmailAggregateApplication>> CreateAsync(NewArgs args);

    public record NewArgs
    {
        public required string EmailAddress { get; init; }
    }
}

internal class EmailAggregateApplicationFactory(
    IEmailsReaderPort readerPort,
    IEmailsWritePort writePort
) : IEmailAggregateApplicationFactory
{
    private static IEmailAggregateApplication NewInstance(IEmailEntityProperties properties)
        => new EmailAggregateApplication(properties);

    public Task<AxisResult<IEmailAggregateApplication>> GetByIdAsync(EmailId id)
        => readerPort.GetByIdAsync(id)
            .MapAsync(NewInstance);

    public Task<AxisResult<IEmailAggregateApplication>> GetByEmailAddressAsync(string emailAddress)
        =>  readerPort.GetByEmailAddressAsync(emailAddress)
            .MapAsync(NewInstance);

    public Task<AxisResult<IEmailAggregateApplication>> CreateAsync(IEmailAggregateApplicationFactory.NewArgs args)
        => GetByEmailAddressAsync(args.EmailAddress)
            .RequireNotFoundAsync(AxisError.ValidationRule("EMAIL_ALREADY_EXISTS"))
            .WithValueAsync(new EmailEntity(EmailId.New, args.EmailAddress))
            .MapAsync(NewInstance)
            .ThenAsync(writePort.CreateAsync);
}








