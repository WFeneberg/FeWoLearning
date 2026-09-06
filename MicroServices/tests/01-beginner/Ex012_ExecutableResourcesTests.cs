using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex012_ExecutableResourcesTests
{
    /// <summary>
    /// Args are not stored as a list - each call appends through a
    /// CommandLineArgsCallbackAnnotation, so running the callbacks in annotation order
    /// is the only way to see the command line the process would actually get. Same
    /// helper as ex008; an executable's args work exactly like a container's.
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
    public void The_migrator_is_a_process_with_a_command_and_a_working_directory()
    {
        var model = ModelHarness.Build(Ex012_ExecutableResources.Configure);

        // The positive half of the row, and the one that does the real work: the
        // mutant that models the tool as a container - AddContainer("db-migrator",
        // "mcr.microsoft.com/dotnet/sdk") - dies here, before any absence assertion
        // gets a chance to be vacuous.
        var migrator = Assert.IsType<ExecutableResource>(model.Resource("db-migrator"));

        // Command and args are separate fields, exactly as entrypoint and args are on
        // a container (ex008). Folding the whole command line into Command - i.e.
        // AddExecutable("db-migrator", "dotnet ef database update ...", dir) - fails
        // here and again on the arg sequence below.
        Assert.Equal("dotnet", migrator.Command);

        // Measured: a relative workingDirectory is resolved to an absolute host path
        // against builder.AppHostDirectory, which is the test assembly's own output
        // folder here - a DIFFERENT absolute path in the red run, the green run and
        // the playground. So the assertion is rootedness plus the last segment, never
        // the whole string. Same rule as a bind mount's Source in ex009.
        Assert.True(Path.IsPathRooted(migrator.WorkingDirectory),
            $"expected an absolute path, got '{migrator.WorkingDirectory}'");
        Assert.Equal("services", Path.GetFileName(migrator.WorkingDirectory.TrimEnd(
            Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)));
        Assert.True(Directory.Exists(migrator.WorkingDirectory),
            $"'{migrator.WorkingDirectory}' does not exist");
    }

    [Fact]
    public async Task Its_arguments_are_five_separate_ordered_strings()
    {
        var model = ModelHarness.Build(Ex012_ExecutableResources.Configure);

        var args = await ResolveArgsAsync(
            model.Resource("db-migrator"), TestContext.Current.CancellationToken);

        // Assert.Equal over a List<string> is ordered and element-wise, which rejects
        // two different wrong answers at once: passing the arguments as one
        // space-joined string (one element, not five - a shell would split it, a
        // process launch does not), and reordering "--project Catalog" ahead of the
        // verb, which is a different command line made of the same five strings.
        Assert.Equal(new[] { "ef", "database", "update", "--project", "Catalog" }, args);
    }

    [Fact]
    public void An_executable_has_no_image_and_the_container_beside_it_does()
    {
        var model = ModelHarness.Build(Ex012_ExecutableResources.Configure);

        // The row's stated discriminator is an ABSENCE, and an absence proves nothing
        // on its own - Assert.Empty over the wrong resource, a misspelt name, an empty
        // model would all satisfy it. So the positive half is graded in the same fact,
        // against a container built by the same Configure: "cache" DOES carry a
        // ContainerImageAnnotation on the image the row asked for, and "db-migrator"
        // carries none. Only a model with both kinds of resource in it passes.
        var cache = Assert.IsType<ContainerResource>(model.Resource("cache"));
        var image = Assert.Single(cache.Annotations.OfType<ContainerImageAnnotation>());
        Assert.Equal("redis", image.Image);

        Assert.Empty(model.Resource("db-migrator").Annotations.OfType<ContainerImageAnnotation>());
    }

    [Fact]
    public async Task The_manifest_publishes_them_as_two_different_types()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex012_ExecutableResources.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");
        var migrator = resources.GetProperty("db-migrator");
        var cache = resources.GetProperty("cache");

        // The same distinction survives publish, and a deployment target reads it: one
        // of these is a process to launch, the other an image to pull. There is no
        // "image" key on an executable at all - not an empty one - so the container
        // mutant fails here too, at the artifact level rather than only in the model.
        Assert.Equal("executable.v0", migrator.GetProperty("type").GetString());
        Assert.Equal("container.v0", cache.GetProperty("type").GetString());
        Assert.False(migrator.TryGetProperty("image", out _));
        Assert.Equal("redis:latest", cache.GetProperty("image").GetString());

        Assert.Equal("dotnet", migrator.GetProperty("command").GetString());
        Assert.Equal(
            new[] { "ef", "database", "update", "--project", "Catalog" },
            migrator.GetProperty("args").EnumerateArray().Select(a => a.GetString()).ToArray());

        // Deliberately NOT asserted: migrator.workingDirectory. Measured - the manifest
        // re-expresses it RELATIVE to the publish output directory, which is a fresh
        // temp folder per run, so it comes back as a long ../../.. chain that differs
        // every time. The model-level assertion above is where that field is graded.
    }
}
