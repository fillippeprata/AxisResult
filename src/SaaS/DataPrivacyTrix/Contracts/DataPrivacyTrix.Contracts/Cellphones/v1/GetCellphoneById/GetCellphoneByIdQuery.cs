using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneById;

public record GetCellphoneByIdQuery : IAxisQuery<GetCellphoneByIdResponse>
{
    public string? CellphoneId { get; init; }
}
