using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityById;
using DataPrivacyTrix.Ports.Registration;
using DataPrivacyTrix.SharedKernel.Registration;

namespace DataPrivacyTrix.Application.Registration.UseCases.GetAxisIdentityById.v1;

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
