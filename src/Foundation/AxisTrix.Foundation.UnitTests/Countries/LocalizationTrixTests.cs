using AxisTrix.Types;
using AxisTrix.Validation.Localization;

namespace AxisTrix.Mediator.UnitTests.Countries;

public class LocalizationTrixTests
{
    [Fact]
    public void GetFormattedPhone_BrCountry_FormatsPhone()
    {
        var result = CountryIds.Br.GetFormattedPhone("11999887766");

        Assert.NotNull(result);
        Assert.Equal("(11) 99988-7766", result);
    }

    [Fact]
    public void GetFormattedPhone_BrCountry_NullPhone_ReturnsNull()
    {
        var result = CountryIds.Br.GetFormattedPhone(null);

        Assert.Null(result);
    }

    [Fact]
    public void GetFormattedPhone_BrCountry_EmptyPhone_ReturnsNull()
    {
        var result = CountryIds.Br.GetFormattedPhone("");

        Assert.Null(result);
    }

    [Fact]
    public void GetFormattedPhone_UsCountry_ReturnsPhoneAsIs()
    {
        var phone = "2025551234";

        var result = CountryIds.Us.GetFormattedPhone(phone);

        Assert.Equal(phone, result);
    }

    [Fact]
    public void GetFormattedPhone_UsCountry_NullPhone_ReturnsNull()
    {
        var result = CountryIds.Us.GetFormattedPhone(null);

        Assert.Null(result);
    }
}
