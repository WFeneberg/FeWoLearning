using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex009_VolumesAndBindMountsTests
{
    private static IReadOnlyList<ContainerMountAnnotation> Mounts(ModelHarness.Result model)
        => model.Resource("db").Annotations.OfType<ContainerMountAnnotation>().ToList();

    [Fact]
    public void The_named_volume_is_a_volume_with_a_name_not_a_path()
    {
        var model = ModelHarness.Build(Ex009_VolumesAndBindMounts.Configure);
        var mounts = Mounts(model);

        // Measured before writing this: a bare AddContainer carries NO
        // ContainerMountAnnotation, so both of these are the learner's doing - unlike
        // health checks, which integration resources bring along uninvited (see
        // ex004). Exactly two also rejects a third, stray mount.
        Assert.Equal(2, mounts.Count);

        var volume = Assert.Single(mounts, m => m.Type == ContainerMountType.Volume);

        // Source is the VOLUME NAME, and naming it is the point: the anonymous
        // overload WithVolume("/var/lib/postgresql/data") also yields a
        // ContainerMountType.Volume at the right target, but leaves Source null and
        // hands the runtime a random name, so nothing is shareable or findable after
        // the run. That mutant fails here.
        Assert.Equal("pgdata", volume.Source);
        Assert.Equal("/var/lib/postgresql/data", volume.Target);

        // A database that cannot write its own data directory is the read-only flag
        // pasted onto the wrong mount.
        Assert.False(volume.IsReadOnly);
    }

    [Fact]
    public void The_bind_mount_is_a_bind_mount_resolved_to_an_absolute_host_path()
    {
        var model = ModelHarness.Build(Ex009_VolumesAndBindMounts.Configure);
        var bind = Assert.Single(Mounts(model), m => m.Type == ContainerMountType.BindMount);

        Assert.Equal("/docker-entrypoint-initdb.d", bind.Target);

        // The mutant this rejects is WithVolume("./seed", "/docker-entrypoint-initdb.d"):
        // it compiles, it reads almost identically, and it silently means something
        // else - a runtime-managed volume literally NAMED "./seed", empty, with the
        // host's seed scripts nowhere in sight. Two assertions separate them. The
        // Type check is the direct one; the rooted-path check is the corroborating
        // one, because a volume's Source stays the literal string it was given while
        // a bind mount's is resolved against the AppHost directory (measured: "./seed"
        // becomes "<test output dir>\seed").
        Assert.NotNull(bind.Source);
        Assert.True(Path.IsPathRooted(bind.Source),
            $"A bind mount's source is resolved to an absolute host path; got '{bind.Source}'.");
        Assert.Equal("seed", Path.GetFileName(bind.Source.TrimEnd('/', '\\')));

        // Deliberately NOT asserting the whole path: it is the AppHost directory,
        // which under these harnesses is the test assembly's own output folder and
        // therefore differs between the red run and the green run.

        // Seed scripts are input. Dropping isReadOnly leaves this false.
        Assert.True(bind.IsReadOnly);
    }

    [Fact]
    public async Task The_manifest_lists_them_in_two_different_arrays()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex009_VolumesAndBindMounts.Configure,
            TestContext.Current.CancellationToken);

        var db = manifest.RootElement.GetProperty("resources").GetProperty("db");

        // The two mount kinds are not a flag on one list - they are two lists, which
        // is the artifact-level statement of "they differ in type, not just in the
        // source string". The both-as-bind-mounts mutant emits no "volumes" key at
        // all and the both-as-volumes mutant no "bindMounts" key, so each fails on
        // the very first line below.
        var volume = Assert.Single(db.GetProperty("volumes").EnumerateArray());
        Assert.Equal("pgdata", volume.GetProperty("name").GetString());
        Assert.Equal("/var/lib/postgresql/data", volume.GetProperty("target").GetString());
        Assert.False(volume.GetProperty("readOnly").GetBoolean());

        var bind = Assert.Single(db.GetProperty("bindMounts").EnumerateArray());
        Assert.Equal("/docker-entrypoint-initdb.d", bind.GetProperty("target").GetString());
        Assert.True(bind.GetProperty("readOnly").GetBoolean());

        // The bind mount's published source is relative to the manifest's own output
        // directory - a temp folder here - so only its last segment is stable enough
        // to assert.
        var source = bind.GetProperty("source").GetString();
        Assert.NotNull(source);
        Assert.EndsWith("/seed", source);
    }
}
