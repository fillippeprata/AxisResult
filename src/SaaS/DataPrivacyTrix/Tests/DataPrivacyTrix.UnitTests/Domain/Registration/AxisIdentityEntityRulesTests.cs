using DataPrivacyTrix.Application.Registration;
using DataPrivacyTrix.Ports.Registration;
using DataPrivacyTrix.SharedKernel.Registration;
using DataPrivacyTrix.UnitTests.Mocks;
using Moq;
using CountryIds = Axis.Localization.CountryIds;

namespace DataPrivacyTrix.UnitTests.Domain.Registration;

public class AxisIdentityEntityRulesTests
{
    [Fact]
    public async Task IsValidAsyncShouldReturnSuccessForValidBrazilianIndividualAsync()
    {
        var app = BuildApp(new MockAxisIdentityProperties(
            AxisIdentityId.New, true, "39053344705", CountryIds.Br, "User", "pt-BR", SecurityLevel.Normal));

        var result = await app.IsValidAsync();

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task IsValidAsyncShouldFailForInvalidCpfAsync()
    {
        var app = BuildApp(new MockAxisIdentityProperties(
            AxisIdentityId.New, true, "12345678900", CountryIds.Br, "User", "pt-BR", SecurityLevel.Normal));

        var result = await app.IsValidAsync();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "DOCUMENT_INVALID");
    }

    [Fact]
    public async Task IsValidAsyncShouldFailForNonBrazilianCountryAsync()
    {
        var app = BuildApp(new MockAxisIdentityProperties(
            AxisIdentityId.New, true, "39053344705", CountryIds.Us, "User", "pt-BR", SecurityLevel.Normal));

        var result = await app.IsValidAsync();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "COUNTRY_ID_DOCUMENT_NOT_IMPLEMENTED");
    }

    [Fact]
    public async Task IsValidAsyncShouldFailForBrazilianCompanyAsync()
    {
        var app = BuildApp(new MockAxisIdentityProperties(
            AxisIdentityId.New, false, "39053344705", CountryIds.Br, "Company", "pt-BR", SecurityLevel.Normal));

        var result = await app.IsValidAsync();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "COUNTRY_ID_DOCUMENT_NOT_IMPLEMENTED");
    }

    [Fact]
    public async Task IsValidAsyncShouldFailForInvalidDefaultLanguageAsync()
    {
        var app = BuildApp(new MockAxisIdentityProperties(
            AxisIdentityId.New, true, "39053344705", CountryIds.Br, "User", "xyz-ZZ", SecurityLevel.Normal));

        var result = await app.IsValidAsync();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "DEFAULT_LANGUAGE_INVALID");
    }

    [Fact]
    public async Task IsValidAsyncShouldFailForInvalidSecurityLevelAsync()
    {
        var app = BuildApp(new MockAxisIdentityProperties(
            AxisIdentityId.New, true, "39053344705", CountryIds.Br, "User", "pt-BR", (SecurityLevel)99));

        var result = await app.IsValidAsync();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "SECURITY_LEVEL_INVALID");
    }

    [Fact]
    public async Task IsValidAsyncShouldFailForEmptyDisplayNameAsync()
    {
        var app = BuildApp(new MockAxisIdentityProperties(
            AxisIdentityId.New, true, "39053344705", CountryIds.Br, "", "pt-BR", SecurityLevel.Normal));

        var result = await app.IsValidAsync();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "DISPLAY_NAME_REQUIRED");
    }

    private static IAxisIdentityAggregateApplication BuildApp(IAxisIdentityEntityProperties properties)
    {
        var cellphonesWriter = new Mock<IAxisIdentityCellphonesWritePort>();
        var emailsWriter = new Mock<IAxisIdentityEmailsWritePort>();
        return new AxisIdentityAggregateApplication(properties, cellphonesWriter.Object, emailsWriter.Object);
    }
}
