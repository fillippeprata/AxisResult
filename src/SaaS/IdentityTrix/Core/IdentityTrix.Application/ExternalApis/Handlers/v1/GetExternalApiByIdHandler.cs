using AxisTrix.CQRS.Queries;
using AxisTrix.Results;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;
using IndentityTrix.Contracts.ExternalApis.v1.GetById;

namespace IdentityTrix.Application.ExternalApis.Handlers.v1;

internal class GetExternalApiByIdHandler(
    IExternalApiReaderPort readerPort
) : IAxisQueryHandler<GetExternalApiByIdQuery, GetExternalApiByIdResponse>
{
    public Task<AxisResult<GetExternalApiByIdResponse>> HandleAsync(GetExternalApiByIdQuery cmd)
        => readerPort.GetExternalApiByIdAsync(cmd.ExternalApiId)
            .MapAsync<IExternalApiEntityProperties, GetExternalApiByIdResponse>(
                entity => new GetExternalApiByIdResponse
                {
                    ExternalApiId = entity.ExternalApiId,
                    Name = entity.ApiName
                });
}
