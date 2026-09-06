using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.MongoDB;
using Aspire.Hosting.Postgres;
using Aspire.Hosting.Redis;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex030_DatabaseAdminToolsTests
{
    [Fact]
    public void Each_helper_adds_a_SEPARATE_resource_of_its_own_type()
    {
        var model = ModelHarness.Build(Ex030_DatabaseAdminTools.Configure);

        // The stores are untouched by the helpers - each WithXxx returns the store's
        // own builder, so this is the "did you configure the store or add something
        // beside it" half.
        Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.IsType<MongoDBServerResource>(model.Resource("docs"));
        Assert.IsType<RedisResource>(model.Resource("cache"));

        // Three more resources, of three integration-specific types. The TYPE is what
        // rejects the plausible wrong answer - AddContainer("pgadmin",
        // "dpage/pgadmin4") next to the store, which looks the same in a dashboard
        // and is not what the row asked for.
        Assert.IsType<PgAdminContainerResource>(model.Resource("pgadmin"));
        Assert.IsType<MongoExpressContainerResource>(model.Resource("docs-mongoexpress"));
        Assert.IsType<RedisInsightResource>(model.Resource("redisinsight"));

        // The names are asserted through Resource(...) above rather than separately,
        // and they are worth reading: measured on 13.5.3, pgAdmin and RedisInsight are
        // SINGLETONS with fixed names, while Mongo Express is per-server and derives
        // its name from its parent. Three helpers, three naming rules.
        Assert.Equal(
            new[] { "cache", "docs", "docs-mongoexpress", "pg", "pgadmin", "redisinsight" },
            model.Resources.Select(r => r.Name).Order().ToArray());
    }

    [Fact]
    public void The_link_to_the_store_is_a_relationship_annotation_not_parenting()
    {
        var model = ModelHarness.Build(Ex030_DatabaseAdminTools.Configure);

        // Typed here as well as in fact 1, so this fact stands on its own: measured,
        // a hand-rolled AddContainer("pgadmin", "dpage/pgadmin4") carrying a
        // ResourceRelationshipAnnotation the learner constructed by hand satisfies
        // every assertion below, and only the console TYPE separates it from the real
        // helper. The stores are typed for the same reason.
        var pg = Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        var docs = Assert.IsType<MongoDBServerResource>(model.Resource("docs"));
        var cache = Assert.IsType<RedisResource>(model.Resource("cache"));
        Assert.IsType<PgAdminContainerResource>(model.Resource("pgadmin"));
        Assert.IsType<MongoExpressContainerResource>(model.Resource("docs-mongoexpress"));
        Assert.IsType<RedisInsightResource>(model.Resource("redisinsight"));

        // One relationship each, pointing at the VERY store object the helper was
        // called on - Assert.Same, so a model with two Postgres servers and the
        // console attached to the wrong one fails.
        var pgAdminLink = Assert.Single(
            model.Resource("pgadmin").Annotations.OfType<ResourceRelationshipAnnotation>());
        Assert.Same(pg, pgAdminLink.Resource);

        var mongoExpressLink = Assert.Single(
            model.Resource("docs-mongoexpress").Annotations.OfType<ResourceRelationshipAnnotation>());
        Assert.Same(docs, mongoExpressLink.Resource);

        var insightLink = Assert.Single(
            model.Resource("redisinsight").Annotations.OfType<ResourceRelationshipAnnotation>());
        Assert.Same(cache, insightLink.Resource);

        // The Type strings, which are NOT the same word three times. Measured on
        // 13.5.3 and undocumented, so treat a failure here as a version tripwire
        // rather than a broken answer - it is the only assertion in this row that
        // pins framework text.
        Assert.Equal("PgAdmin", pgAdminLink.Type);
        Assert.Equal("Parent", mongoExpressLink.Type);
        Assert.Equal("RedisInsight", insightLink.Type);

        // "Tied to its parent" is a RELATIONSHIP here, not the IResourceWithParent
        // link a database child has (ex001, ex014). None of the three consoles is a
        // child of anything, and the direction is one-way: the stores carry no
        // relationship annotation pointing back.
        foreach (var console in new[] { "pgadmin", "docs-mongoexpress", "redisinsight" })
        {
            Assert.IsNotAssignableFrom<IResourceWithParent>(model.Resource(console));
        }
        foreach (IResource store in new IResource[] { pg, docs, cache })
        {
            Assert.Empty(store.Annotations.OfType<ResourceRelationshipAnnotation>());
        }
    }

    [Fact]
    public async Task The_consoles_are_local_only_and_never_reach_the_manifest()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex030_DatabaseAdminTools.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");

        // The positive half, and it is load-bearing: an empty or failed manifest
        // would satisfy the exclusions below just as well.
        foreach (var store in new[] { "pg", "docs", "cache" })
        {
            Assert.True(resources.TryGetProperty(store, out var published), $"'{store}' is missing from the manifest.");
            Assert.Equal("container.v0", published.GetProperty("type").GetString());
        }

        // ...and the exclusions. Each helper calls ExcludeFromManifest for you -
        // measured on 13.5.3 - which is why an unauthenticated admin console cannot
        // accidentally be deployed next to production data. Measured too, so that the
        // reach of this fact is not overstated: a hand-rolled
        // AddContainer("pgadmin", ...) that ALSO calls ExcludeFromManifest passes
        // here, and is rejected only by the type assertion in fact 1. What this fact
        // rejects on its own is the hand-rolled console that did not - it publishes
        // as an ordinary container.v0, which is ex019's whole subject.
        foreach (var console in new[] { "pgadmin", "docs-mongoexpress", "redisinsight" })
        {
            Assert.False(resources.TryGetProperty(console, out _), $"'{console}' must not be published.");
        }

        // Deliberately not asserted anywhere in this row: a port. The catalog row says
        // so - a port is the least stable thing about these helpers and grades nothing
        // about the mechanism.
    }
}
