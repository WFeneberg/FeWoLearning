namespace FeWoLearning.Architecture.Exercises.Scale.Ex072;

public sealed record Document(string Id, string TenantId, string Title);

public sealed class CrossTenantWriteException(string expected, string actual)
    : Exception($"Refusing to write a document for tenant '{actual}' into tenant '{expected}'.")
{
    public string ExpectedTenant { get; } = expected;
    public string ActualTenant { get; } = actual;
}

/// <summary>The shared table every tenant's rows live in.</summary>
public sealed class DocumentTable
{
    private readonly List<Document> _rows = [];

    public IReadOnlyList<Document> AllRows => _rows;

    public void Seed(Document document) => _rows.Add(document);

    public void Insert(Document document) => _rows.Add(document);
}

// Exercise 072 — MultiTenancyIsolation (scale).
// Goal:   Share one table between tenants without ever letting one of them see or touch
//         another's rows.
// Drills: tenant-scoped access, the missing filter, refusing rather than correcting.
// Passes: List      - returns only this tenant's documents.
//         THE ONE    - Find with an id belonging to ANOTHER tenant returns NULL. Not the
//                     document, and not an exception that names it: to this tenant, that
//                     row does not exist.
//         Add       - stamps the current tenant onto the row.
//         refusal   - Add with a document already carrying a DIFFERENT tenant id is
//                     REFUSED, not silently re-stamped.
//         isolation - two repositories over the same table see disjoint sets.
//
// Find is where multi-tenant systems leak, and they leak the same way every time: the id
// is a primary key, the lookup is by primary key, and the tenant filter is the one
// clause nobody adds because the row is already unique without it. The result is an
// endpoint that returns any tenant's document to any tenant who can guess an id - and it
// passes every functional test, because every functional test uses one tenant.
//
// The refusal is the other half, and it is a judgement about whose bug it is. Silently
// re-stamping a document that arrived with somebody else's tenant id makes the write
// succeed and hides that the caller mixed two tenants' data in one request. The next
// such bug will not be a write.
public sealed class TenantScopedRepository(string tenantId, DocumentTable table)
{
    public string TenantId => tenantId;

    public IReadOnlyList<Document> List() =>
        throw new NotImplementedException("TODO: Ex072 - only this tenant's rows");

    public Document? Find(string id) =>
        throw new NotImplementedException(
            "TODO: Ex072 - find by id AND tenant; another tenant's row does not exist as far as this repository is concerned");

    public void Add(Document document) =>
        throw new NotImplementedException(
            "TODO: Ex072 - stamp this tenant onto a document that has none, and refuse one that names a different tenant");
}
