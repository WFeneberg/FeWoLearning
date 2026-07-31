namespace FeWoLearning.Exercises.Expert;

// Exercise 092 — CQRS + mediator pattern (reference solution).
// A dictionary keyed by the message's runtime Type maps to a boxed delegate
// closing over the strongly-typed handler, so dispatch stays allocation-light
// and type-safe without reflection at call time.
public interface ICqrsCommand
{
}

public interface ICqrsQuery<TResult>
{
}

public interface ICqrsCommandHandler<in TCommand> where TCommand : ICqrsCommand
{
    void Handle(TCommand command);
}

public interface ICqrsQueryHandler<in TQuery, out TResult> where TQuery : ICqrsQuery<TResult>
{
    TResult Handle(TQuery query);
}

public sealed class CqrsMediator
{
    private readonly Dictionary<Type, Action<ICqrsCommand>> _commandHandlers = new();
    private readonly Dictionary<Type, Func<object, object?>> _queryHandlers = new();

    public CqrsMediator()
    {
    }

    public void RegisterCommandHandler<TCommand>(ICqrsCommandHandler<TCommand> handler)
        where TCommand : ICqrsCommand
    {
        ArgumentNullException.ThrowIfNull(handler);
        _commandHandlers[typeof(TCommand)] = command => handler.Handle((TCommand)command);
    }

    public void RegisterQueryHandler<TQuery, TResult>(ICqrsQueryHandler<TQuery, TResult> handler)
        where TQuery : ICqrsQuery<TResult>
    {
        ArgumentNullException.ThrowIfNull(handler);
        _queryHandlers[typeof(TQuery)] = query => handler.Handle((TQuery)query);
    }

    public void Send(ICqrsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var type = command.GetType();
        if (!_commandHandlers.TryGetValue(type, out var dispatch))
            throw new InvalidOperationException($"No command handler registered for '{type.Name}'.");
        dispatch(command);
    }

    public TResult Send<TResult>(ICqrsQuery<TResult> query)
    {
        ArgumentNullException.ThrowIfNull(query);
        var type = query.GetType();
        if (!_queryHandlers.TryGetValue(type, out var dispatch))
            throw new InvalidOperationException($"No query handler registered for '{type.Name}'.");
        return (TResult)dispatch(query)!;
    }
}
