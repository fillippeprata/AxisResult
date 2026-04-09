using AxisTrix.Validation.Localization.Brazil;
using AxisTrix.Validation.Localization.Brazil.Helpers;

namespace AxisTrix.Mediator.UnitTests.Countries.Brazil;

public class RandomBrazilianDataHelperTests
{
    [Fact]
    public void GenerateCpf_Unformatted_Returns11Digits()
    {
        var cpf = RandomBrazilianDataHelper.GenerateCpf();

        Assert.Equal(11, cpf.Length);
        Assert.True(cpf.All(char.IsDigit));
    }

    [Fact]
    public void GenerateCpf_Unformatted_IsValid()
    {
        var cpf = RandomBrazilianDataHelper.GenerateCpf();

        Assert.True(CpfValidator.Validate(cpf), $"Generated CPF '{cpf}' should be valid");
    }

    [Fact]
    public void GenerateCpf_Formatted_Returns14Chars()
    {
        var cpf = RandomBrazilianDataHelper.GenerateCpf(format: true);

        Assert.Equal(14, cpf.Length);
    }

    [Fact]
    public void GenerateCpf_Formatted_MatchesPattern()
    {
        var cpf = RandomBrazilianDataHelper.GenerateCpf(format: true);

        // Expected pattern: XXX.XXX.XXX-XX
        Assert.Equal('.', cpf[3]);
        Assert.Equal('.', cpf[7]);
        Assert.Equal('-', cpf[11]);
    }

    [Fact]
    public void GenerateCpf_Formatted_IsValid()
    {
        var cpf = RandomBrazilianDataHelper.GenerateCpf(format: true);

        Assert.True(CpfValidator.Validate(cpf), $"Generated formatted CPF '{cpf}' should be valid");
    }

    [Fact]
    public void GenerateCpf_MultipleIterations_AllValid()
    {
        for (var i = 0; i < 50; i++)
        {
            var cpf = RandomBrazilianDataHelper.GenerateCpf();
            Assert.True(CpfValidator.Validate(cpf), $"Iteration {i}: generated CPF '{cpf}' should be valid");
        }
    }

    [Fact]
    public void GenerateCpf_MultipleIterations_Formatted_AllValid()
    {
        for (var i = 0; i < 50; i++)
        {
            var cpf = RandomBrazilianDataHelper.GenerateCpf(format: true);
            Assert.True(CpfValidator.Validate(cpf), $"Iteration {i}: generated formatted CPF '{cpf}' should be valid");
        }
    }
}
