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

// Exercise 083 — AggregateTransactionBoundary (domain).
// Goal:   Decide what has to be consistent RIGHT NOW, and let everything else be
//         consistent shortly.
// Drills: aggregate roots, invariants, referencing by id, one aggregate per transaction.
// Passes: invariant   - the order's total may not exceed the credit limit it was created
//                       with; AddLine refuses the line that would break it, and the order
//                       is unchanged afterwards.
//         boundary    - Order holds a CustomerId, not a Customer. It cannot read the
//                       customer's current credit limit, and it does not try.
//         THE ONE      - placing an order saves ONE aggregate. A handler that also updates
//                       the customer's running balance in the same breath has made two
//                       aggregates one, and every order now contends on the customer row.
//         eventual    - the customer's balance is updated by reacting to the event
//                       afterwards, and the count shows the two saves are separate.
//
// An aggregate is a consistency boundary and nothing else: the set of things that must be
// correct together, in one transaction, at every instant. Everything outside it is allowed
// to be correct a moment later.
//
// Getting the boundary too big is the common failure and it is invisible until load
// arrives. Order and Customer in one transaction reads beautifully - the balance is always
// exactly right - and it means every order for a busy customer contends on that one row.
// The system is correct and it does not work.
//
// The credit limit being a SNAPSHOT taken when the order was created is the honest version
// of the trade. It may be stale. That is not a bug to be fixed by reaching into the
// customer aggregate; it is the price of the boundary, and the compensating action when it
// turns out wrong is a business decision rather than a locking one.
public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public Order(string id, CustomerId customer, decimal creditLimitAtCreation) =>
        throw new NotImplementedException("TODO: Ex083 - assign the id, the customer reference and the snapshot");

    public string Id =>
        throw new NotImplementedException("TODO: Ex083 - the order's own id");

    public CustomerId Customer =>
        throw new NotImplementedException("TODO: Ex083 - the referenced customer, by id");

    public decimal Total => _lines.Sum(l => l.Total);

    public IReadOnlyList<OrderLine> Lines => _lines;

    /// <summary>Add a line, unless it would push the total past the credit limit.</summary>
    public void AddLine(OrderLine line) =>
        throw new NotImplementedException(
            "TODO: Ex083 - refuse a line that would break the limit, leaving the order untouched, otherwise add it");
}

public static class Ex083_AggregateTransactionBoundary
{
    /// <summary>
    /// Persist the order. Exactly one aggregate is saved here - record it in
    /// <paramref name="saved"/> as "order:{id}".
    /// </summary>
    public static void PlaceOrder(Order order, SavedAggregates saved) =>
        throw new NotImplementedException(
            "TODO: Ex083 - save the order and nothing else");

    /// <summary>
    /// The reaction, in its own transaction: update the customer's running balance.
    /// Record it as "customer:{id}".
    /// </summary>
    public static void ApplyToCustomerBalance(Order order, SavedAggregates saved) =>
        throw new NotImplementedException(
            "TODO: Ex083 - save the customer aggregate separately");
}
