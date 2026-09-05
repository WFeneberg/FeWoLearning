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

// Exercise 031 — AggregateDomainEvents (services-data).
// Goal:   Let an aggregate record what happened while it enforces its invariants, and
//         publish those records only once the change is actually durable.
// Drills: aggregate invariants, event collection, dispatch after commit.
// Passes: AddLine  - appends the line and RAISES LineAdded; refuses a non-positive
//                    quantity, raising nothing.
//         Submit   - refuses an empty order, raising nothing; otherwise marks the order
//                    submitted and raises OrderSubmitted carrying the correct total.
//         SaveAndDispatch - on a successful commit, dispatches every pending event in
//                    order and then clears them, so a second save dispatches nothing.
//         THE ONE   - when the commit THROWS, nothing is dispatched and the events stay
//                    pending.
//
// "Nothing is dispatched when the commit throws" is the fact worth the exercise. Raising
// the event inside AddLine - publishing it there and then - passes every ordering
// assertion and tells the rest of the system about an order that was never stored. The
// consumers are not wrong to believe it: they were told.
public sealed class Order : AggregateRoot
{
    private readonly List<(string Sku, int Quantity, decimal Price)> _lines = [];

    public Order(string id) => Id = id;

    public string Id { get; }

    public bool IsSubmitted { get; private set; }

    public decimal Total => _lines.Sum(l => l.Quantity * l.Price);

    public void AddLine(string sku, int quantity, decimal price) =>
        throw new NotImplementedException(
            "TODO: Ex031 - refuse a quantity of zero or less, otherwise append the line and raise LineAdded");

    public void Submit() =>
        throw new NotImplementedException(
            "TODO: Ex031 - refuse an empty order, otherwise mark it submitted and raise OrderSubmitted with the total");
}

public static class Ex031_AggregateDomainEvents
{
    /// <summary>
    /// Persist the aggregate by calling <paramref name="commit"/>, then publish what
    /// happened. If <paramref name="commit"/> throws, the exception propagates and
    /// nothing is published.
    /// </summary>
    public static void SaveAndDispatch(Order order, Action commit, Action<IDomainEvent> dispatch) =>
        throw new NotImplementedException(
            "TODO: Ex031 - commit first, then dispatch every pending event in order, then clear them");
}
