using AxisTrix;
using IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;
using IdentityTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;

namespace IdentityTrix.Contracts.ExternalApis.v1;

public interface IExternalApisMediator
{
    Task<AxisResult<AddExternalApiResponse>> AddAsync(AddExternalApiCommand command);
    Task<AxisResult<GetExternalApiByIdResponse>> GetByIdAsync(GetExternalApiByIdQuery query);
    Task<AxisResult<GenerateNewExternalApiSecretResponse>> GenerateNewExternalApiSecretAsync(GenerateNewExternalApiSecretCommand command);
}
