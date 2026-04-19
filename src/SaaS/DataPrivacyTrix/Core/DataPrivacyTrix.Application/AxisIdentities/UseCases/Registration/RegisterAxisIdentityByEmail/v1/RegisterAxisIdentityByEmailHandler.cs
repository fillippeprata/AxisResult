using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByEmail;
using DataPrivacyTrix.Ports;
using CountryId = Axis.Localization.CountryId;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;
using SharedKernelSecurityLevel = DataPrivacyTrix.SharedKernel.AxisIdentities.SecurityLevel;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Registration.RegisterAxisIdentityByEmail.v1;

internal class RegisterAxisIdentityByEmailHandler(
    IAxisIdentityAggregateApplicationFactory factory,
    IUnitOfWorkProvider unitOfWorkProvider
) : IAxisCommandHandler<RegisterAxisIdentityByEmailCommand, RegisterAxisIdentityByEmailResponse>
{
    public Task<AxisResult<RegisterAxisIdentityByEmailResponse>> HandleAsync(RegisterAxisIdentityByEmailCommand cmd)
    {
        var data = cmd.Data!;
        var args = new IAxisIdentityAggregateApplicationFactory.NewArgs
        {
            IsIndividual = data.IsIndividual!.Value,
            Document = data.Document!,
            CountryId = (CountryId)data.CountryId!,
            DisplayName = data.DisplayName!,
            DefaultLanguage = data.DefaultLanguage!,
            SecurityLevel = Enum.Parse<SharedKernelSecurityLevel>(data.SecurityLevel!, ignoreCase: true)
        };

        EmailId emailId = cmd.EmailId!;

        return factory.CreateAsync(args)
            .ThenAsync(app => app.AddEmailAsync(emailId))
            .ThenAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync(app => new RegisterAxisIdentityByEmailResponse { AxisIdentityId = app.AxisIdentityId });
    }
}
