using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.MongoDB;
using Aspire.Hosting.Postgres;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex031_DataVolumesPerFlavourTests
{
    private static IReadOnlyList<ContainerMountAnnotation> Mounts(ModelHarness.Result model, string name)
        => model.Resource(name).Annotations.OfType<ContainerMountAnnotation>().ToList();

    private static ContainerMountAnnotation SingleVolume(ModelHarness.Result model, string name)
    {
        var mounts = Mounts(model, name);
        Assert.True(mounts.Count == 1,
            $"'{name}' should carry exactly one data mount; it carries {mounts.Count}: "
            + string.Join(", ", mounts.Select(m => $"{m.Type} {m.Source}->{m.Target}")));
        return Assert.Single(mounts, m => m.Type == ContainerMountType.Volume);
    }

    [Fact]
    public void Each_flavour_puts_its_data_somewhere_else_and_the_helper_knows_where()
    {
        var model = ModelHarness.Build(Ex031_DataVolumesPerFlavour.Configure);

        // The types first, per the track's persistence rule: AddContainer("pg", "postgres")
        // plus a hand-written WithVolume would satisfy every path assertion below and
        // grade nothing. Only the integration resource proves the flavour-aware helper
        // was even available to be called.
        Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.IsType<MongoDBServerResource>(model.Resource("docs"));
        Assert.IsType<SqlServerServerResource>(model.Resource("sql"));

        var pg = SingleVolume(model, "pg");
        var docs = SingleVolume(model, "docs");
        var sql = SingleVolume(model, "sql");

        // Source is the volume NAME the learner chose - the one thing about a data
        // volume that IS theirs to decide. The anonymous WithDataVolume() overload
        // leaves an auto-generated name derived from the assembly, which differs
        // between the red run and the green run; naming it is what makes it findable.
        Assert.Equal("pgdata", pg.Source);
        Assert.Equal("mongodata", docs.Source);
        Assert.Equal("sqldata", sql.Source);

        // The targets, which are NOT the learner's to decide. Measured on 13.5.3 with
        // the default image tags (postgres 18.3, mongo 8.3, mssql/server 2022-latest).
        // Note the Postgres one: /var/lib/postgresql, NOT the /var/lib/postgresql/data
        // that every tutorial and the catalog row itself quote - that path belongs to
        // Postgres 17 and earlier, and fact 2 pins both.
        Assert.Equal("/var/lib/postgresql", pg.Target);
        Assert.Equal("/data/db", docs.Target);
        Assert.Equal("/var/opt/mssql", sql.Target);

        // ...and the claim the row actually makes: no shared constant could have
        // produced all three. Stated as an assertion rather than left implicit, so a
        // future version that unified two of them fails HERE with a readable message
        // rather than three lines up with a string diff.
        var targets = new[] { pg.Target, docs.Target, sql.Target };
        Assert.Equal(3, targets.Distinct().Count());

        // A database that cannot write its own data directory is the read-only flag on
        // the wrong mount; WithDataVolume defaults to writable and should stay that way.
        Assert.All(new[] { pg, docs, sql }, m => Assert.False(m.IsReadOnly));
    }

    [Fact]
    public void The_ANONYMOUS_overload_names_the_volume_itself_which_no_typed_name_can()
    {
        var model = ModelHarness.Build(Ex031_DataVolumesPerFlavour.Configure);
        Assert.IsType<PostgresServerResource>(model.Resource("pgauto"));

        var auto = SingleVolume(model, "pgauto");
        Assert.Equal("/var/lib/postgresql", auto.Target);

        // This fact exists because of a measured hole in fact 1. Measured on 13.5.3:
        //     builder.AddPostgres("pg").WithVolume("pgdata", "/var/lib/postgresql")
        // writes a ContainerMountAnnotation BYTE-IDENTICAL to
        //     builder.AddPostgres("pg").WithDataVolume("pgdata")
        // - same Type, Source, Target, IsReadOnly - so no assertion about a NAMED data
        // volume can tell the flavour-aware helper from a path looked up once and typed
        // in. The whole mutant below passes facts 1-3 without calling a single helper:
        //
        //     builder.AddPostgres("pg").WithVolume("pgdata", "/var/lib/postgresql");
        //     builder.AddPostgres("pg17").WithImageTag("17.5")
        //            .WithVolume("pg17data", "/var/lib/postgresql/data");
        //     builder.AddMongoDB("docs").WithVolume("mongodata", "/data/db");
        //     builder.AddSqlServer("sql").WithVolume("sqldata", "/var/opt/mssql");
        //     builder.AddSqlServer("sqlbind")
        //            .WithBindMount("./sqlserver-data/data", "/var/opt/mssql/data")
        //            .WithBindMount("./sqlserver-data/log", "/var/opt/mssql/log")
        //            .WithBindMount("./sqlserver-data/secrets", "/var/opt/mssql/secrets");
        //
        // The anonymous overload is the one thing it cannot reproduce. Aspire derives
        // the name from the AppHost, and the hash in the middle differs between the red
        // run (fewolearning.microservices.tests-549a2a9f7b-pgauto-data) and the green
        // one (…-9a0e1cce07-…) - so it is not a literal anyone can commit.
        Assert.NotNull(auto.Source);
        Assert.EndsWith("-pgauto-data", auto.Source);
        Assert.Matches(@"^.+-[0-9a-f]{8,}-pgauto-data$", auto.Source);

        // The generated shape is undocumented framework text, so treat a failure on the
        // pattern as a version tripwire rather than a broken answer - the same stance
        // ex004 takes on health-check keys and ex030 on relationship types. What the row
        // actually claims is the line below: nothing about the resource, and nothing the
        // learner wrote, is enough to predict this string.
        Assert.NotEqual("pgauto-data", auto.Source);
        Assert.NotEqual("pgauto", auto.Source);

        // ...and the named server beside it still has the name it was given, so the two
        // overloads are graded against each other rather than one being asserted alone.
        Assert.Equal("pgdata", SingleVolume(model, "pg").Source);
    }

    [Fact]
    public void The_postgres_path_follows_the_IMAGE_TAG_so_it_is_not_a_constant_at_all()
    {
        var model = ModelHarness.Build(Ex031_DataVolumesPerFlavour.Configure);

        // Two Postgres servers, the same WithDataVolume call, two different answers.
        // This is the assertion that makes "one shared constant is wrong" measurable
        // instead of a slogan - and it is the one a learner who typed
        // /var/lib/postgresql/data by hand cannot pass twice.
        Assert.IsType<PostgresServerResource>(model.Resource("pg17"));

        var tag = Assert.Single(model.Resource("pg17").Annotations.OfType<ContainerImageAnnotation>());
        Assert.Equal("17.5", tag.Tag);

        var pg17 = SingleVolume(model, "pg17");
        Assert.Equal("pg17data", pg17.Source);
        Assert.Equal("/var/lib/postgresql/data", pg17.Target);

        // ...against the default-tag server from fact 1.
        Assert.Equal("/var/lib/postgresql", SingleVolume(model, "pg").Target);
        Assert.NotEqual(SingleVolume(model, "pg").Target, pg17.Target);

        // The measured mutant this rejects, and it is a sharp one because it compiles,
        // reads correctly, and is silently wrong:
        //     builder.AddPostgres("pg17").WithDataVolume("pg17data").WithImageTag("17.5")
        // WithDataVolume reads the tag CONFIGURED AT THE MOMENT IT RUNS, so with the
        // calls in that order the mount is written for the default tag 18 and lands on
        // /var/lib/postgresql while the image annotation still says 17.5. The
        // container then starts an initdb into an empty directory beside the real one
        // on every run. Nothing warns. Only this assertion notices.
    }

    [Fact]
    public void A_data_bind_mount_is_a_different_annotation_and_SQL_Server_needs_three()
    {
        var model = ModelHarness.Build(Ex031_DataVolumesPerFlavour.Configure);
        Assert.IsType<SqlServerServerResource>(model.Resource("sqlbind"));

        var mounts = Mounts(model, "sqlbind");

        // THREE, not one - measured on 13.5.3, and the asymmetry is real: the volume
        // helper covers /var/opt/mssql in one go, the bind-mount helper splits the same
        // host folder into data/log/secrets so that mounting it cannot hide the server
        // binaries that live in the same directory.
        // The mutant this rejects is the obvious hand-rolled equivalent,
        //     .WithBindMount("./sqlserver-data", "/var/opt/mssql")
        // which produces exactly one mount, a container that will not start, and a
        // model that looks right in the dashboard.
        Assert.Equal(3, mounts.Count);
        Assert.All(mounts, m => Assert.Equal(ContainerMountType.BindMount, m.Type));

        Assert.Equal(
            new[] { "/var/opt/mssql/data", "/var/opt/mssql/log", "/var/opt/mssql/secrets" },
            mounts.Select(m => m.Target).Order().ToArray());

        // A bind mount's Source is resolved to an absolute HOST path against the
        // AppHost directory - which under these harnesses is the test assembly's own
        // output folder, so it differs between the red and the green run. Assert that
        // it is rooted, and assert the two stable segments; never the whole path.
        // (Being rooted is also the second, independent way of separating a bind mount
        // from a volume, whose Source stays the literal name it was given - ex009.)
        foreach (var mount in mounts)
        {
            Assert.NotNull(mount.Source);
            Assert.True(Path.IsPathRooted(mount.Source),
                $"A bind mount's source is an absolute host path; got '{mount.Source}'.");
        }
        Assert.Equal(
            new[] { "data", "log", "secrets" },
            mounts.Select(m => Path.GetFileName(m.Source!.TrimEnd('/', '\\'))).Order().ToArray());
        Assert.All(mounts, m => Assert.Equal(
            "sqlserver-data",
            Path.GetFileName(Path.GetDirectoryName(m.Source!.TrimEnd('/', '\\')))));

        // ...and the same flavour with the OTHER helper still has exactly one mount,
        // so the asymmetry is graded in both directions rather than asserted once.
        Assert.Single(Mounts(model, "sql"));
    }
}
