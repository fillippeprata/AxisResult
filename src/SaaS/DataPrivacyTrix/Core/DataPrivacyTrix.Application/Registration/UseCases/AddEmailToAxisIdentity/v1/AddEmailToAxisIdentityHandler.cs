using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.Registration.v1.AddEmailToAxisIdentity;
using DataPrivacyTrix.Ports;

namespace DataPrivacyTrix.Application.Registration.UseCases.AddEmailToAxisIdentity.v1;

internal class AddEmailToAxisIdentityHandler(
    IAxisIdentityAggregateApplicationFactory factory,
    IUnitOfWorkProvider unitOfWorkProvider
) : IAxisCommandHandler<AddEmailToAxisIdentityCommand>
{
    public Task<AxisResult> HandleAsync(AddEmailToAxisIdentityCommand cmd) =>
        factory.GetByIdAsync(cmd.AxisIdentityId!)
            .ThenAsync(app => app.AddEmailAsync(cmd.EmailId!))
            .ThenAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
            .MatchAsync(onSuccess: _ => AxisResult.Ok(), AxisResult.Error);
}
