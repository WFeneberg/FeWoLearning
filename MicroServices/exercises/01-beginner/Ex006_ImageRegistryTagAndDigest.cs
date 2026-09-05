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
        => throw new NotImplementedException(
            "TODO: ex006 - add container 'api' whose registry is 'ghcr.io', image "
            + "'acme/api' and tag '2.4.1', each set separately; container 'cache' "
            + "(image 'redis') with only its tag overridden to '7.4'; and container "
            + "'pinned' (image 'acme/tool') pinned to the sha256 digest "
            + "'9f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0'.");
}
