using FeWoLearning.Architecture.Exercises.Web.Ex006;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex006_CqrsCommandQueryTests
{
    private static ProductStore SeededStore()
    {
        var store = new ProductStore();
        store.Seed(new Product("A", "Anvil", 2));
        store.Seed(new Product("B", "Bolt", 10));
        store.Seed(new Product("C", "Cog", 5));
        return store;
    }

    [Fact]
    public void Command_Raises_The_Stock_Of_An_Existing_Product()
    {
        var store = SeededStore();

        new RestockCommandHandler(store).Handle(new RestockCommand("A", "Anvil", 3));

        Assert.Equal(5, store.Find("A")!.Stock);
    }

    [Fact]
    public void Command_Creates_The_Product_When_The_Sku_Is_Unknown()
    {
        var store = SeededStore();

        new RestockCommandHandler(store).Handle(new RestockCommand("Z", "Zip", 7));

        var created = store.Find("Z");
        Assert.NotNull(created);
        Assert.Equal("Zip", created.Name);
        Assert.Equal(7, created.Stock);
    }

    [Fact]
    public void Mechanism_A_Command_Performs_Exactly_One_Write()
    {
        // Asserting only the final stock is satisfied by delete-then-insert, or by
        // writing a shell row and then writing the stock. Both leave a real store with
        // two rows of history, two change events, and a window in which the row is
        // wrong. The count is the only thing that separates them.
        var store = SeededStore();

        new RestockCommandHandler(store).Handle(new RestockCommand("A", "Anvil", 3));

        Assert.Equal(1, store.Writes);
    }

    [Fact]
    public void Query_Returns_Only_Products_Below_The_Threshold_Ordered_By_Stock()
    {
        var store = SeededStore();

        var result = new LowStockQueryHandler(store).Handle(new LowStockQuery(6));

        Assert.Equal(["A", "C"], result.Select(p => p.Sku));
    }

    [Fact]
    public void Query_Excludes_A_Product_Sitting_Exactly_On_The_Threshold()
    {
        // "Below" is strict. The boundary is where an off-by-one silently changes which
        // products a reorder job buys.
        var store = SeededStore();

        var result = new LowStockQueryHandler(store).Handle(new LowStockQuery(5));

        Assert.Equal(["A"], result.Select(p => p.Sku));
    }

    [Fact]
    public void Mechanism_A_Query_Performs_Zero_Writes()
    {
        // The fact this exercise exists for. A query handler that caches its result
        // back into the store, or normalises a record while reading it, has quietly
        // become a command - and every caller that believed reads were safe to retry,
        // route to a replica, or run twice is now wrong.
        var store = SeededStore();

        new LowStockQueryHandler(store).Handle(new LowStockQuery(6));

        Assert.Equal(0, store.Writes);
    }
}
