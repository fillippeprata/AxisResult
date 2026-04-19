using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Emails.AddEmailToAxisIdentity;
using DataPrivacyTrix.Ports;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Emails.AddEmailToAxisIdentity.v1;

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
