namespace AxisTrix.Accessor;

public interface IAxisMediatorContextAccessor
{
    string? OriginId { get; set; }
    string? JourneyId { get; set; }
    CancellationToken CancellationToken { get; set; }
    string? PersonId { get; set; }
}

internal class AxisMediatorContextAccessor : IAxisMediatorContextAccessor
{
    private static readonly AsyncLocal<string?> _originId = new();
    private static readonly AsyncLocal<string?> _journeyId = new();
    private static readonly AsyncLocal<CancellationToken> _cancellationToken = new();
    private static readonly AsyncLocal<string?> _personId = new();

    public string? OriginId
    {
        get => _originId.Value;
        set => _originId.Value = value;
    }

    public string? JourneyId
    {
        get => _journeyId.Value;
        set => _journeyId.Value = value;
    }

    public CancellationToken CancellationToken
    {
        get => _cancellationToken.Value;
        set => _cancellationToken.Value = value;
    }

    public string? PersonId
    {
        get => _personId.Value;
        set => _personId.Value = value;
    }
}
