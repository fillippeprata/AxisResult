using Axis;
using TenantTrix.Contracts.ExternalApis.v1.AddExternalApi;
using TenantTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;

namespace TenantTrix.Contracts.ExternalApis.v1;

public interface IExternalApisMediator
{
    Task<AxisResult<AddExternalApiResponse>> AddAsync(AddExternalApiCommand command);
    Task<AxisResult<GetExternalApiByIdResponse>> GetByIdAsync(GetExternalApiByIdQuery query);
    Task<AxisResult<GenerateNewExternalApiSecretResponse>> GenerateNewExternalApiSecretAsync(GenerateNewExternalApiSecretCommand command);
}
