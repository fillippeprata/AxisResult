using AxisTrix.Types.Localization;
using CountryId = AxisTrix.Types.Localization.CountryId;

namespace AxisTrix.Mediator.UnitTests.Types;

public class CountryIdTests
{
    // ── CountryId construction ──────────────────────────────────────────────

    [Fact]
    public void CountryId_NullValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CountryId(null));
    }

    [Fact]
    public void CountryId_EmptyValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CountryId(""));
    }

    [Fact]
    public void CountryId_WhitespaceValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CountryId("   "));
    }

    // ── CountryIds static entries ───────────────────────────────────────────

    [Fact]
    public void Br_HasCorrectValue()
    {
        Assert.Equal("br", CountryIds.Br.Value);
    }

    [Fact]
    public void Us_HasCorrectValue()
    {
        Assert.Equal("us", CountryIds.Us.Value);
    }

    [Fact]
    public void AllCountries_HasTwoEntries()
    {
        Assert.Equal(2, CountryIds.AllCountries.Count);
    }

    // ── CountryId TryParse ─────────────────────────────────────────────────

    [Fact]
    public void TryParse_ValidBrString_ReturnsTrue()
    {
        var result = CountryId.TryParse("br", out var countryId);

        Assert.True(result);
        Assert.Equal("br", countryId.Value);
    }

    [Fact]
    public void TryParse_ValidUpperCase_ReturnsTrueLowered()
    {
        var result = CountryId.TryParse("BR", out var countryId);

        Assert.True(result);
        Assert.Equal("br", countryId.Value);
    }

    [Fact]
    public void TryParse_NullValue_ReturnsFalse()
    {
        var result = CountryId.TryParse(null, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryParse_EmptyString_ReturnsFalse()
    {
        var result = CountryId.TryParse("", out _);

        Assert.False(result);
    }

    [Fact]
    public void TryParse_ObjectOverload_ValidString_ReturnsTrue()
    {
        var result = CountryId.TryParse("us");

        Assert.True(result);
    }

    [Fact]
    public void TryParse_ObjectOverload_NullObject_ReturnsFalse()
    {
        var result = CountryId.TryParse(null);

        Assert.False(result);
    }

    // ── Implicit conversions and ToString ──────────────────────────────────

    [Fact]
    public void ImplicitToString_ReturnsValue()
    {
        CountryId id = new("br");
        string value = id;

        Assert.Equal("br", value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var id = new CountryId("us");

        Assert.Equal("us", id.ToString());
    }

    [Fact]
    public void GetHashCode_SameValues_AreCaseInsensitiveEqual()
    {
        var id1 = new CountryId("br");
        var id2 = new CountryId("BR");

        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }
}
