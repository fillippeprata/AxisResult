using AxisTrix.Types;

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

    // ── GetById ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("br")]
    [InlineData("Br")]
    [InlineData("BR")]
    public void GetById_ValidBr_CaseInsensitive_ReturnsBr(string id)
    {
        var result = CountryIds.GetById(id);

        Assert.NotNull(result);
        Assert.Equal(CountryIds.Br, result.Value);
    }

    [Theory]
    [InlineData("us")]
    [InlineData("Us")]
    [InlineData("US")]
    public void GetById_ValidUs_CaseInsensitive_ReturnsUs(string id)
    {
        var result = CountryIds.GetById(id);

        Assert.NotNull(result);
        Assert.Equal(CountryIds.Us, result.Value);
    }

    [Fact]
    public void GetById_Null_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CountryIds.GetById(null));
    }

    [Fact]
    public void GetById_Empty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CountryIds.GetById(""));
    }

    [Fact]
    public void GetById_UnknownCountry_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CountryIds.GetById("xx"));
    }
}
