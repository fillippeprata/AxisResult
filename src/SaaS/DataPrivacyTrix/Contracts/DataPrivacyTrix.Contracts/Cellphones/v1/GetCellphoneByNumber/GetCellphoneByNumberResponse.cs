using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneByNumber;

public record GetCellphoneByNumberResponse : IAxisQueryResponse
{
    public required string CellphoneId { get; init; }
}
