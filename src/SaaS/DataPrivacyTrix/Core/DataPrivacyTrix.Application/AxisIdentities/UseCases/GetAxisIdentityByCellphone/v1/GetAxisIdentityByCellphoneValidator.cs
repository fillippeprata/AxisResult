using AxisValidator;
using AxisValidator.Brazil;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByCellphone;
using FluentValidation;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.GetAxisIdentityByCellphone.v1;

internal class GetAxisIdentityByCellphoneValidator : AxisValidatorBase<GetAxisIdentityByCellphoneQuery>
{
    public GetAxisIdentityByCellphoneValidator()
    {
        RequiredTryParse(x => x.CountryId, "COUNTRY_ID_REQUIRED",
            value => value is not null && CountryId.TryParse(value.ToString(), out _));
        NotNullOrEmpty(x => x.CellphoneNumber, "CELLPHONE_NUMBER_INVALID");

        RuleFor(x => x.CellphoneNumber)
            .Must((query, cellphone) =>
            {
                if (!CountryId.TryParse(query.CountryId, out var countryId)) return true;
                return countryId.FormatCellphone(cellphone).IsSuccess;
            })
            .WithErrorCode("CELLPHONE_NUMBER_INVALID")
            .When(x => !string.IsNullOrWhiteSpace(x.CountryId) && !string.IsNullOrWhiteSpace(x.CellphoneNumber));
    }
}
