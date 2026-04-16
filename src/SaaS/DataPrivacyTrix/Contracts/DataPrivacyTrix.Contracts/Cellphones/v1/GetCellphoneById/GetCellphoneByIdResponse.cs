using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneById;

public record GetCellphoneByIdResponse : IAxisQueryResponse
{
    public required string CountryId { get; init; }
    public required string CellphoneNumber { get; init; }
}
