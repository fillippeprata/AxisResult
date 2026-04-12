namespace AxisTrix.Bus;

public interface IAxisEventHandler<in TEvent> where TEvent : IAxisEvent
{
    Task<AxisResult.AxisResult> HandleAsync(TEvent @event);
}
