using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityById;
using DataPrivacyTrix.Ports.AxisIdentities;
using AxisIdentityId = DataPrivacyTrix.SharedKernel.AxisIdentities.AxisIdentityId;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Identities.GetAxisIdentityById.v1;

internal class GetAxisIdentityByIdHandler(
    IAxisIdentitiesReaderPort readerPort
) : IAxisQueryHandler<GetAxisIdentityByIdQuery, GetAxisIdentityByIdResponse>
{
    public Task<AxisResult<GetAxisIdentityByIdResponse>> HandleAsync(GetAxisIdentityByIdQuery query)
    {
        AxisIdentityId axisIdentityId = query.AxisIdentityId!;

        return readerPort.GetByIdAsync(axisIdentityId)
            .MapAsync(entity => new GetAxisIdentityByIdResponse
            {
                AxisIdentityId = entity.AxisIdentityId,
                IsIndividual = entity.IsIndividual,
                Document = entity.Document,
                CountryId = entity.CountryId,
                DisplayName = entity.DisplayName,
                DefaultLanguage = entity.DefaultLanguage,
                SecurityLevel = entity.SecurityLevel.ToString()
            });
    }
}
