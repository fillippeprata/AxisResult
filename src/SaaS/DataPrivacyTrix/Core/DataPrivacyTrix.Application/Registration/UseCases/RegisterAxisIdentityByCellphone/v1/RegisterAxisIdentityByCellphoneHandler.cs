using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Application.AxisIdentities;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Ports;
using DataPrivacyTrix.SharedKernel.Cellphones;
using CountryId = Axis.Localization.CountryId;
using SharedKernelSecurityLevel = DataPrivacyTrix.SharedKernel.AxisIdentities.SecurityLevel;

namespace DataPrivacyTrix.Application.Registration.UseCases.RegisterAxisIdentityByCellphone.v1;

internal class RegisterAxisIdentityByCellphoneHandler(
    IAxisIdentityAggregateApplicationFactory factory,
    IUnitOfWorkProvider unitOfWorkProvider
) : IAxisCommandHandler<RegisterAxisIdentityByCellphoneCommand, RegisterAxisIdentityByCellphoneResponse>
{
    public Task<AxisResult<RegisterAxisIdentityByCellphoneResponse>> HandleAsync(RegisterAxisIdentityByCellphoneCommand cmd)
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

        CellphoneId cellphoneId = cmd.CellphoneId!;

        return factory.CreateAsync(args)
            .ThenAsync(app => app.AddCellphoneAsync(cellphoneId))
            .ThenAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync(app => new RegisterAxisIdentityByCellphoneResponse { AxisIdentityId = app.AxisIdentityId });
    }
}
