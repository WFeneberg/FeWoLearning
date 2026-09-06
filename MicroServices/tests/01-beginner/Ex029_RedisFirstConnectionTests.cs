using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex029_RedisFirstConnectionTests
{
    /// <summary>
    /// Every resource in the model whose Parent is this one. Used for BOTH halves of
    /// the row's absence claim, on purpose: a helper that always answered "none"
    /// would satisfy the Redis assertion and fail the Postgres one.
    /// </summary>
    private static IReadOnlyList<IResource> ChildrenOf(ModelHarness.Result model, IResource parent)
        => model.Resources.OfType<IResourceWithParent>()
                .Where(r => ReferenceEquals(r.Parent, parent))
                .Cast<IResource>()
                .ToList();

    [Fact]
    public void Redis_is_addressed_by_host_and_port_with_no_scheme_and_no_keyed_clauses()
    {
        var model = ModelHarness.Build(Ex029_RedisFirstConnection.Configure);

        // The type, per the track's first rule: AddContainer("cache", "redis") is a
        // "Redis-ish container" and would satisfy any name-or-image assertion.
        var cache = Assert.IsType<RedisResource>(model.Resource("cache"));
        var expression = ModelHarness.ConnectionString(cache);

        // Host and port, adjacent, colon-separated, at the very start - that IS the
        // address. Asserted as a prefix rather than as the whole string: measured on
        // 13.5.3, the tail is ",password={cache-password.value}" followed by a
        // conditional TLS fragment whose resource name carries a content hash
        // ("{cond-cache-bindings-tcp-tlsenabled-<hash>.connectionString}"). Pinning
        // that hash would make this fact a version tripwire it is not meant to be;
        // the password clause below is pinned because it is not hashed.
        Assert.StartsWith("{cache.bindings.tcp.host}:{cache.bindings.tcp.port}", expression);

        // Comma-separated StackExchange.Redis option syntax, not the
        // semicolon-separated ADO.NET syntax the other three stores in this tier use.
        Assert.Contains(",password={cache-password.value}", expression);
        Assert.DoesNotContain(";", expression);

        // "No scheme" is the row's phrase and it is asserted literally. A learner who
        // reached for a redis:// URL - the form every Redis client documents - is
        // wrong about what Aspire hands over.
        Assert.DoesNotContain("://", expression);

        // And none of the keyed spellings the other flavours use.
        Assert.DoesNotContain("Host=", expression);
        Assert.DoesNotContain("Server=", expression);
        Assert.DoesNotContain("Database=", expression);
        Assert.DoesNotContain("Initial Catalog=", expression);
    }

    [Fact]
    public void The_cache_has_no_database_child_while_the_relational_store_does()
    {
        var model = ModelHarness.Build(Ex029_RedisFirstConnection.Configure);

        // Typed here as well as in fact 1, so this fact stands on its own: a bare
        // AddContainer("cache", "redis") also has no children, and would otherwise
        // pass everything below.
        var cache = Assert.IsType<RedisResource>(model.Resource("cache"));
        var pg = Assert.IsType<PostgresServerResource>(model.Resource("pg"));

        // The POSITIVE half first. This is what stops the negative half from being
        // vacuous: the same ChildrenOf helper, over the same model, finds exactly one
        // child for Postgres and names it.
        var pgChildren = ChildrenOf(model, pg);
        var sessions = Assert.IsType<PostgresDatabaseResource>(Assert.Single(pgChildren));
        Assert.Equal("sessions", sessions.Name);

        // The negative half - the row's actual subject. There is no AddDatabase on a
        // RedisResource to call, so a Redis logical database is an integer selected
        // on the connection, never a resource in the graph.
        Assert.Empty(ChildrenOf(model, cache));

        // Redis is not a child of anything either - it is a root resource, not one
        // half of a server/database pair.
        Assert.IsNotAssignableFrom<IResourceWithParent>(cache);

        // Exactly three resources, so nothing has quietly appeared beside the cache:
        // a model with a hand-rolled fourth "cache-db" would pass both assertions
        // above (it would not be IResourceWithParent) and fails here.
        Assert.Equal(
            new[] { "cache", "pg", "sessions" },
            model.Resources.Select(r => r.Name).Order().ToArray());
    }

    [Fact]
    public void The_scheme_exists_on_the_binding_even_though_the_connection_string_has_none()
    {
        var model = ModelHarness.Build(Ex029_RedisFirstConnection.Configure);

        // Measured on 13.5.3, and the reason fact 1's "no scheme" is a statement
        // about the STRING and not about the model: Redis's endpoint declares
        // UriScheme "redis" - the dashboard and the proxy know about it - while the
        // connection string a client is handed omits it entirely.
        // Typed here too - a hand-rolled container given WithEndpoint(scheme: "redis",
        // targetPort: 6379) produces an EndpointAnnotation identical in every field
        // asserted below, so without this line the fact would grade nothing.
        var cache = Assert.IsType<RedisResource>(model.Resource("cache"));
        var endpoint = Assert.Single(cache.Annotations.OfType<EndpointAnnotation>());
        Assert.Equal("redis", endpoint.UriScheme);
        Assert.Equal(6379, endpoint.TargetPort);

        // The contrast, in the same model: Postgres's endpoint is a plain tcp one,
        // so "the binding carries a scheme" is not something every resource does and
        // the assertion above grades a real difference.
        var pgEndpoint = Assert.Single(model.Resource("pg").Annotations.OfType<EndpointAnnotation>());
        Assert.Equal("tcp", pgEndpoint.UriScheme);
        Assert.Equal(5432, pgEndpoint.TargetPort);
    }
}
