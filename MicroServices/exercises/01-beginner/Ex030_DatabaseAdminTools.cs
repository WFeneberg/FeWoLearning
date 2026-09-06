using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Attach the three web consoles - pgAdmin, Mongo Express, RedisInsight - to
///         the three stores, and see what each helper actually put in the graph.
/// Drills: `WithPgAdmin`, `WithMongoExpress`, `WithRedisInsight`. None of them
///         configures the store: each ADDS A SEPARATE CONTAINER RESOURCE and links
///         it to the store with a ResourceRelationshipAnnotation.
/// Passes: The model holds six resources - the three stores plus "pgadmin"
///         (PgAdminContainerResource), "docs-mongoexpress"
///         (MongoExpressContainerResource) and "redisinsight" (RedisInsightResource);
///         each console carries exactly one ResourceRelationshipAnnotation pointing
///         at the very store object it was called on; and none of the three consoles
///         appears in aspire-manifest.json while all three stores do.
/// Note:   No port is asserted anywhere, deliberately - a port is the least stable
///         and least interesting thing about these helpers.
///         Three measured asymmetries (13.5.3), each of which would be easy to
///         assume away:
///           * the console's NAME is derived differently per integration. pgAdmin
///             and RedisInsight are singletons with fixed names ("pgadmin",
///             "redisinsight"); Mongo Express is per-server and is named after its
///             parent ("docs-mongoexpress").
///           * the relationship's Type string differs too: "PgAdmin", "Parent" and
///             "RedisInsight" respectively. It is undocumented text and a version
///             bump may change it, so treat a failure there as a tripwire.
///           * the link is a RELATIONSHIP, not parenting. None of the three consoles
///             implements IResourceWithParent, so nothing about them behaves like
///             the database children of ex001 and ex014.
///         And they are local-only: each helper excludes its console from the
///         manifest for you, which is what the last fact checks - a hand-rolled
///         AddContainer("pgadmin", "dpage/pgadmin4") would be published.
/// </summary>
public static class Ex030_DatabaseAdminTools
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex030 - add a Postgres server \"pg\" with pgAdmin, a MongoDB "
            + "server \"docs\" with Mongo Express, and a Redis resource \"cache\" "
            + "with RedisInsight.");
}
