using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Cellphones.v1;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneByNumber;
using DataPrivacyTrix.Ports.AxisIdentities;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Identities.GetAxisIdentityByCellphone.v1;

internal class GetAxisIdentityByCellphoneHandler(
    ICellphonesMediator cellphonesMediator,
    IAxisIdentitiesReaderPort readerPort
) : IAxisQueryHandler<GetAxisIdentityByCellphoneQuery, GetAxisIdentityByCellphoneResponse>
{
    public Task<AxisResult<GetAxisIdentityByCellphoneResponse>> HandleAsync(GetAxisIdentityByCellphoneQuery query) =>
        cellphonesMediator.GetByCellphoneNumberAsync(new GetCellphoneByNumberQuery
            {
                CountryId = query.CountryId,
                CellphoneNumber = query.CellphoneNumber
            })
            .ZipAsync(cellphone => readerPort.GetByCellphoneIdAsync(cellphone.CellphoneId))
            .MapAsync((_, entity) => new GetAxisIdentityByCellphoneResponse
            {
                AxisIdentityId = entity.AxisIdentityId,
                DisplayName = entity.DisplayName
            });
}
