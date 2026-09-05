namespace FeWoLearning.Architecture.Exercises.Web.Ex006;

public sealed record Product(string Sku, string Name, int Stock);

/// <summary>
/// Counts reads and writes. That instrumentation is the whole grading mechanism here:
/// "a query does not write" is a claim about what the handler DID, and the only honest
/// way to check it is to count.
/// </summary>
public interface IProductStore
{
    int Reads { get; }
    int Writes { get; }

    Product? Find(string sku);
    IReadOnlyList<Product> All();
    void Upsert(Product product);
}

public sealed class ProductStore : IProductStore
{
    private readonly Dictionary<string, Product> _products = [];

    public int Reads { get; private set; }
    public int Writes { get; private set; }

    public Product? Find(string sku)
    {
        Reads++;
        return _products.GetValueOrDefault(sku);
    }

    public IReadOnlyList<Product> All()
    {
        Reads++;
        return [.. _products.Values];
    }

    public void Upsert(Product product)
    {
        Writes++;
        _products[product.Sku] = product;
    }

    /// <summary>Test setup only - does not move the counters.</summary>
    public void Seed(Product product) => _products[product.Sku] = product;
}

public sealed record RestockCommand(string Sku, string Name, int Quantity);

public sealed record LowStockQuery(int Threshold);

// Exercise 006 — CqrsCommandQuery (web).
// Goal:   Split one model into a command side that changes state and a query side that
//         only reads it, and keep each honest about which one it is.
// Drills: command/query separation, distinct handler contracts, write instrumentation.
// Passes: RestockCommandHandler - raises an existing product's stock by Quantity;
//                    creates the product with that stock when the SKU is unknown; and
//                    performs EXACTLY ONE write per command.
//         LowStockQueryHandler  - returns the products whose stock is strictly below
//                    the threshold, ordered by stock ascending, and performs ZERO
//                    writes.
//
// The zero-writes clause is what makes this exercise more than a naming convention. A
// query handler that "helpfully" caches its result back into the store, or that
// normalises a record while reading it, has quietly become a command - and every
// caller that believed reads were safe to retry, route to a replica or run twice is
// now wrong.
public sealed class RestockCommandHandler(IProductStore store)
{
    /// <summary>Commands change state and return nothing.</summary>
    public void Handle(RestockCommand command) =>
        throw new NotImplementedException(
            "TODO: Ex006 - raise the product's stock by Quantity, creating it if the SKU is unknown, with exactly one write");
}

public sealed class LowStockQueryHandler(IProductStore store)
{
    /// <summary>Queries return data and change nothing.</summary>
    public IReadOnlyList<Product> Handle(LowStockQuery query) =>
        throw new NotImplementedException(
            "TODO: Ex006 - return products with stock strictly below the threshold, ordered by stock ascending, writing nothing");
}
