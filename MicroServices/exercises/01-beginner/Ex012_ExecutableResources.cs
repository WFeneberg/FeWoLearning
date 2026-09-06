using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Model a local command-line tool next to a containerised cache, so the
///         two kinds of resource sit side by side and can be told apart.
/// Drills: `AddExecutable`, ExecutableResource, its Command, its WorkingDirectory
///         and its ordered args. An executable runs as a PROCESS on the developer's
///         machine - there is no image, no registry and no daemon - which is why it
///         carries no ContainerImageAnnotation and publishes as executable.v0
///         rather than container.v0.
/// Passes: "db-migrator" is an ExecutableResource whose Command is "dotnet", whose
///         WorkingDirectory is the track's services/ folder, and whose args are
///         exactly ["ef", "database", "update", "--project", "Catalog"] IN THAT
///         ORDER; it carries no ContainerImageAnnotation while "cache" - a plain
///         container on the redis image - does.
/// Note:   Measured on Aspire 13.5.3, and the reason the working directory is built
///         from ServiceRoot below rather than passed as "./services": a RELATIVE
///         workingDirectory is resolved to an absolute path against
///         builder.AppHostDirectory, exactly like a bind mount's source in ex009 -
///         and that directory is the test assembly's own output folder under the
///         harnesses, a different place in the red run, the green run and the
///         playground. In the published manifest the same path comes back relative
///         to the publish OUTPUT directory (a temp folder), i.e. a long ../../..
///         chain, so it is not a useful assertion target there either.
/// </summary>
public static class Ex012_ExecutableResources
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex012 - add a container \"cache\" on image \"redis\", and an "
            + "executable \"db-migrator\" running the command \"dotnet\" from "
            + "ServiceRoot(builder) with the arguments ef, database, update, "
            + "--project, Catalog - in that order.");

    /// <summary>
    /// GIVEN, not a TODO. The absolute path of the track's shared services/ folder,
    /// found by walking up from whatever directory the current host happens to be
    /// running in until the track's .slnx turns up. See ex011 and README section 5.
    /// </summary>
    private static string ServiceRoot(IDistributedApplicationBuilder builder)
    {
        var dir = new DirectoryInfo(builder.AppHostDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "FeWoLearning.MicroServices.slnx")))
        {
            dir = dir.Parent;
        }

        var root = dir?.FullName
                   ?? throw new InvalidOperationException(
                       $"'{builder.AppHostDirectory}' is not inside MicroServices/.");

        return Path.Combine(root, "services");
    }
}
