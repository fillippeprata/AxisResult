using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityById;

public record GetAxisIdentityByIdResponse : IAxisQueryResponse
{
    public required string AxisIdentityId { get; init; }
    public required bool IsIndividual { get; init; }
    public required string Document { get; init; }
    public required string CountryId { get; init; }
    public required string DisplayName { get; init; }
    public required string DefaultLanguage { get; init; }
    public required string SecurityLevel { get; init; }
}
