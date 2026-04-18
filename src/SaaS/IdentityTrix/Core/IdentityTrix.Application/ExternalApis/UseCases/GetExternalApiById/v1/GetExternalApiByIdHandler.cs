using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis.UseCases.GetExternalApiById.v1;

internal class GetExternalApiByIdHandler(
    IExternalApisReaderPort readerPort
) : IAxisQueryHandler<GetExternalApiByIdQuery, GetExternalApiByIdResponse>
{
    public Task<AxisResult<GetExternalApiByIdResponse>> HandleAsync(GetExternalApiByIdQuery cmd)
        => readerPort.GetByIdAsync(cmd.ExternalApiId)
            .MapAsync<IExternalApiEntityProperties, GetExternalApiByIdResponse>(
                entity => new GetExternalApiByIdResponse
                {
                    ExternalApiId = entity.ExternalApiId,
                    Name = entity.ApiName,
                    TenantId = entity.TenantId
                });
}
