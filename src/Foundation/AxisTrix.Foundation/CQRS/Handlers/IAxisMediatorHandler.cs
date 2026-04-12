using AxisResult;
using AxisTrix.CQRS.Commands;
using AxisTrix.CQRS.Queries;

namespace AxisTrix.CQRS.Handlers;

public interface IAxisMediatorHandler
{
    Task<AxisResult.AxisResult> ExecuteAsync<TCommand>(TCommand command) where TCommand : IAxisCommand;

    Task<AxisResult<TResponse>> ExecuteAsync<TCommand, TResponse>(TCommand command)
        where TCommand : IAxisCommand<TResponse>
        where TResponse : IAxisCommandResponse;

    Task<AxisResult<TResponse>> QueryAsync<TQuery, TResponse>(TQuery query)
        where TQuery : IAxisQuery<TResponse>
        where TResponse : IAxisQueryResponse;

    IAsyncEnumerable<TItem> StreamAsync<TQuery, TItem>(TQuery query)
        where TQuery : IAxisStreamQuery<TItem>;
}
