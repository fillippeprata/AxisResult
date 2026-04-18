using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using TenantTrix.Ports.ExternalApis;
using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.Application.ExternalApis.UseCases.GetExternalApiById.v1;

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
