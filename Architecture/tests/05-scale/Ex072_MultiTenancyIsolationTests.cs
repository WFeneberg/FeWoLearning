using FeWoLearning.Architecture.Exercises.Scale.Ex072;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex072_MultiTenancyIsolationTests
{
    private static (DocumentTable Table, TenantScopedRepository Acme, TenantScopedRepository Globex) Build()
    {
        var table = new DocumentTable();
        table.Seed(new Document("doc-1", "acme", "Acme roadmap"));
        table.Seed(new Document("doc-2", "globex", "Globex salaries"));
        table.Seed(new Document("doc-3", "acme", "Acme budget"));

        return (table, new TenantScopedRepository("acme", table), new TenantScopedRepository("globex", table));
    }

    [Fact]
    public void Listing_Returns_Only_This_Tenants_Documents()
    {
        var (_, acme, _) = Build();

        Assert.Equal(["doc-1", "doc-3"], acme.List().Select(d => d.Id).OrderBy(i => i));
    }

    [Fact]
    public void Mechanism_Another_Tenants_Document_Does_Not_Exist()
    {
        // Where multi-tenant systems leak, and they leak the same way every time: the id
        // is a primary key, the lookup is by primary key, and the tenant clause is the one
        // nobody adds because the row is already unique without it. The result returns any
        // tenant's document to anybody who can guess an id, and it passes every functional
        // test - because every functional test uses one tenant.
        var (_, acme, _) = Build();

        Assert.Null(acme.Find("doc-2"));
        Assert.NotNull(acme.Find("doc-1"));
    }

    [Fact]
    public void Adversarial_The_Miss_Is_Silent_Rather_Than_Informative()
    {
        // Null, not an exception naming the other tenant. "Document doc-2 belongs to
        // Globex" is a helpful message and an enumeration oracle: it confirms the id
        // exists and tells the caller whose it is.
        var (_, acme, _) = Build();

        Assert.Null(Record.Exception(() => acme.Find("doc-2")));
        Assert.Null(acme.Find("never-existed"));
    }

    [Fact]
    public void Adding_Stamps_The_Current_Tenant()
    {
        var (_, acme, globex) = Build();

        acme.Add(new Document("doc-4", "", "New acme doc"));

        Assert.Equal("acme", acme.Find("doc-4")!.TenantId);
        Assert.Null(globex.Find("doc-4"));
    }

    [Fact]
    public void Mechanism_A_Document_Naming_Another_Tenant_Is_Refused()
    {
        // Refused, not corrected. Silently re-stamping makes the write succeed and hides
        // that the caller mixed two tenants' data in one request - and the next bug of
        // that shape will not be a write.
        var (table, acme, _) = Build();
        var before = table.AllRows.Count;

        var failure = Assert.Throws<CrossTenantWriteException>(
            () => acme.Add(new Document("doc-4", "globex", "Sneaky")));

        Assert.Equal("acme", failure.ExpectedTenant);
        Assert.Equal("globex", failure.ActualTenant);
        Assert.Equal(before, table.AllRows.Count);
    }

    [Fact]
    public void Two_Tenants_Over_One_Table_See_Disjoint_Sets()
    {
        var (_, acme, globex) = Build();

        acme.Add(new Document("doc-4", "", "Acme extra"));
        globex.Add(new Document("doc-5", "", "Globex extra"));

        Assert.Empty(acme.List().Select(d => d.Id).Intersect(globex.List().Select(d => d.Id)));
        Assert.All(acme.List(), d => Assert.Equal("acme", d.TenantId));
        Assert.All(globex.List(), d => Assert.Equal("globex", d.TenantId));
    }
}
