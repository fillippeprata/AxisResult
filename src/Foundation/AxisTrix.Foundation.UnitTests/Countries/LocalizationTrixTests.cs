using AxisTrix.Types;
using AxisTrix.Validation.Localization;

namespace AxisTrix.Mediator.UnitTests.Countries;

public class LocalizationTrixTests
{
    [Fact]
    public void GetFormattedPhone_BrCountry_ReturnsSuccessWithFormattedPhone()
    {
        var result = CountryIds.Br.GetFormattedPhone("11999887766");

        Assert.True(result.IsSuccess);
        Assert.Equal("(11) 99988-7766", result.Value);
    }

    [Fact]
    public void GetFormattedPhone_BrCountry_NullPhone_ReturnsValidationFailure()
    {
        var result = CountryIds.Br.GetFormattedPhone(null);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void GetFormattedPhone_BrCountry_EmptyPhone_ReturnsValidationFailure()
    {
        var result = CountryIds.Br.GetFormattedPhone("");

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void GetFormattedPhone_UsCountry_ReturnsMappingFailure()
    {
        var phone = "2025551234";

        var result = CountryIds.Us.GetFormattedPhone(phone);

        Assert.Equal("THERE_IS_NO_VALIDATION_FOR_THIS_COUNTRY", result.Errors[0].Code);
    }
}
