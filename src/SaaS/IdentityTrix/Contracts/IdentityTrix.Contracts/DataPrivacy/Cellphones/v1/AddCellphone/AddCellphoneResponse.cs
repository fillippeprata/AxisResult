using AxisTrix.CQRS.Commands;
using IdentityTrix.SharedKernel.DataPrivacy;

namespace IdentityTrix.Contracts.DataPrivacy.Cellphones.v1.AddCellphone;

public record AddCellphoneResponse : IAxisCommandResponse
{
    public required CellphoneId CellphoneId { get; init; }
}
