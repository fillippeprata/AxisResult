using AxisResult;

namespace AxisTrix.CQRS.Commands;

public interface IAxisCommandHandler<in TCommand> where TCommand : IAxisCommand
{
    Task<AxisResult.AxisResult> HandleAsync(TCommand command);
}

public interface IAxisCommandHandler<in TCommand, TResponse>
    where TCommand : IAxisCommand<TResponse>
    where TResponse : IAxisCommandResponse
{
    Task<AxisResult<TResponse>> HandleAsync(TCommand command);
}
