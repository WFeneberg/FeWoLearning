namespace FeWoLearning.Architecture.Exercises.Web.Ex006;

public sealed record Product(string Sku, string Name, int Stock);

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

    public void Seed(Product product) => _products[product.Sku] = product;
}

public sealed record RestockCommand(string Sku, string Name, int Quantity);

public sealed record LowStockQuery(int Threshold);

// Exercise 006 — CqrsCommandQuery (reference solution).
public sealed class RestockCommandHandler(IProductStore store)
{
    public void Handle(RestockCommand command)
    {
        var existing = store.Find(command.Sku);

        // One Upsert on both paths. A "delete then insert" or a "write the shell, then
        // write the stock" implementation is two writes, and in a real store that is
        // two rows of history, two change events and a window where the row is wrong.
        var updated = existing is null
            ? new Product(command.Sku, command.Name, command.Quantity)
            : existing with { Stock = existing.Stock + command.Quantity };

        store.Upsert(updated);
    }
}

public sealed class LowStockQueryHandler(IProductStore store)
{
    public IReadOnlyList<Product> Handle(LowStockQuery query) =>
        [.. store.All()
            .Where(p => p.Stock < query.Threshold)   // strictly below: the threshold itself is fine
            .OrderBy(p => p.Stock)
            .ThenBy(p => p.Sku, StringComparer.Ordinal)];
}
