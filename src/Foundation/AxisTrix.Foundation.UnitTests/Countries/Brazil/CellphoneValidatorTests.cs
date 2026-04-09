using CellphoneValidator = AxisTrix.Validation.Localization.Brazil.CellphoneValidator;

namespace AxisTrix.Mediator.UnitTests.Countries.Brazil;

public class CellphoneValidatorTests
{
    [Theory]
    [InlineData("1199998888", "(11) 99999-8888")]
    [InlineData("11999998888", "(11) 99999-8888")]
    [InlineData("11 99999-8888", "(11) 99999-8888")]
    [InlineData("(11) 99999-8888", "(11) 99999-8888")]
    [InlineData("5511999998888", "(11) 99999-8888")]
    [InlineData("55 11 99999 8888", "(11) 99999-8888")]
    [InlineData("011999998888", "(11) 99999-8888")]
    [InlineData("0011999998888", "(11) 99999-8888")]
    public void ValidateAndFormat_ValidCellphone_ReturnsFormattedString(string cellphone, string expected)
    {
        // Act
        var result = CellphoneValidator.Format(cellphone);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("00999998888")] // invalid DDD (00)
    [InlineData("119999988888")] // too long
    [InlineData("11099998888")] // 9 after DDD must be 9
    [InlineData("abc11999998888")] // letters
    public void ValidateAndFormat_InvalidCellphone_ReturnsNull(string? cellphone)
    {
        // Act
        var result = CellphoneValidator.Format(cellphone!);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("5511999998888", "(11) 99999-8888")]
    [InlineData("551199999888", "(11) 99999-9888")]
    [InlineData("55119999988881", null)] // 55 + 12 digits = 14 total. Length not 12 or 13.
    public void ValidateAndFormat_WithCountryCode_HandlesCorrectly(string cellphone, string? expected)
    {
        // Act
        var result = CellphoneValidator.Format(cellphone);

        // Assert
        Assert.Equal(expected, result);
    }
}
