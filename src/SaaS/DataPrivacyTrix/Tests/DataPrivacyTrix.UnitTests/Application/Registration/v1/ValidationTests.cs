using Axis;
using DataPrivacyTrix.Contracts.AxisIdentities.v1;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityById;
using DataPrivacyTrix.Contracts.Registration.v1;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.Registration.v1.SharedData;
using DataPrivacyTrix.SharedKernel.Cellphones;
using DataPrivacyTrix.UnitTests.Mocks;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.UnitTests.Application.Registration.v1;

public class ValidationTests
{
    private static IRegistrationMediator RegistrationMediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IRegistrationMediator>();

    private static IAxisIdentitiesMediator AxisIdentitiesMediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IAxisIdentitiesMediator>();

    [Fact]
    public async Task RegisterByCellphoneShouldFailWhenDataIsNullAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var command = new RegisterAxisIdentityByCellphoneCommand
        {
            Data = null,
            CellphoneId = CellphoneId.New.ToString()
        };

        var result = await RegistrationMediator(scope).RegisterByCellphoneAsync(command);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "DATA_REQUIRED");
    }

    [Fact]
    public async Task RegisterByCellphoneShouldFailWhenCellphoneIdIsInvalidAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var command = new RegisterAxisIdentityByCellphoneCommand
        {
            Data = ValidData(),
            CellphoneId = "not-a-guid"
        };

        var result = await RegistrationMediator(scope).RegisterByCellphoneAsync(command);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "CELLPHONE_ID_INVALID");
    }

    [Fact]
    public async Task RegisterByCellphoneShouldFailWhenDocumentIsInvalidCpfAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var data = ValidData() with { Document = "12345678900" };
        var command = new RegisterAxisIdentityByCellphoneCommand
        {
            Data = data,
            CellphoneId = CellphoneId.New.ToString()
        };

        var result = await RegistrationMediator(scope).RegisterByCellphoneAsync(command);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "DOCUMENT_INVALID");
    }

    [Fact]
    public async Task RegisterByEmailShouldFailWhenEmailIdIsInvalidAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var command = new RegisterAxisIdentityByEmailCommand
        {
            Data = ValidData(),
            EmailId = "not-a-guid"
        };

        var result = await RegistrationMediator(scope).RegisterByEmailAsync(command);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "EMAIL_ID_INVALID");
    }

    [Fact]
    public async Task AddCellphoneShouldFailWhenAxisIdentityIdIsInvalidAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var command = new AddCellphoneToAxisIdentityCommand
        {
            AxisIdentityId = null,
            CellphoneId = CellphoneId.New.ToString()
        };

        var result = await AxisIdentitiesMediator(scope).AddCellphoneAsync(command);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "AXIS_IDENTITY_ID_INVALID");
    }

    [Fact]
    public async Task GetByIdShouldFailWhenAxisIdentityIdIsInvalidAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var query = new GetAxisIdentityByIdQuery { AxisIdentityId = "not-a-guid" };

        var result = await AxisIdentitiesMediator(scope).GetByIdAsync(query);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "AXIS_IDENTITY_ID_INVALID");
    }

    [Fact]
    public async Task GetByCellphoneShouldFailWhenCountryIdIsNullAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var query = new GetAxisIdentityByCellphoneQuery { CountryId = null, CellphoneNumber = "11987654321" };

        var result = await AxisIdentitiesMediator(scope).GetByCellphoneAsync(query);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task GetByEmailShouldFailWhenEmailIsInvalidAsync()
    {
        using var scope = DataPrivacyTrixMocks.GetServiceProvider().CreateScope();
        var query = new GetAxisIdentityByEmailQuery { EmailAddress = "not-an-email" };

        var result = await AxisIdentitiesMediator(scope).GetByEmailAsync(query);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "EMAIL_ADDRESS_INVALID");
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
