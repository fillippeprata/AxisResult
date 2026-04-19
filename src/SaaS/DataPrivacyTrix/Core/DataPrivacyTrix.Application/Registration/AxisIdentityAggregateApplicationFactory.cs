using Axis;
using DataPrivacyTrix.Domain.Registration.Root;
using DataPrivacyTrix.Ports.Registration;
using DataPrivacyTrix.SharedKernel.Cellphones;
using DataPrivacyTrix.SharedKernel.Registration;
using CountryId = Axis.Localization.CountryId;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Application.Registration;

internal interface IAxisIdentityAggregateApplicationFactory
{
    Task<AxisResult<IAxisIdentityAggregateApplication>> GetByIdAsync(AxisIdentityId id);
    Task<AxisResult<IAxisIdentityAggregateApplication>> GetByCellphoneIdAsync(CellphoneId cellphoneId);
    Task<AxisResult<IAxisIdentityAggregateApplication>> GetByEmailIdAsync(EmailId emailId);
    Task<AxisResult<IAxisIdentityAggregateApplication>> CreateAsync(NewArgs args);

    public record NewArgs
    {
        public required bool IsIndividual { get; init; }
        public required string Document { get; init; }
        public required CountryId CountryId { get; init; }
        public required string DisplayName { get; init; }
        public required string DefaultLanguage { get; init; }
        public required SecurityLevel SecurityLevel { get; init; }
    }
}

internal class AxisIdentityAggregateApplicationFactory(
    IAxisIdentitiesReaderPort readerPort,
    IAxisIdentitiesWritePort writePort,
    IAxisIdentityCellphonesWritePort cellphonesWriter,
    IAxisIdentityEmailsWritePort emailsWriter
) : IAxisIdentityAggregateApplicationFactory
{
    private IAxisIdentityAggregateApplication NewInstance(IAxisIdentityEntityProperties properties)
        => new AxisIdentityAggregateApplication(properties, cellphonesWriter, emailsWriter);

    public Task<AxisResult<IAxisIdentityAggregateApplication>> GetByIdAsync(AxisIdentityId id)
        => readerPort.GetByIdAsync(id)
            .MapAsync(NewInstance);

    public Task<AxisResult<IAxisIdentityAggregateApplication>> GetByCellphoneIdAsync(CellphoneId cellphoneId)
        => readerPort.GetByCellphoneIdAsync(cellphoneId)
            .MapAsync(NewInstance);

    public Task<AxisResult<IAxisIdentityAggregateApplication>> GetByEmailIdAsync(EmailId emailId)
        => readerPort.GetByEmailIdAsync(emailId)
            .MapAsync(NewInstance);

    public Task<AxisResult<IAxisIdentityAggregateApplication>> CreateAsync(IAxisIdentityAggregateApplicationFactory.NewArgs args)
        => readerPort.GetByDocumentAsync(args.CountryId, args.Document)
            .RequireNotFoundAsync(AxisError.ValidationRule("DOCUMENT_ALREADY_REGISTERED"))
            .WithValueAsync(new AxisIdentityEntity(
                AxisIdentityId.New,
                args.IsIndividual,
                args.Document,
                args.CountryId,
                args.DisplayName,
                args.DefaultLanguage,
                args.SecurityLevel))
            .MapAsync(NewInstance)
            .ActionAsync(app => app.IsValidAsync())
            .ThenAsync(writePort.CreateAsync);
}
