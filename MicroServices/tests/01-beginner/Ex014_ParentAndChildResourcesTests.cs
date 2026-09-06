using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex014_ParentAndChildResourcesTests
{
    private static IResource ParentOf(IResource resource)
        => Assert.IsAssignableFrom<IResourceWithParent>(resource).Parent;

    [Fact]
    public void Every_database_names_the_server_it_actually_hangs_off()
    {
        var model = ModelHarness.Build(Ex014_ParentAndChildResources.Configure);

        var pg = Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        var sql = Assert.IsType<SqlServerServerResource>(model.Resource("sql"));
        var mongo = Assert.IsType<MongoDBServerResource>(model.Resource("mongo"));

        // Assert.Same, not Assert.Equal on a name: reference equality is what rejects
        // the learner who added a second Postgres server and hung "shipping" off the
        // wrong one, and it is what rejects the mutant this row exists to catch -
        //
        //     builder.AddConnectionString("billing", ReferenceExpression.Create(
        //         $"{pg};Database=billing"));
        //
        // Measured: that renders the byte-identical expression
        // "{pg.connectionString};Database=billing" that the next fact asserts, so a
        // test grading only the rendered string would pass it. It is a
        // ConnectionStringResource, which is not IResourceWithParent at all, so
        // IsAssignableFrom throws here instead.
        Assert.Same(pg, ParentOf(model.Resource("billing")));
        Assert.Same(pg, ParentOf(model.Resource("shipping")));
        Assert.Same(sql, ParentOf(model.Resource("inventory")));
        Assert.Same(mongo, ParentOf(model.Resource("reviews")));

        // And the children are real database resources of their flavour, not generic
        // ones - the type is what proves AddDatabase was called on the right builder.
        Assert.IsType<PostgresDatabaseResource>(model.Resource("billing"));
        Assert.IsType<PostgresDatabaseResource>(model.Resource("shipping"));
        Assert.IsType<SqlServerDatabaseResource>(model.Resource("inventory"));
        Assert.IsType<MongoDBDatabaseResource>(model.Resource("reviews"));
    }

    [Fact]
    public void A_composing_child_defers_to_its_parent_instead_of_repeating_it()
    {
        var model = ModelHarness.Build(Ex014_ParentAndChildResources.Configure);

        // Both halves of the row's "rather than repeating the host and port" are
        // graded, because only the pair says anything. The PARENT is where the host
        // and port live...
        Assert.Contains("{pg.bindings.tcp.host}", ModelHarness.ConnectionString(model.Resource("pg")));
        Assert.Contains("{sql.bindings.tcp.host}", ModelHarness.ConnectionString(model.Resource("sql")));

        // ...and the CHILD names none of it. It interpolates the parent's whole
        // connection string and appends its own clause, so when the server moves - a
        // different port, a rotated password - nothing about the child changes.
        Assert.Equal("{pg.connectionString};Database=billing",
            ModelHarness.ConnectionString(model.Resource("billing")));
        Assert.Equal("{pg.connectionString};Database=shipping",
            ModelHarness.ConnectionString(model.Resource("shipping")));
        Assert.Equal("{sql.connectionString};Initial Catalog=inventory",
            ModelHarness.ConnectionString(model.Resource("inventory")));

        // Stated as its own assertion rather than left implied by the equality above,
        // because it is the claim the row makes: a child that had copied its parent's
        // host and port would carry a "bindings" fragment here.
        foreach (var child in new[] { "billing", "shipping", "inventory" })
        {
            Assert.DoesNotContain("bindings", ModelHarness.ConnectionString(model.Resource(child)));
        }
    }

    [Fact]
    public void Mongo_is_the_counterexample_parented_and_yet_not_composed()
    {
        var model = ModelHarness.Build(Ex014_ParentAndChildResources.Configure);

        var reviews = ModelHarness.ConnectionString(model.Resource("reviews"));

        // The whole reason this row does not stop at Postgres. "reviews" has the same
        // parent link as "billing" - fact 1 asserted it against the mongo object - and
        // yet its expression does NOT interpolate {mongo.connectionString}; it repeats
        // the host and the port. Measured on 13.5.3, and it is structural rather than
        // an oversight: Postgres appends ";Database=" and SQL Server appends
        // ";Initial Catalog=" to the END of the parent's string, whereas Mongo's
        // database name is a path segment in the MIDDLE of a URI, so there is no tail
        // to append to.
        //
        // Parenting and interpolation are therefore two separate facts about a child,
        // which is what a learner who saw only fact 2 would get wrong.
        Assert.DoesNotContain("{mongo.connectionString}", reviews);
        Assert.Contains("{mongo.bindings.tcp.host}", reviews);
        Assert.Contains("/reviews?", reviews);

        // Asserted as substrings, not as one pinned literal: the surrounding URI - the
        // admin user, the authSource and authMechanism query - is Aspire's text, not
        // the learner's, and pinning it whole would turn this fact into a version
        // tripwire it was never meant to be. Row 028 owns the full Mongo URI.
    }
}
