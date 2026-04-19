using System.Globalization;
using Axis;
using AxisValidator.Brazil;

namespace DataPrivacyTrix.Domain.AxisIdentities.Root;

internal partial class AxisIdentityEntity
{
    protected Task<AxisResult> IsValidAsync()
    {
        return Task.FromResult(AxisResult.Combine(
            ValidateDisplayName(),
            ValidateDefaultLanguage(),
            ValidateSecurityLevel(),
            ValidateDocument()));
    }

    protected Task<AxisResult> AddCellphoneAsync()
        => Task.FromResult(AxisResult.Ok());

    protected Task<AxisResult> AddEmailAsync()
        => Task.FromResult(AxisResult.Ok());

    private AxisResult ValidateDisplayName()
    {
        if (string.IsNullOrWhiteSpace(DisplayName))
            return AxisError.ValidationRule("DISPLAY_NAME_REQUIRED");

        return DisplayName.Length > 255
            ? AxisError.ValidationRule("DISPLAY_NAME_TOO_LONG")
            : AxisResult.Ok();
    }

    private AxisResult ValidateDefaultLanguage()
    {
        if (string.IsNullOrWhiteSpace(DefaultLanguage))
            return AxisError.ValidationRule("DEFAULT_LANGUAGE_INVALID");

        var exists = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Any(c => c.Name.Equals(DefaultLanguage, StringComparison.OrdinalIgnoreCase));

        return exists
            ? AxisResult.Ok()
            : AxisError.ValidationRule("DEFAULT_LANGUAGE_INVALID");
    }

    private AxisResult ValidateSecurityLevel()
        => Enum.IsDefined(SecurityLevel)
            ? AxisResult.Ok()
            : AxisError.ValidationRule("SECURITY_LEVEL_INVALID");

    private AxisResult ValidateDocument()
        => CountryId.ValidateDocument(IsIndividual, Document);
}
