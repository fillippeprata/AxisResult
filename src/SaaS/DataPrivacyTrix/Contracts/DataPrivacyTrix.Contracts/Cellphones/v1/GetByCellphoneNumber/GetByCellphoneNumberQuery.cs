using AxisTrix.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;

public record GetByCellphoneNumberQuery : IAxisQuery<GetByCellphoneNumberResponse>
{
    public string? CountryId { get; init; }
    public string? CellphoneNumber { get; init; }
}
