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

// Exercise 044 — CqrsReadModel (reference solution).
public sealed class CustomerSummaryReadModel(WriteStore writeStore)
{
    private Dictionary<string, CustomerSummary> _summaries = [];

    public void Project() =>
        // One pass over the write store, here and nowhere else. The grouping is the
        // read model's whole value: the screen asks "how much has this customer spent",
        // and the write store has no such row.
        _summaries = writeStore.All()
            .GroupBy(o => o.CustomerId)
            .ToDictionary(
                g => g.Key,
                g => new CustomerSummary(g.Key, g.Count(), g.Sum(o => o.Amount)));

    public CustomerSummary? Query(string customerId) =>
        // No fallback to the write store. Falling back is the most natural thing in the
        // world to write, passes every value assertion, and quietly reintroduces exactly
        // the load the separation existed to remove.
        _summaries.GetValueOrDefault(customerId);
}
