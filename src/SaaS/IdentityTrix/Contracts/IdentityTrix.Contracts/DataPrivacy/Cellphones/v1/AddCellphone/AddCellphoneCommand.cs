using AxisTrix.CQRS.Commands;

namespace IdentityTrix.Contracts.DataPrivacy.Cellphones.v1.AddCellphone;

public record AddCellphoneCommand : IAxisCommand<AddCellphoneResponse>
{
    public string? CountryId { get; init; }
    public string? CellphoneNumber { get; init; }
}
