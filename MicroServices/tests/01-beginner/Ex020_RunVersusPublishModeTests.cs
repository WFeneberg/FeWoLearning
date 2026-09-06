using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex020_RunVersusPublishModeTests
{
    private const string ManagedConnectionString =
        "Host=orders.postgres.database.contoso.com;Port=5432;Database=orders;Username=app";

    [Fact]
    public void Run_mode_gets_a_container_a_developer_can_throw_away()
    {
        // ModelHarness.Build is the RUN-mode builder - Args is empty, so
        // ExecutionContext.IsRunMode is true.
        var model = ModelHarness.Build(Ex020_RunVersusPublishMode.Configure);

        Assert.IsType<PostgresServerResource>(model.Resource("pg"));

        // The type AND the expression, per README section 9 rule 1: "a Postgres-ish
        // resource named orders" would be satisfied by AddConnectionString on this
        // side too, and that is precisely the wrong answer for run mode.
        Assert.IsType<PostgresDatabaseResource>(model.Resource("orders"));
        Assert.Equal("{pg.connectionString};Database=orders",
                     ModelHarness.ConnectionString(model.Resource("orders")));

        Assert.True(model.Has("api"));
    }

    [Fact]
    public void Publish_mode_gets_the_managed_database_and_no_server_of_its_own()
    {
        // The same Configure, the same process, a different execution context.
        var model = ModelHarness.BuildForPublish(Ex020_RunVersusPublishMode.Configure);

        // The absence is the point: publishing a container image for a database that
        // is already operated by someone else is the failure this row is about, and
        // an implementation that branches on nothing lands "pg" right here.
        Assert.False(model.Has("pg"));

        var orders = Assert.IsType<ConnectionStringResource>(model.Resource("orders"));
        Assert.Equal(ManagedConnectionString, orders.ConnectionStringExpression.ValueExpression);

        // The consumer is untouched by the branch - that is what makes the pattern
        // worth teaching rather than just two AppHosts.
        Assert.True(model.Has("api"));
    }

    [Fact]
    public void The_two_graphs_differ_where_the_row_says_they_differ()
    {
        var run = ModelHarness.Build(Ex020_RunVersusPublishMode.Configure);
        var publish = ModelHarness.BuildForPublish(Ex020_RunVersusPublishMode.Configure);

        // Stated as a direct comparison, and deliberately not as "something differs":
        // a model that differed only in some incidental field would satisfy a
        // diff-everything assertion while branching on nothing that matters. These
        // are the two named differences.
        Assert.True(run.Has("pg"));
        Assert.False(publish.Has("pg"));
        Assert.NotEqual(run.Resource("orders").GetType(), publish.Resource("orders").GetType());

        // And the part that must NOT differ, or the branch has forked the whole
        // AppHost instead of the one decision it was supposed to fork.
        Assert.True(run.Has("api"));
        Assert.True(publish.Has("api"));
    }

    [Fact]
    public async Task The_published_manifest_describes_the_managed_database_not_a_container()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex020_RunVersusPublishMode.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");

        // The artifact-level restatement, which is what a deployment actually reads.
        // value.v0 is a connection string and nothing else; a Postgres container
        // would have arrived as container.v0 with an image and bindings.
        var orders = resources.GetProperty("orders");
        Assert.Equal("value.v0", orders.GetProperty("type").GetString());
        Assert.Equal(ManagedConnectionString, orders.GetProperty("connectionString").GetString());

        Assert.Equal("container.v0", resources.GetProperty("api").GetProperty("type").GetString());
        Assert.False(resources.TryGetProperty("pg", out _));
    }
}
