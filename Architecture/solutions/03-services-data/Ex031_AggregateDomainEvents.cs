namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex031;

public interface IDomainEvent;

public sealed record OrderSubmitted(string OrderId, decimal Total) : IDomainEvent;

public sealed record LineAdded(string OrderId, string Sku, int Quantity) : IDomainEvent;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _pending = [];

    public IReadOnlyList<IDomainEvent> PendingEvents => _pending;

    protected void Raise(IDomainEvent domainEvent) => _pending.Add(domainEvent);

    public void ClearEvents() => _pending.Clear();
}

// Exercise 031 — AggregateDomainEvents (reference solution).
public sealed class Order : AggregateRoot
{
    private readonly List<(string Sku, int Quantity, decimal Price)> _lines = [];

    public Order(string id) => Id = id;

    public string Id { get; }

    public bool IsSubmitted { get; private set; }

    public decimal Total => _lines.Sum(l => l.Quantity * l.Price);

    public void AddLine(string sku, int quantity, decimal price)
    {
        // The invariant is checked BEFORE anything is recorded. An event raised for a
        // change that was then rejected is a lie the aggregate told about itself.
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Quantity must be positive.");

        _lines.Add((sku, quantity, price));
        Raise(new LineAdded(Id, sku, quantity));
    }

    public void Submit()
    {
        if (_lines.Count == 0)
            throw new InvalidOperationException("An empty order cannot be submitted.");

        IsSubmitted = true;
        Raise(new OrderSubmitted(Id, Total));
    }
}

public static class Ex031_AggregateDomainEvents
{
    public static void SaveAndDispatch(Order order, Action commit, Action<IDomainEvent> dispatch)
    {
        ArgumentNullException.ThrowIfNull(order);

        // Commit FIRST. If it throws, the exception leaves this method and the events
        // are still pending - which is correct, because as far as the outside world is
        // concerned nothing happened. Publishing inside AddLine instead tells the rest
        // of the system about an order that was never stored, and the consumers are not
        // wrong to believe it: they were told.
        commit();

        // Snapshot before clearing: a handler is allowed to touch the aggregate.
        foreach (var domainEvent in order.PendingEvents.ToArray())
            dispatch(domainEvent);

        order.ClearEvents();
    }
}
