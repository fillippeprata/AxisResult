using Axis;
using DataPrivacyTrix.Contracts.Cellphones.v1;
using DataPrivacyTrix.Contracts.Emails.v1;
using DataPrivacyTrix.Ports;
using DataPrivacyTrix.Ports.Cellphones;
using DataPrivacyTrix.Ports.Emails;
using DataPrivacyTrix.Ports.Registration;
using DataPrivacyTrix.SharedKernel.Cellphones;
using DataPrivacyTrix.SharedKernel.Emails;
using DataPrivacyTrix.SharedKernel.Registration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DataPrivacyTrix.UnitTests.Mocks;

internal record DataPrivacyTrixMocks
{
    public static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddMocks(CreateSuccessfulMocks());
        return services.GetServiceProvider();
    }

    public static DataPrivacyTrixMocks CreateSuccessfulMocks()
    {
        var mocks = new DataPrivacyTrixMocks();

        mocks.UowProvider.Setup(x => x.UnitOfWork.SaveChangesAsync())
            .ReturnsAsync(AxisResult.Ok());

        mocks.AxisIdentitiesReader.Setup(x => x.GetByDocumentAsync(It.IsAny<Axis.Localization.CountryId>(), It.IsAny<string>()))
            .ReturnsAsync(AxisResult.Error<IAxisIdentityEntityProperties>(AxisError.NotFound("AXIS_IDENTITY_NOT_FOUND")));

        mocks.AxisIdentitiesWriter.Setup(x => x.CreateAsync(It.IsAny<IAxisIdentityEntityProperties>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.AxisIdentityCellphonesWriter.Setup(x => x.AddCellphoneAsync(It.IsAny<AxisIdentityId>(), It.IsAny<CellphoneId>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.AxisIdentityEmailsWriter.Setup(x => x.AddEmailAsync(It.IsAny<AxisIdentityId>(), It.IsAny<EmailId>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.CellphonesReader.Setup(x => x.GetByIdAsync(It.IsAny<CellphoneId>()))
            .ReturnsAsync(AxisResult.Error<ICellphoneEntityProperties>(AxisError.NotFound("CELLPHONE_NOT_FOUND")));

        mocks.CellphonesWriter.Setup(x => x.CreateAsync(It.IsAny<ICellphoneEntityProperties>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.EmailsReader.Setup(x => x.GetByIdAsync(It.IsAny<EmailId>()))
            .ReturnsAsync(AxisResult.Error<IEmailEntityProperties>(AxisError.NotFound("EMAIL_NOT_FOUND")));

        mocks.EmailsWriter.Setup(x => x.CreateAsync(It.IsAny<IEmailEntityProperties>()))
            .ReturnsAsync(AxisResult.Ok());

        return mocks;
    }

    public Mock<IUnitOfWorkProvider> UowProvider { get; init; } = new();
    public Mock<IAxisIdentitiesReaderPort> AxisIdentitiesReader { get; init; } = new();
    public Mock<IAxisIdentitiesWritePort> AxisIdentitiesWriter { get; init; } = new();
    public Mock<IAxisIdentityCellphonesWritePort> AxisIdentityCellphonesWriter { get; init; } = new();
    public Mock<IAxisIdentityEmailsWritePort> AxisIdentityEmailsWriter { get; init; } = new();
    public Mock<ICellphonesReaderPort> CellphonesReader { get; init; } = new();
    public Mock<ICellphonesWritePort> CellphonesWriter { get; init; } = new();
    public Mock<IEmailsReaderPort> EmailsReader { get; init; } = new();
    public Mock<IEmailsWritePort> EmailsWriter { get; init; } = new();
    public Mock<ICellphonesMediator> CellphonesMediator { get; init; } = new();
    public Mock<IEmailsMediator> EmailsMediator { get; init; } = new();
}
