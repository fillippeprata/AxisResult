using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;

public record AddCellphoneCommand : IAxisCommand<AddCellphoneResponse>
{
    public string? CountryId { get; init; }
    public string? CellphoneNumber { get; init; }
}
