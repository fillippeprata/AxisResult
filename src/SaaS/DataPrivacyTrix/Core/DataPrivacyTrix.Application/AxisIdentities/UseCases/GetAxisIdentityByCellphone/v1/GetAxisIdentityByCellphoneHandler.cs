using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Cellphones.v1;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;
using DataPrivacyTrix.Ports.AxisIdentities;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.GetAxisIdentityByCellphone.v1;

internal class GetAxisIdentityByCellphoneHandler(
    ICellphonesMediator cellphonesMediator,
    IAxisIdentitiesReaderPort readerPort
) : IAxisQueryHandler<GetAxisIdentityByCellphoneQuery, GetAxisIdentityByCellphoneResponse>
{
    public async Task<AxisResult<GetAxisIdentityByCellphoneResponse>> HandleAsync(GetAxisIdentityByCellphoneQuery query)
    {
        var cellphoneResult = await cellphonesMediator.GetByCellphoneNumberAsync(new GetByCellphoneNumberQuery
        {
            CountryId = query.CountryId,
            CellphoneNumber = query.CellphoneNumber
        });

        if (cellphoneResult.IsFailure)
            return AxisResult.Error<GetAxisIdentityByCellphoneResponse>(cellphoneResult.Errors);

        return await readerPort.GetByCellphoneIdAsync((CellphoneId)cellphoneResult.Value.CellphoneId)
            .MapAsync(entity => new GetAxisIdentityByCellphoneResponse
            {
                AxisIdentityId = entity.AxisIdentityId,
                DisplayName = entity.DisplayName
            });
    }
}
