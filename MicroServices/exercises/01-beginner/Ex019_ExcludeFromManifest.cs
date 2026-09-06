using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Keep a database web console in the graph for local development while
///         making sure it never reaches a deployment artifact - nobody wants an
///         unauthenticated admin UI published next to production data.
/// Drills: `ExcludeFromManifest`, and the fact that the built model and
///         aspire-manifest.json are two different things. The resource is fully
///         wired - image, endpoint, connection string reference - and Aspire starts
///         it locally like any other; the exclusion changes only what publish writes.
/// Passes: The built model contains "adminer" as a real ContainerResource on image
///         "adminer" with its own endpoint; aspire-manifest.json contains "pg" and
///         "api" and does NOT contain "adminer"; and "adminer" is the resource
///         carrying a ManifestPublishingCallbackAnnotation while "api" carries none.
/// Note:   Both halves are needed or the row grades nothing, because a resource that
///         was never added is also missing from the manifest. And the third fact is
///         not decoration: measured on 13.5.3, wrapping the AddContainer call in
///         `if (builder.ExecutionContext.IsRunMode)` produces a model and a manifest
///         BYTE-IDENTICAL to the correct answer as far as the first two facts can
///         see - the tests build in run mode - and is a different exercise (ex020).
///         The annotation is the only trace of which mechanism was used.
/// </summary>
public static class Ex019_ExcludeFromManifest
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex019 - add a Postgres server \"pg\" with a database \"orders\"; "
            + "add a container \"api\" on image \"nginx\" referencing \"orders\"; and "
            + "add a container \"adminer\" on image \"adminer\" with an http endpoint "
            + "named \"http\" on target port 8080, also referencing \"orders\", which "
            + "is present when the app runs but absent from the published manifest.");
}
