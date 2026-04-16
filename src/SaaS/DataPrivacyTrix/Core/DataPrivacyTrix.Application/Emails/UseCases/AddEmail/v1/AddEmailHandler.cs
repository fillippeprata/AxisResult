using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.Emails.v1.AddEmail;
using DataPrivacyTrix.Ports;

namespace DataPrivacyTrix.Application.Emails.UseCases.AddEmail.v1;

internal class AddEmailHandler(
    IEmailAggregateApplicationFactory factory,
    IUnitOfWorkProvider unitOfWorkProvider
) : IAxisCommandHandler<AddEmailCommand, AddEmailResponse>
{
    public Task<AxisResult<AddEmailResponse>> HandleAsync(AddEmailCommand cmd)
        => factory.CreateAsync(new() { EmailAddress = cmd.EmailAddress! })
            .ThenAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync(entity => new AddEmailResponse { EmailId = entity.EmailId });
}
