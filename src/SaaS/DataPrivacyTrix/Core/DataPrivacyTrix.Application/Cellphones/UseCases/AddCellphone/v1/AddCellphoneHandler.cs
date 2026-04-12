using AxisTrix.CQRS.Commands;
using AxisTrix.Results;
using DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;
using DataPrivacyTrix.Ports;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.AddCellphone.v1;

internal class AddCellphoneHandler(
    ICellphoneAggregateApplicationFactory factory,
    IUnitOfWorkProvider unitOfWorkProvider
) : IAxisCommandHandler<AddCellphoneCommand, AddCellphoneResponse>
{
    public Task<AxisResult<AddCellphoneResponse>> HandleAsync(AddCellphoneCommand cmd)
        => factory.CreateAsync(new() { CountryId = cmd.CountryId!, CellphoneNumber = cmd.CellphoneNumber! })
            .ThenAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync(entity => new AddCellphoneResponse { CellphoneId = entity.CellphoneId });
}
