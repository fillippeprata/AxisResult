using AxisTrix.Validation.Localization.Brazil;

namespace AxisTrix.Mediator.UnitTests.Countries.Brazil;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("007.173.370-14")]
    [InlineData("00717337014")]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void Validate_ValidCpf_ReturnsTrue(string cpf)
    {
        // Act
        var result = CpfValidator.Validate(cpf);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    [InlineData("12345678909")] // Invalid checksum
    public void Validate_InvalidCpf_ReturnsFalse(string? cpf)
    {
        // Act
        var result = CpfValidator.Validate(cpf);

        // Assert
        Assert.False(result);
    }
}
