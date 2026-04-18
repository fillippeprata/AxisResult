namespace AxisValidator.FluentValidation.UnitTests;

public class AxisValidatorBaseTests
{
    // ── Test commands ──────────────────────────────────────────────────────

    private record TestCommand
    {
        public string? Email { get; init; }
        public string? Guid7Id { get; init; }
        public string? Name { get; init; }
    }

    // ── RequiredEmail ──────────────────────────────────────────────────────

    private class EmailValidator : AxisValidatorBase<TestCommand>
    {
        public EmailValidator()
        {
            RequiredEmail(x => x.Email, "EMAIL_NULL_OR_NOT_VALID");
        }
    }

    [Fact]
    public void RequiredEmail_ValidEmail_Passes()
    {
        var validator = new EmailValidator();
        var result = validator.Validate(new TestCommand { Email = "test@example.com" });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RequiredEmail_NullEmail_Fails()
    {
        var validator = new EmailValidator();
        var result = validator.Validate(new TestCommand { Email = null });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "EMAIL_NULL_OR_NOT_VALID");
    }

    [Fact]
    public void RequiredEmail_InvalidFormat_Fails()
    {
        var validator = new EmailValidator();
        var result = validator.Validate(new TestCommand { Email = "not-an-email" });
        Assert.False(result.IsValid);
    }

    // ── RequiredGuid7 ──────────────────────────────────────────────────────

    private class Guid7Validator : AxisValidatorBase<TestCommand>
    {
        public Guid7Validator()
        {
            RequiredGuid7(x => x.Guid7Id, "GUID7_NULL_OR_NOT_VALID");
        }
    }

    [Fact]
    public void RequiredGuid7_ValidGuid7_Passes()
    {
        var guid7 = Guid.CreateVersion7().ToString();
        var validator = new Guid7Validator();
        var result = validator.Validate(new TestCommand { Guid7Id = guid7 });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RequiredGuid7_NullValue_Fails()
    {
        var validator = new Guid7Validator();
        var result = validator.Validate(new TestCommand { Guid7Id = null });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "GUID7_NULL_OR_NOT_VALID");
    }

    [Fact]
    public void RequiredGuid7_NonGuidString_Fails()
    {
        var validator = new Guid7Validator();
        var result = validator.Validate(new TestCommand { Guid7Id = "not-a-guid" });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void RequiredGuid7_GuidVersion4_Fails()
    {
        var guid4 = Guid.NewGuid().ToString();
        var validator = new Guid7Validator();
        var result = validator.Validate(new TestCommand { Guid7Id = guid4 });
        Assert.False(result.IsValid);
    }

    // ── NotNullOrEmpty with Action dependentRules ──────────────────────────

    private class NotNullOrEmptyActionValidator : AxisValidatorBase<TestCommand>
    {
        public NotNullOrEmptyActionValidator()
        {
            NotNullOrEmpty(x => x.Name, "NAME_NULL_OR_EMPTY",
                () => RequiredEmail(x => x.Email, "EMAIL_NULL_OR_NOT_VALID"));
        }
    }

    [Fact]
    public void NotNullOrEmpty_Action_NullName_FailsNameOnly()
    {
        var validator = new NotNullOrEmptyActionValidator();
        var result = validator.Validate(new TestCommand { Name = null, Email = null });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_NULL_OR_EMPTY");
    }

    [Fact]
    public void NotNullOrEmpty_Action_ValidName_NullEmail_FailsEmail()
    {
        var validator = new NotNullOrEmptyActionValidator();
        var result = validator.Validate(new TestCommand { Name = "John", Email = null });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "EMAIL_NULL_OR_NOT_VALID");
    }

    [Fact]
    public void NotNullOrEmpty_Action_ValidNameAndEmail_Passes()
    {
        var validator = new NotNullOrEmptyActionValidator();
        var result = validator.Validate(new TestCommand { Name = "John", Email = "test@test.com" });
        Assert.True(result.IsValid);
    }

    // ── RequiredWithMaxLength ──────────────────────────────────────────────

