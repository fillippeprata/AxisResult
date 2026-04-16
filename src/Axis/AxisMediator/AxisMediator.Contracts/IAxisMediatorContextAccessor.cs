namespace AxisMediator.Contracts;

public interface IAxisMediatorContextAccessor
{
    string? OriginId { get; set; }
    string? JourneyId { get; set; }
    CancellationToken CancellationToken { get; set; }
    string? PersonId { get; set; }
}
