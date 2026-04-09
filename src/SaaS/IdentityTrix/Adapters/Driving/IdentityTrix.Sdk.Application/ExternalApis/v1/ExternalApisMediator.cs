using AxisTrix;
using AxisTrix.Results;
using IndentityTrix.Contracts.ExternalApis.v1;
using IndentityTrix.Contracts.ExternalApis.v1.AddExternalApi;
using IndentityTrix.Contracts.ExternalApis.v1.GenerateNewSecret;
using IndentityTrix.Contracts.ExternalApis.v1.GetById;

namespace IdentityTrix.Sdk.Application.ExternalApis.v1;

internal class ExternalApisMediator(IAxisMediator mediator) : IExternalApisMediator
{
    public Task<AxisResult<AddExternalApiResponse>> AddAsync(AddExternalApiCommand command)
        => mediator.Cqrs.ExecuteAsync<AddExternalApiCommand, AddExternalApiResponse>(command);

    public Task<AxisResult<GetExternalApiByIdResponse>> GetByIdAsync(GetExternalApiByIdQuery query)
        => mediator.Cqrs.QueryAsync<GetExternalApiByIdQuery, GetExternalApiByIdResponse>(query);

    public Task<AxisResult<GenerateNewExternalApiSecretResponse>> GenerateNewSecretAsync(GenerateNewExternalApiSecretCommand command)
        => mediator.Cqrs.ExecuteAsync<GenerateNewExternalApiSecretCommand, GenerateNewExternalApiSecretResponse>(command);

}
