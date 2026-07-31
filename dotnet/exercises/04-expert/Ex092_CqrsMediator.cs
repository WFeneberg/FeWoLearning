namespace FeWoLearning.Exercises.Expert;

// Exercise 092 — CQRS + mediator pattern (expert).
// Goal:   Implement a minimal mediator that dispatches Command and Query objects
//         to their registered handlers, decoupling callers from handler lookup.
// Drills: CQRS, mediator pattern, generics with variance, dependency inversion.

/// <summary>Marker for a write operation that mutates state and returns nothing.</summary>
/// <remarks>Named <c>ICqrsCommand</c> (not <c>ICommand</c>) to avoid colliding with
/// <see cref="System.Windows.Input.ICommand"/>, which other exercises in this
/// namespace also use.</remarks>
public interface ICqrsCommand
{
}

/// <summary>Marker for a read operation that returns a projection of type <typeparamref name="TResult"/>.</summary>
public interface ICqrsQuery<TResult>
{
}

/// <summary>Handles a single command type.</summary>
public interface ICqrsCommandHandler<in TCommand> where TCommand : ICqrsCommand
{
    void Handle(TCommand command);
}

/// <summary>Handles a single query type, producing a <typeparamref name="TResult"/> projection.</summary>
public interface ICqrsQueryHandler<in TQuery, out TResult> where TQuery : ICqrsQuery<TResult>
{
    TResult Handle(TQuery query);
}

/// <summary>
/// Minimal in-process mediator: routes each Command/Query to the single handler
/// registered for its concrete type. Unrouted messages must fail loudly.
/// </summary>
public sealed class CqrsMediator
{
    public CqrsMediator() => throw new NotImplementedException();

    /// <summary>Registers the only handler allowed to serve <typeparamref name="TCommand"/>.</summary>
    public void RegisterCommandHandler<TCommand>(ICqrsCommandHandler<TCommand> handler)
        where TCommand : ICqrsCommand
        => throw new NotImplementedException();

    /// <summary>Registers the only handler allowed to serve <typeparamref name="TQuery"/>.</summary>
    public void RegisterQueryHandler<TQuery, TResult>(ICqrsQueryHandler<TQuery, TResult> handler)
        where TQuery : ICqrsQuery<TResult>
        => throw new NotImplementedException();

    /// <summary>Dispatches <paramref name="command"/> to its registered handler.</summary>
    /// <exception cref="InvalidOperationException">No handler registered for the command's type.</exception>
    public void Send(ICqrsCommand command) => throw new NotImplementedException();

    /// <summary>Dispatches <paramref name="query"/> to its registered handler and returns the projection.</summary>
    /// <exception cref="InvalidOperationException">No handler registered for the query's type.</exception>
    public TResult Send<TResult>(ICqrsQuery<TResult> query) => throw new NotImplementedException();
}
