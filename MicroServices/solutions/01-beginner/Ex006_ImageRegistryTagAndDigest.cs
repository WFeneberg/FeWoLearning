using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Pin three container images the way a deployable AppHost has to - by the
///         SEPARATE parts of the reference, never by one opaque string.
/// Drills: `WithImageRegistry`, `WithImage`, `WithImageTag` and `WithImageSHA256`,
///         all four of which write into the one ContainerImageAnnotation a
///         container already carries. `AddContainer("api", "nginx")` arrives with
///         Image="nginx", Tag="latest", Registry=null, SHA256=null - so the
///         annotation's mere presence proves nothing, and only its four fields do.
/// Passes: "api" is registry ghcr.io + image acme/api + tag 2.4.1, held in three
///         separate fields; "cache" overrides only its tag and keeps a null
///         registry; "pinned" carries a digest and, as a consequence, no tag.
/// Note:   Measured on Aspire 13.5.3 - the published manifest renders all of this
///         back down to a single "image" string, and
///         AddContainer("api", "ghcr.io/acme/api:2.4.1") produces the identical
///         string. The manifest therefore cannot grade this exercise; the model can.
/// </summary>
public static class Ex006_ImageRegistryTagAndDigest
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // Three calls, three fields. The reference the runtime finally pulls is
        // ghcr.io/acme/api:2.4.1, but nothing in the model ever holds that string:
        // a deployment tool that has to retag for a private registry rewrites
        // Registry alone, which is only possible because it is its own field.
        builder.AddContainer("api", "nginx")
               .WithImageRegistry("ghcr.io")
               .WithImage("acme/api")
               .WithImageTag("2.4.1");

        // Only the tag is overridden here, so Registry stays null and the image
        // resolves against the runtime's default registry.
        builder.AddContainer("cache", "redis")
               .WithImageTag("7.4");

        // A digest is not a tag - it names one immutable image. Aspire clears Tag
        // when a digest is set, because "acme/tool:1.0@sha256:..." would be two
        // conflicting ways to say which image to pull.
        builder.AddContainer("pinned", "acme/tool")
               .WithImageSHA256("9f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0");
    }
}
