namespace FeWoLearning.Architecture.Exercises.Domain.Ex083;

public sealed class InvariantViolationException(string message) : Exception(message);

/// <summary>
/// A reference to another aggregate. NOT the aggregate itself - that is the whole
/// exercise, and the type exists to make the difference impossible to miss.
/// </summary>
public readonly record struct CustomerId(string Value);

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice)
{
    public decimal Total => Quantity * UnitPrice;
}

/// <summary>Counts how many aggregates one save touched. That count is the mechanism.</summary>
public sealed class SavedAggregates
{
    public List<string> Saved { get; } = [];
}

// Exercise 083 — AggregateTransactionBoundary (reference solution).
public sealed class Order
{
    private readonly List<OrderLine> _lines = [];
    private readonly decimal _creditLimitAtCreation;

    public Order(string id, CustomerId customer, decimal creditLimitAtCreation)
    {
        Id = id;

        // The customer is held BY ID. The order cannot read the customer's current credit
        // limit, and that is the boundary doing its job rather than a missing feature.
        Customer = customer;

        // A snapshot, and it may be stale. That is not a bug to fix by reaching into the
        // customer aggregate - it is the price of the boundary, and the compensating
        // action when it turns out wrong is a business decision rather than a locking one.
        _creditLimitAtCreation = creditLimitAtCreation;
    }

    public string Id { get; }

    public CustomerId Customer { get; }

    public decimal Total => _lines.Sum(l => l.Total);

    public IReadOnlyList<OrderLine> Lines => _lines;

    public void AddLine(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        // Checked BEFORE the line is added. Adding first and rolling back afterwards
        // leaves the aggregate briefly invalid, and "briefly" is long enough for an event
        // handler or a serialiser to see it.
        if (Total + line.Total > _creditLimitAtCreation)
            throw new InvariantViolationException(
                $"Order {Id} would reach {Total + line.Total}, over its limit of {_creditLimitAtCreation}.");

        _lines.Add(line);
    }
}

public static class Ex083_AggregateTransactionBoundary
{
    public static void PlaceOrder(Order order, SavedAggregates saved) =>
        // ONE aggregate. A handler that also updates the customer's running balance here
        // has made two aggregates one, and every order for a busy customer now contends on
        // that row - correct, and it does not work.
        saved.Saved.Add("order:" + order.Id);

    public static void ApplyToCustomerBalance(Order order, SavedAggregates saved) =>
        // Separately, afterwards, in reaction to the event. The balance is briefly behind,
        // which is what "eventually consistent" costs and buys.
        saved.Saved.Add("customer:" + order.Customer.Value);
}
