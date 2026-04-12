using AxisTrix.CQRS.Queries;
using AxisTrix.Results;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;
using IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.GetExternalApiById;

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
                    Name = entity.ApiName
                });
}
