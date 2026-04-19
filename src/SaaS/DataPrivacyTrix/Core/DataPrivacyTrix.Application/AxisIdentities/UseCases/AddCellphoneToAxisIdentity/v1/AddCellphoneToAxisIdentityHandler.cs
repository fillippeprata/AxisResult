using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Ports;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.AddCellphoneToAxisIdentity.v1;

internal class AddCellphoneToAxisIdentityHandler(
    IAxisIdentityAggregateApplicationFactory factory,
    IUnitOfWorkProvider unitOfWorkProvider
) : IAxisCommandHandler<AddCellphoneToAxisIdentityCommand>
{
    public Task<AxisResult> HandleAsync(AddCellphoneToAxisIdentityCommand cmd) =>
        factory.GetByIdAsync(cmd.AxisIdentityId!)
            .ThenAsync(app => app.AddCellphoneAsync(cmd.CellphoneId!))
            .ThenAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
            .MatchAsync(onSuccess: _ => AxisResult.Ok(), AxisResult.Error);
}
