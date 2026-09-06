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
    {
        var orders = builder.AddPostgres("pg")
                            .AddDatabase("orders");

        // The service that does reach production, so nothing about it is excluded.
        builder.AddContainer("api", "nginx")
               .WithReference(orders);

        // A fully wired resource: it has an image, an endpoint the dashboard links
        // to, and the same connection string the API gets. ExcludeFromManifest does
        // not weaken any of that - it only tells the publisher to skip the resource,
        // so it runs locally and does not exist in aspire-manifest.json. Deleting the
        // resource, or writing it only in run mode, would produce the same manifest
        // by different and worse means.
        builder.AddContainer("adminer", "adminer")
               .WithHttpEndpoint(targetPort: 8080, name: "http")
               .WithReference(orders)
               .ExcludeFromManifest();
    }
}
