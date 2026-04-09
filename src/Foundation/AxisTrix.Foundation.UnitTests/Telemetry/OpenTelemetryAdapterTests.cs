using System.Diagnostics;
using AxisTrix.Telemetry;
using AxisTrix.Telemetry.OpenTelemetry;

namespace AxisTrix.Mediator.UnitTests.Telemetry;

public class OpenTelemetryAdapterTests : IDisposable
{
    private readonly ActivityListener _listener;
    private readonly List<Activity> _activities = [];

    public OpenTelemetryAdapterTests()
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == OpenTelemetryAdapter.SourceName,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => _activities.Add(activity)
        };
        ActivitySource.AddActivityListener(_listener);
    }

    [Fact]
    public void StartSpan_CreatesActivity_WithCorrectName()
    {
        var adapter = new OpenTelemetryAdapter();

        using var span = adapter.StartSpan("TestOperation");

        Assert.NotNull(span);
        Assert.Single(_activities);
        Assert.Equal("TestOperation", _activities[0].OperationName);
    }

    [Fact]
    public void StartSpan_ServerKind_MapsToActivityKindServer()
    {
        var adapter = new OpenTelemetryAdapter();

        using var span = adapter.StartSpan("ServerOp", AxisSpanKind.Server);

        Assert.Single(_activities);
        Assert.Equal(ActivityKind.Server, _activities[0].Kind);
    }

    [Fact]
    public void CurrentTraceId_ReturnsActiveTraceId()
    {
        var adapter = new OpenTelemetryAdapter();

        using var span = adapter.StartSpan("TraceIdTest");

        Assert.False(string.IsNullOrWhiteSpace(adapter.CurrentTraceId));
        Assert.False(string.IsNullOrWhiteSpace(adapter.CurrentSpanId));
    }

    [Fact]
    public void IncrementCounter_DoesNotThrow()
    {
        var adapter = new OpenTelemetryAdapter();

        var act = () => adapter.IncrementCounter("test.counter", 1,
            new KeyValuePair<string, object?>("key", "value"));

        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    [Fact]
    public void RecordHistogram_DoesNotThrow()
    {
        var adapter = new OpenTelemetryAdapter();

        var act = () => adapter.RecordHistogram("test.histogram", 42.5,
            new KeyValuePair<string, object?>("key", "value"));

        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    public void Dispose() => _listener.Dispose();
}
