namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex044;

/// <summary>The write model: normalised, and shaped for changing one thing at a time.</summary>
public sealed record OrderRow(string OrderId, string CustomerId, decimal Amount);

/// <summary>
/// The read model: denormalised, and shaped for the one question the screen asks. It is
/// not a mirror of the write model, and that is the point of having it.
/// </summary>
public sealed record CustomerSummary(string CustomerId, int OrderCount, decimal TotalSpent);

public sealed class WriteStore
{
    private readonly Dictionary<string, OrderRow> _orders = [];

    public int Reads { get; private set; }

    public void Save(OrderRow order) => _orders[order.OrderId] = order;

    public IReadOnlyList<OrderRow> All()
    {
        Reads++;
        return [.. _orders.Values];
    }
}

// Exercise 044 — CqrsReadModel (services-data).
// Goal:   Serve queries from a separate, denormalised store, and be honest about the
//         window in which it is out of date.
// Drills: separate read store, eventual consistency, staleness window.
// Passes: write, then read - the read model still shows the OLD answer.
//         Project()        - after it runs, the read model is current.
//         THE ONE           - a query NEVER touches the write store: WriteStore.Reads
//                             stays at whatever projection left it, no matter how many
//                             queries are served.
//         shape             - the summary is per CUSTOMER with a count and a total, which
//                             is not a row the write store has at all.
//         a query for an unknown customer returns null rather than throwing.
//
// The Reads counter is what stops this from being CQRS in name only. A "read model" that
// falls back to the write store when it has no entry is the most natural thing in the
// world to write, passes every value assertion, and quietly reintroduces exactly the
// load the separation existed to remove - while still being described as eventually
// consistent, which it now is not.
public sealed class CustomerSummaryReadModel(WriteStore writeStore)
{
    /// <summary>Rebuild the read model from the write store. This is the only time it may be touched.</summary>
    public void Project() =>
        throw new NotImplementedException(
            "TODO: Ex044 - read the write store once and rebuild the per-customer summaries");

    /// <summary>Answer from the read model alone.</summary>
    public CustomerSummary? Query(string customerId) =>
        throw new NotImplementedException(
            "TODO: Ex044 - return the projected summary, or null - and do NOT consult the write store");
}
