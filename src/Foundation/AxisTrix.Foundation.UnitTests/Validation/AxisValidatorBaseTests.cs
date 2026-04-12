using AxisTrix.Validation;

namespace AxisTrix.Mediator.UnitTests.Validation;

public class AxisValidatorBaseTests
{
    // ── Test commands ──────────────────────────────────────────────────────

    private record TestCommand
    {
        public string? Email { get; init; }
        public string? Guid7Id { get; init; }
        public string? CountryId { get; init; }
        public string? DocumentId { get; init; }
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

    // ── DocumentId (with dynamic CountryId selector) ───────────────────────

    private class DocumentIdSelectorValidator : AxisValidatorBase<TestCommand>
    {
        public DocumentIdSelectorValidator()
        {
            DocumentId(x => x.DocumentId, x => x.CountryId, "DOCUMENT_ID_NULL_OR_NOT_VALID");
        }
    }

    [Fact]
    public void DocumentIdSelector_ValidCpfWithBr_Passes()
    {
        var validator = new DocumentIdSelectorValidator();
        var result = validator.Validate(new TestCommand { DocumentId = "52998224725", CountryId = "br" });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DocumentIdSelector_NullCountry_InvalidDocument_Fails()
    {
        var validator = new DocumentIdSelectorValidator();
        var result = validator.Validate(new TestCommand { DocumentId = "12345678901", CountryId = null });
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
}
