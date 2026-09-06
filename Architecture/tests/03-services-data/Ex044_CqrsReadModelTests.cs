using FeWoLearning.Architecture.Exercises.ServicesData.Ex044;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex044_CqrsReadModelTests
{
    private static (WriteStore Store, CustomerSummaryReadModel ReadModel) Build()
    {
        var store = new WriteStore();
        return (store, new CustomerSummaryReadModel(store));
    }

    [Fact]
    public void Mechanism_A_Write_Is_Not_Visible_Until_The_Projection_Runs()
    {
        // The staleness window, asserted rather than hoped for. A read model that reads
        // through to the write store is always current, always correct, and is not a
        // read model - it is the write store with extra steps.
        var (store, readModel) = Build();
        store.Save(new OrderRow("o-1", "c-1", 10m));
        readModel.Project();

        store.Save(new OrderRow("o-2", "c-1", 5m));

        Assert.Equal(1, readModel.Query("c-1")!.OrderCount);
        Assert.Equal(10m, readModel.Query("c-1")!.TotalSpent);
    }

    [Fact]
    public void Projecting_Closes_The_Window()
    {
        var (store, readModel) = Build();
        store.Save(new OrderRow("o-1", "c-1", 10m));
        store.Save(new OrderRow("o-2", "c-1", 5m));

        readModel.Project();

        Assert.Equal(2, readModel.Query("c-1")!.OrderCount);
        Assert.Equal(15m, readModel.Query("c-1")!.TotalSpent);
    }

    [Fact]
    public void Mechanism_Queries_Never_Touch_The_Write_Store()
    {
        // The counter is what stops this being CQRS in name only. Falling back to the
        // write store on a miss passes every value assertion above and puts back exactly
        // the load the separation removed - while the design document still calls the
        // read model eventually consistent, which it no longer is.
        var (store, readModel) = Build();
        store.Save(new OrderRow("o-1", "c-1", 10m));
        readModel.Project();

        var readsAfterProjection = store.Reads;

        readModel.Query("c-1");
        readModel.Query("c-1");
        readModel.Query("nobody");

        Assert.Equal(readsAfterProjection, store.Reads);
    }

    [Fact]
    public void Adversarial_An_Unknown_Customer_Returns_Null_Rather_Than_Consulting_The_Write_Store()
    {
        // The miss is where the fallback is most tempting, so it gets its own fact.
        var (store, readModel) = Build();
        readModel.Project();
        var readsAfterProjection = store.Reads;

        Assert.Null(readModel.Query("never-ordered"));
        Assert.Equal(readsAfterProjection, store.Reads);
    }

    [Fact]
    public void The_Read_Model_Has_A_Shape_The_Write_Store_Does_Not()
    {
        // Per customer, with a count and a total - three orders collapse into one row.
        // A read model that mirrors the write model row for row has bought the
        // consistency problem without buying anything else.
        var (store, readModel) = Build();
        store.Save(new OrderRow("o-1", "c-1", 10m));
        store.Save(new OrderRow("o-2", "c-1", 5m));
        store.Save(new OrderRow("o-3", "c-2", 7m));

        readModel.Project();

        Assert.Equal(new CustomerSummary("c-1", 2, 15m), readModel.Query("c-1"));
        Assert.Equal(new CustomerSummary("c-2", 1, 7m), readModel.Query("c-2"));
    }

    [Fact]
    public void Projecting_Reads_The_Write_Store_Once()
    {
        // Rebuilding per customer, or per query, is the other way to make this expensive.
        var (store, readModel) = Build();
        store.Save(new OrderRow("o-1", "c-1", 10m));
        store.Save(new OrderRow("o-2", "c-2", 5m));

        var before = store.Reads;
        readModel.Project();

        Assert.Equal(before + 1, store.Reads);
    }
}
