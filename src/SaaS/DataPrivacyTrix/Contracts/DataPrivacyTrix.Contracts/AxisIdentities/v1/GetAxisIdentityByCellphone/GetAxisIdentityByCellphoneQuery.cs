using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByCellphone;

public record GetAxisIdentityByCellphoneQuery : IAxisQuery<GetAxisIdentityByCellphoneResponse>
{
    public string? CountryId { get; init; }
    public string? CellphoneNumber { get; init; }
}
