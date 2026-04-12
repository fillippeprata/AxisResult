using AxisTrix.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;

public record GetByCellphoneNumberResponse : IAxisQueryResponse
{
    public required string CellphoneId { get; init; }
}
