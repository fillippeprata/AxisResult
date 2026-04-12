using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Bus.Memory;

public class MemoryBusAdapter(IServiceProvider serviceProvider) : IAxisBus
{
    public async Task<AxisResult> PublishAsync<TEvent>(TEvent @event, params string[] topics) where TEvent : IAxisEvent
    {
        var handlers = serviceProvider.GetServices<IAxisEventHandler<TEvent>>().ToList();

        if (handlers.Count == 0)
            return AxisResult.Ok();

        var tasks = handlers.Select(handler => handler.HandleAsync(@event));

        var results = await Task.WhenAll(tasks);

        return AxisResult.Combine(results);
    }
}
