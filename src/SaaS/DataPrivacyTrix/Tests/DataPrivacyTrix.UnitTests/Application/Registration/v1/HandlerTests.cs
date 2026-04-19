using Axis;
using DataPrivacyTrix.Contracts.Registration.v1;
using DataPrivacyTrix.Contracts.Registration.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityById;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.SharedData;
using DataPrivacyTrix.SharedKernel.Cellphones;
using DataPrivacyTrix.SharedKernel.Registration;
using DataPrivacyTrix.UnitTests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.UnitTests.Application.Registration.v1;

public class HandlerTests
{
    private static IRegistrationMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IRegistrationMediator>();

    [Fact]
    public async Task RegisterByCellphoneShouldSucceedForValidDataAsync()
    {
        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        using var scope = services.GetServiceProvider().CreateScope();

        var command = new RegisterAxisIdentityByCellphoneCommand
        {
            Data = ValidData(),
            CellphoneId = CellphoneId.New.ToString()
        };

        var result = await Mediator(scope).RegisterByCellphoneAsync(command);

        Assert.True(result.IsSuccess, $"Failed: {string.Join("; ", result.Errors.Select(e => e.Code))}");
        Assert.NotEmpty(result.Value.AxisIdentityId);
        mocks.AxisIdentitiesWriter.Verify(x => x.CreateAsync(It.IsAny<IAxisIdentityEntityProperties>()), Times.Once);
        mocks.AxisIdentityCellphonesWriter.Verify(
            x => x.AddCellphoneAsync(It.IsAny<AxisIdentityId>(), It.IsAny<CellphoneId>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterByCellphoneShouldFailWhenDocumentAlreadyRegisteredAsync()
    {
        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        mocks.AxisIdentitiesReader.Setup(x => x.GetByDocumentAsync(It.IsAny<CountryId>(), It.IsAny<string>()))
            .ReturnsAsync(AxisResult.Ok<IAxisIdentityEntityProperties>(new MockAxisIdentityProperties(
                AxisIdentityId.New, true, "39053344705", "BR", "Existing", "pt-BR", SecurityLevel.Normal)));
        using var scope = services.GetServiceProvider().CreateScope();

        var command = new RegisterAxisIdentityByCellphoneCommand
        {
            Data = ValidData(),
            CellphoneId = CellphoneId.New.ToString()
        };

        var result = await Mediator(scope).RegisterByCellphoneAsync(command);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "DOCUMENT_ALREADY_REGISTERED");
        mocks.AxisIdentitiesWriter.Verify(x => x.CreateAsync(It.IsAny<IAxisIdentityEntityProperties>()), Times.Never);
    }

    [Fact]
    public async Task AddCellphoneShouldSucceedWhenIdentityExistsAsync()
    {
        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        var identity = new MockAxisIdentityProperties(AxisIdentityId.New, true, "39053344705", "BR", "User", "pt-BR", SecurityLevel.Normal);
        mocks.AxisIdentitiesReader.Setup(x => x.GetByIdAsync(It.IsAny<AxisIdentityId>()))
            .ReturnsAsync(AxisResult.Ok<IAxisIdentityEntityProperties>(identity));
        using var scope = services.GetServiceProvider().CreateScope();

        var command = new AddCellphoneToAxisIdentityCommand
        {
            AxisIdentityId = identity.AxisIdentityId.ToString(),
            CellphoneId = CellphoneId.New.ToString()
        };

        var result = await Mediator(scope).AddCellphoneAsync(command);

        Assert.True(result.IsSuccess, $"Failed: {string.Join("; ", result.Errors.Select(e => e.Code))}");
        mocks.AxisIdentityCellphonesWriter.Verify(
            x => x.AddCellphoneAsync(It.IsAny<AxisIdentityId>(), It.IsAny<CellphoneId>()),
            Times.Once);
    }

    [Fact]
    public async Task AddCellphoneShouldFailWhenIdentityNotFoundAsync()
    {
        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        mocks.AxisIdentitiesReader.Setup(x => x.GetByIdAsync(It.IsAny<AxisIdentityId>()))
            .ReturnsAsync(AxisResult.Error<IAxisIdentityEntityProperties>(AxisError.NotFound("AXIS_IDENTITY_NOT_FOUND")));
        using var scope = services.GetServiceProvider().CreateScope();

        var command = new AddCellphoneToAxisIdentityCommand
        {
            AxisIdentityId = AxisIdentityId.New.ToString(),
            CellphoneId = CellphoneId.New.ToString()
        };

        var result = await Mediator(scope).AddCellphoneAsync(command);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "AXIS_IDENTITY_NOT_FOUND");
    }

    [Fact]
    public async Task GetByIdShouldReturnEntityWhenFoundAsync()
    {
        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        var identity = new MockAxisIdentityProperties(AxisIdentityId.New, true, "39053344705", "BR", "User", "pt-BR", SecurityLevel.Advanced);
        mocks.AxisIdentitiesReader.Setup(x => x.GetByIdAsync(identity.AxisIdentityId))
            .ReturnsAsync(AxisResult.Ok<IAxisIdentityEntityProperties>(identity));
        using var scope = services.GetServiceProvider().CreateScope();

        var result = await Mediator(scope).GetByIdAsync(new GetAxisIdentityByIdQuery
        {
            AxisIdentityId = identity.AxisIdentityId.ToString()
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(identity.AxisIdentityId.ToString(), result.Value.AxisIdentityId);
        Assert.Equal("User", result.Value.DisplayName);
        Assert.Equal("Advanced", result.Value.SecurityLevel);
    }

    private static RegisterAxisIdentityData ValidData() => new()
    {
        IsIndividual = true,
        Document = "39053344705",
        CountryId = "BR",
        DisplayName = "Test User",
        DefaultLanguage = "pt-BR",
        SecurityLevel = "Normal"
    };
}
