using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneByNumber;

public record GetCellphoneByNumberQuery : IAxisQuery<GetCellphoneByNumberResponse>
{
    public string? CountryId { get; init; }
    public string? CellphoneNumber { get; init; }
}