    private class MaxLengthValidator : AxisValidatorBase<TestCommand>
    {
        public MaxLengthValidator()
        {
            RequiredWithMaxLength(x => x.Name, "NAME_NULL_OR_TOO_LONG", 10);
        }
    }

    [Fact]
    public void RequiredWithMaxLength_ValidName_Passes()
    {
        var validator = new MaxLengthValidator();
        var result = validator.Validate(new TestCommand { Name = "Short" });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RequiredWithMaxLength_NullName_FailsWithErrorCode()
    {
        var validator = new MaxLengthValidator();
        var result = validator.Validate(new TestCommand { Name = null });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_NULL_OR_TOO_LONG");
    }

    [Fact]
    public void RequiredWithMaxLength_EmptyName_FailsWithErrorCode()
    {
        var validator = new MaxLengthValidator();
        var result = validator.Validate(new TestCommand { Name = "" });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_NULL_OR_TOO_LONG");
    }

    [Fact]
    public void RequiredWithMaxLength_WhitespaceName_FailsWithErrorCode()
    {
        var validator = new MaxLengthValidator();
        var result = validator.Validate(new TestCommand { Name = "   " });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_NULL_OR_TOO_LONG");
    }

    [Fact]
    public void RequiredWithMaxLength_TooLongName_FailsWithErrorCode()
    {
        var validator = new MaxLengthValidator();
        var result = validator.Validate(new TestCommand { Name = new string('a', 11) });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_NULL_OR_TOO_LONG");
    }

    [Fact]
    public void RequiredEmail_EmptyEmail_FailsWithErrorCode()
    {
        var validator = new EmailValidator();
        var result = validator.Validate(new TestCommand { Email = "" });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "EMAIL_NULL_OR_NOT_VALID");
    }

    [Fact]
    public void RequiredGuid7_EmptyValue_FailsWithErrorCode()
    {
        var validator = new Guid7Validator();
        var result = validator.Validate(new TestCommand { Guid7Id = "" });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "GUID7_NULL_OR_NOT_VALID");
    }

    // ── RequiredSlug ───────────────────────────────────────────────────────

    private class SlugValidator : AxisValidatorBase<TestCommand>
    {
        public SlugValidator()
        {
            RequiredSlug(x => x.Name, "NAME_INVALID", 10);
        }
    }

    [Theory]
    [InlineData("acme")]
    [InlineData("acme-corp")]
    [InlineData("acme_corp")]
    [InlineData("Acme-Cp-1")]
    [InlineData("ABC123")]
    [InlineData("a")]
    public void RequiredSlug_ValidSlug_Passes(string name)
    {
        var validator = new SlugValidator();
        var result = validator.Validate(new TestCommand { Name = name });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RequiredSlug_NullName_FailsWithErrorCode()
    {
        var validator = new SlugValidator();
        var result = validator.Validate(new TestCommand { Name = null });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_INVALID");
    }

    [Fact]
    public void RequiredSlug_EmptyName_FailsWithErrorCode()
    {
        var validator = new SlugValidator();
        var result = validator.Validate(new TestCommand { Name = "" });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_INVALID");
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("acme corp")]
    [InlineData("acme\tcorp")]
    [InlineData(" acme")]
    [InlineData("acme ")]
    public void RequiredSlug_WhitespaceInName_FailsWithErrorCode(string name)
    {
        var validator = new SlugValidator();
        var result = validator.Validate(new TestCommand { Name = name });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_INVALID");
    }

    [Theory]
    [InlineData("acme@corp")]
    [InlineData("acme!")]
    [InlineData("acme.corp")]
    [InlineData("acme#1")]
    [InlineData("acme/corp")]
    [InlineData("acme\\corp")]
    [InlineData("ação")]
    public void RequiredSlug_SpecialCharactersInName_FailsWithErrorCode(string name)
    {
        var validator = new SlugValidator();
        var result = validator.Validate(new TestCommand { Name = name });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_INVALID");
    }

    [Fact]
    public void RequiredSlug_NameTooLong_FailsWithErrorCode()
    {
        var validator = new SlugValidator();
        var result = validator.Validate(new TestCommand { Name = new string('a', 11) });
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "NAME_INVALID");
    }
}
