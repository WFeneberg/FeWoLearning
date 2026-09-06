using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Put the track's two real ASP.NET Core services into the model as PROJECT
///         resources, and let each one's LAUNCH PROFILE decide its endpoints - the
///         default profile for one, an explicitly named profile for the other.
/// Drills: `AddProject`, ProjectResource, IProjectMetadata.ProjectPath, and the
///         launchProfileName argument. A project is not a container: Aspire builds
///         and runs it, and reads services/&lt;name&gt;/Properties/launchSettings.json to
///         learn which URLs it listens on. Those URLs become EndpointAnnotations
///         that nobody wrote by hand.
/// Passes: "catalog" and "orders" are both ProjectResources; each carries an
///         IProjectMetadata whose ProjectPath is the ABSOLUTE path of the real
///         .csproj on disk; "catalog" takes the DEFAULT profile and ends up with
///         exactly one endpoint (http, port 5080); "orders" names the "https"
///         profile and ends up with two (https 7081 and http 5081), plus a
///         LaunchProfileAnnotation recording the choice.
/// Note:   Read MicroServices/README.md section 5 before touching projectPath.
///         `exercises/` is a plain class library, so the generated
///         `Projects.Catalog` marker type every Aspire tutorial passes to
///         AddProject&lt;T&gt;() does NOT exist here - the (name, projectPath) overload
///         is the one to call. And projectPath resolves against
///         builder.AppHostDirectory, which is the TEST ASSEMBLY's output directory
///         under the harnesses and MicroServices/playground in the dashboard, so a
///         relative literal cannot be right in all three. ServiceProject below is
///         given to you for exactly that reason - use it.
/// </summary>
public static class Ex011_ProjectResources
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex011 - add the Catalog project as \"catalog\" using its DEFAULT "
            + "launch profile, and the Orders project as \"orders\" using the launch "
            + "profile named \"https\". Use ServiceProject(builder, \"Catalog\") and "
            + "ServiceProject(builder, \"Orders\") for the paths.");

    /// <summary>
    /// GIVEN, not a TODO. The absolute path of one of the track's shared services'
    /// project files, found by walking up from whatever directory the current host
    /// happens to be running in until the track's .slnx turns up.
    /// </summary>
    private static string ServiceProject(IDistributedApplicationBuilder builder, string name)
    {
        var dir = new DirectoryInfo(builder.AppHostDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "FeWoLearning.MicroServices.slnx")))
        {
            dir = dir.Parent;
        }

        var root = dir?.FullName
                   ?? throw new InvalidOperationException(
                       $"'{builder.AppHostDirectory}' is not inside MicroServices/.");

        return Path.Combine(root, "services", name, $"{name}.csproj");
    }
}
