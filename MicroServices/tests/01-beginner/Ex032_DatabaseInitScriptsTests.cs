using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Postgres;
using FeWoLearning.MicroServices.Exercises.Beginner;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex032_DatabaseInitScriptsTests
{
    private const string InitDirectory = "/docker-entrypoint-initdb.d";
    private const string ScriptFileName = "001-create-schema.sql";

    [Fact]
    public async Task WithInitFiles_COPIES_the_scripts_and_leaves_no_mount_behind()
    {
        var model = ModelHarness.Build(Ex032_DatabaseInitScripts.Configure);
        var pg = Assert.IsType<PostgresServerResource>(model.Resource("pg"));

        // The annotation is a file-system CALLBACK, not a mount. Measured on 13.5.3:
        // WithInitFiles writes ContainerFileSystemCallbackAnnotation and WithBindMount /
        // WithInitBindMount write ContainerMountAnnotation, so the two mechanisms are
        // distinguishable at model time and the row does not have to guess.
        var files = Assert.Single(pg.Annotations.OfType<ContainerFileSystemCallbackAnnotation>());
        Assert.Equal(InitDirectory, files.DestinationPath);

        // The mutant this rejects, and the reason the negative half is here: the
        // generic
        //     .WithBindMount(Ex032_DatabaseInitScripts.InitFolder, "/docker-entrypoint-initdb.d")
        // reaches the same directory in the same container and satisfies every
        // assertion about the destination path - it simply is not the mechanism the row
        // is about. A bare AddPostgres carries no mount at all (ex009), so "zero
        // mounts" is a real statement about the answer and not something free.
        Assert.Empty(pg.Annotations.OfType<ContainerMountAnnotation>());

        // ...and the files themselves. The callback enumerates the source folder when
        // it runs, so invoking it is how a test learns whether the folder that was
        // pointed at actually contains anything. An implementation aimed at an empty
        // directory publishes a callback that yields nothing, and only this half
        // notices.
        var context = new ContainerFileSystemCallbackContext
        {
            Model = pg,
            Services = new ServiceCollection().BuildServiceProvider()
        };
        var entries = await files.Callback(context, TestContext.Current.CancellationToken);

        var script = Assert.Single(entries.OfType<ContainerFile>(), f => f.Name == ScriptFileName);

        // Measured: the copy carries SourcePath and leaves Contents NULL - the bytes are
        // streamed into the container at run time rather than captured into the model -
        // so a test must assert on the path, not on the SQL.
        Assert.Null(script.Contents);
        Assert.NotNull(script.SourcePath);
        Assert.True(Path.IsPathRooted(script.SourcePath),
            $"The copied file's source is an absolute host path; got '{script.SourcePath}'.");
        Assert.Equal("initdb", Path.GetFileName(Path.GetDirectoryName(script.SourcePath)));
        Assert.True(File.Exists(script.SourcePath), $"'{script.SourcePath}' is not on disk.");
    }

    [Fact]
    public void The_obsolete_spelling_is_a_READ_ONLY_bind_mount_to_the_same_directory()
    {
        var model = ModelHarness.Build(Ex032_DatabaseInitScripts.Configure);
        var legacy = Assert.IsType<PostgresServerResource>(model.Resource("legacy"));

        // The mirror image of fact 1: a mount and no callback, where "pg" had a callback
        // and no mount. Graded in both directions with the same two annotation types, so
        // an answer that used one spelling twice fails on whichever server it got wrong.
        Assert.Empty(legacy.Annotations.OfType<ContainerFileSystemCallbackAnnotation>());
        var mount = Assert.Single(legacy.Annotations.OfType<ContainerMountAnnotation>());

        Assert.Equal(ContainerMountType.BindMount, mount.Type);
        Assert.Equal(InitDirectory, mount.Target);

        // isReadOnly defaults to TRUE here, where the generic WithBindMount defaults to
        // false (measured, 13.5.3). So a learner reaching for WithBindMount and copying
        // the target across gets a WRITABLE mount of their own source tree into a
        // container that runs everything in it as the superuser - which is exactly the
        // difference this assertion is worth.
        Assert.True(mount.IsReadOnly);

        Assert.NotNull(mount.Source);
        Assert.True(Path.IsPathRooted(mount.Source),
            $"A bind mount's source is an absolute host path; got '{mount.Source}'.");
        Assert.Equal("initdb", Path.GetFileName(mount.Source.TrimEnd('/', '\\')));
    }

    [Fact]
    public async Task The_consumer_waits_for_the_DATABASE_which_means_waiting_for_its_server()
    {
        var model = ModelHarness.Build(Ex032_DatabaseInitScripts.Configure);

        Assert.IsType<PostgresDatabaseResource>(model.Resource("orders"));
        var api = model.Resource("orders-api");

        // Measured (ex002, ex015): WaitFor on a database CHILD leaves two annotations -
        // one for the child, one for its server - both WaitUntilHealthy. That pair is
        // the whole ordering claim of this row: the init scripts run inside the
        // Postgres entrypoint before it listens, so the server cannot report healthy
        // until they are done, and the consumer cannot start until the server does.
        var waits = api.Annotations.OfType<WaitAnnotation>().ToList();
        Assert.Equal(
            new[] { "orders", "pg" },
            waits.Select(w => w.Resource.Name).Order().ToArray());
        Assert.All(waits, w => Assert.Equal(WaitType.WaitUntilHealthy, w.WaitType));

        // Two mutants die here. WithReference(orders) alone - very easy to write, and
        // the model looks complete - leaves ZERO wait annotations. WaitFor(postgres)
        // instead of WaitFor(orders) leaves exactly ONE, for "pg", and starts the
        // consumer before Aspire has created the database it is about to connect to.
        // And nobody waits on the second server, which exists only as the obsolete
        // counterexample.
        Assert.DoesNotContain(waits, w => w.Resource.Name == "legacy");

        // The other half of the edge: the reference itself. Run the environment
        // callbacks, because that is the only place a WithReference is observable
        // (ex007) - the annotation type alone says nothing about which resource.
        var environment = new Dictionary<string, object>();
        var context = new EnvironmentCallbackContext(
            new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run),
            api, environment, TestContext.Current.CancellationToken);
        foreach (var callback in api.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            await callback.Callback(context);
        }

        Assert.Contains("ConnectionStrings__orders", environment.Keys);
        Assert.DoesNotContain("ConnectionStrings__legacy", environment.Keys);
    }
}
