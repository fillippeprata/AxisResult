using AxisTrix.Results;
using IndentityTrix.Contracts.ExternalApis.v1.AddExternalApi;
using IndentityTrix.Contracts.ExternalApis.v1.GenerateNewSecret;
using IndentityTrix.Contracts.ExternalApis.v1.GetById;

namespace IndentityTrix.Contracts.ExternalApis.v1;

public interface IExternalApisMediator
{
    Task<AxisResult<AddExternalApiResponse>> AddAsync(AddExternalApiCommand command);
    Task<AxisResult<GetExternalApiByIdResponse>> GetByIdAsync(GetExternalApiByIdQuery query);
    Task<AxisResult<GenerateNewExternalApiSecretResponse>> GenerateNewSecretAsync(GenerateNewExternalApiSecretCommand command);
}
