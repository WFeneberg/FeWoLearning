using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex008_ContainerArgsAndEntrypointTests
{
    /// <summary>
    /// Args are not stored as a list - each WithArgs call adds a
    /// CommandLineArgsCallbackAnnotation whose callback APPENDS to a list handed to
    /// it. Running them in annotation order is the only way to see the command line
    /// the container would actually get, which is also why order is observable here.
    /// </summary>
    private static async Task<List<string>> ResolveArgsAsync(IResource resource, CancellationToken ct)
    {
        var args = new List<object>();
        var context = new CommandLineArgsCallbackContext(args, ct);
        foreach (var annotation in resource.Annotations.OfType<CommandLineArgsCallbackAnnotation>())
        {
            await annotation.Callback(context);
        }
        return args.Select(a => a.ToString()!).ToList();
    }

    [Fact]
    public async Task Entrypoint_and_arguments_are_separate_and_ordered()
    {
        var model = ModelHarness.Build(Ex008_ContainerArgsAndEntrypoint.Configure);
        var cache = Assert.IsType<ContainerResource>(model.Resource("cache"));

        // The entrypoint is a property of the resource, not an argument. The mutant:
        // WithArgs("redis-server", "--port", ...) with no WithEntrypoint. It leaves
        // Entrypoint null and puts the program name at args[0] - so it fails here
        // AND on the sequence below.
        Assert.Equal("redis-server", cache.Entrypoint);

        var args = await ResolveArgsAsync(cache, TestContext.Current.CancellationToken);

        // Assert.Equal on a List<string> is an ORDERED, element-wise comparison, and
        // that is deliberate. "--appendonly yes --port 6379" contains the same four
        // strings and is a different command line; a set-equality or
        // Assert.Contains-per-item test would call it correct and grade nothing.
        Assert.Equal(new[] { "--port", "6379", "--appendonly", "yes" }, args);
    }

    [Fact]
    public async Task Arguments_need_no_entrypoint_override()
    {
        var model = ModelHarness.Build(Ex008_ContainerArgsAndEntrypoint.Configure);
        var worker = Assert.IsType<ContainerResource>(model.Resource("worker"));

        // Rejects the learner who, having found WithEntrypoint for "cache", reaches
        // for it again here - e.g. WithEntrypoint("sleep").WithArgs("3600"), which
        // sets Entrypoint and shortens args to one element. Both assertions fail.
        // A null entrypoint means "keep the image's own ENTRYPOINT", which is the
        // right answer for busybox.
        Assert.Null(worker.Entrypoint);

        var args = await ResolveArgsAsync(worker, TestContext.Current.CancellationToken);
        Assert.Equal(new[] { "sleep", "3600" }, args);
    }

    [Fact]
    public async Task The_manifest_keeps_entrypoint_and_args_apart()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex008_ContainerArgsAndEntrypoint.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");
        var cache = resources.GetProperty("cache");

        // Two fields, not one concatenated command string - the deployment target is
        // expected to set them separately. This is what rejects the "just put it all
        // in args" mutant at the artifact level rather than only in the model.
        Assert.Equal("redis-server", cache.GetProperty("entrypoint").GetString());
        Assert.Equal(
            new[] { "--port", "6379", "--appendonly", "yes" },
            cache.GetProperty("args").EnumerateArray().Select(a => a.GetString()).ToArray());

        // And a container that overrode no entrypoint emits no "entrypoint" key at
        // all, rather than an empty string - so the mutant that sets one on "worker"
        // fails here too.
        var worker = resources.GetProperty("worker");
        Assert.False(worker.TryGetProperty("entrypoint", out _));
        Assert.Equal(
            new[] { "sleep", "3600" },
            worker.GetProperty("args").EnumerateArray().Select(a => a.GetString()).ToArray());
    }
}
