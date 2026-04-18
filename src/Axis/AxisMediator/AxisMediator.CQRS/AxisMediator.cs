using System.Diagnostics;
using Axis;
using AxisMediator.Contracts;
using AxisMediator.Contracts.CQRS.Handlers;

namespace AxisMediator;

internal class AxisMediator : IAxisMediator, IDisposable
{
    private readonly IAxisMediatorAccessor _accessor;
    public IAxisMediatorHandler Cqrs { get; }
    public string? OriginId { get; }
    public string? JourneyId { get; }
    public CancellationToken CancellationToken { get; }
    public PersonData? AuthenticatedUser { get; }

    public AxisMediator(
        IAxisMediatorHandler cqrs,
        IAxisMediatorAccessor accessor,
        IAxisMediatorContextAccessor contextAccessor
    )
    {
        Cqrs = cqrs;
        _accessor = accessor;
        _accessor.AxisMediator = this;

        OriginId = contextAccessor.OriginId;
        JourneyId = contextAccessor.JourneyId;
        CancellationToken = contextAccessor.CancellationToken;
        AuthenticatedUser = contextAccessor.PersonId is { } personId
            ? new PersonData(personId, "Mock User", "mock", "pt-br")
            : null;
    }

    public Task<AxisResult<PersonData>> GetPersonFromCacheAsync(string id) => Task.FromResult(AxisResult.Ok(new PersonData(id,"Test Person","temp", "pt-br")));

    public string TraceId { get; } = ResolveTraceId();
    private static string ResolveTraceId()
    {
        return Activity.Current is not { } activity
            ? Guid.NewGuid().ToString()
            : activity.TraceId.ToString();
    }

    public void Dispose() => _accessor.AxisMediator = null;

}
