using Axis;
using AxisMediator.Contracts;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Contracts.ExternalApis.v1.AddExternalApi;
using TenantTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;

namespace TenantTrix.Sdk.Application.ExternalApis.v1;

internal class ExternalApisMediator(IAxisMediator mediator) : IExternalApisMediator
{
    public Task<AxisResult<AddExternalApiResponse>> AddAsync(AddExternalApiCommand command)
        => mediator.Cqrs.ExecuteAsync<AddExternalApiCommand, AddExternalApiResponse>(command);

    public Task<AxisResult<GetExternalApiByIdResponse>> GetByIdAsync(GetExternalApiByIdQuery query)
        => mediator.Cqrs.QueryAsync<GetExternalApiByIdQuery, GetExternalApiByIdResponse>(query);

    public Task<AxisResult<GenerateNewExternalApiSecretResponse>> GenerateNewExternalApiSecretAsync(GenerateNewExternalApiSecretCommand command)
        => mediator.Cqrs.ExecuteAsync<GenerateNewExternalApiSecretCommand, GenerateNewExternalApiSecretResponse>(command);

}
