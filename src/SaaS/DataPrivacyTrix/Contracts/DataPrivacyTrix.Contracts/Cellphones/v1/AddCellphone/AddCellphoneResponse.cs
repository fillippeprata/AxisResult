using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;

public record AddCellphoneResponse : IAxisCommandResponse
{
    public required string CellphoneId { get; init; }
}
