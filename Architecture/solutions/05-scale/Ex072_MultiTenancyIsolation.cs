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

// Exercise 072 — MultiTenancyIsolation (reference solution).
public sealed class TenantScopedRepository(string tenantId, DocumentTable table)
{
    public string TenantId => tenantId;

    public IReadOnlyList<Document> List() =>
        [.. table.AllRows.Where(d => d.TenantId == tenantId)];

    public Document? Find(string id) =>
        // The tenant clause is in the lookup, not applied after it. It is the clause
        // nobody adds - the id is already unique - and its absence is an endpoint that
        // returns any tenant's document to anybody who can guess an id, passing every
        // functional test, because every functional test uses one tenant.
        table.AllRows.FirstOrDefault(d => d.Id == id && d.TenantId == tenantId);

    public void Add(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        // Refused, not corrected. Silently re-stamping a document that arrived carrying
        // somebody else's tenant makes the write succeed and hides that the caller mixed
        // two tenants' data in one request - and the next bug of that shape will not be a
        // write.
        if (!string.IsNullOrEmpty(document.TenantId) && document.TenantId != tenantId)
            throw new CrossTenantWriteException(tenantId, document.TenantId);

        table.Insert(document with { TenantId = tenantId });
    }
}
