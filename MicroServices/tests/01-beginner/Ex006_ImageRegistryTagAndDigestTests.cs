using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex006_ImageRegistryTagAndDigestTests
{
    private static ContainerImageAnnotation Image(ModelHarness.Result model, string name)
        => Assert.Single(model.Resource(name).Annotations.OfType<ContainerImageAnnotation>());

    [Fact]
    public void Registry_image_and_tag_are_three_separate_fields()
    {
        var model = ModelHarness.Build(Ex006_ImageRegistryTagAndDigest.Configure);
        var image = Image(model, "api");

        // Measured first, so this fact is known to grade something: a bare
        // AddContainer("api", "nginx") ALREADY carries a ContainerImageAnnotation,
        // with Image="nginx", Tag="latest", Registry=null. Asserting that the
        // annotation exists therefore proves nothing at all - only its fields do.
        //
        // The mutant this rejects is the whole point of the row:
        // AddContainer("api", "ghcr.io/acme/api:2.4.1") is what most people write,
        // and it parses into Image="ghcr.io/acme/api", Tag="2.4.1", Registry=null
        // (measured). It fails on Image and on Registry, while producing a manifest
        // "image" string byte-identical to the correct answer - which is exactly why
        // this row is graded here and not against the manifest.
        Assert.Equal("acme/api", image.Image);
        Assert.Equal("2.4.1", image.Tag);
        Assert.Equal("ghcr.io", image.Registry);

        // ...and no digest, so the tag is what selects the image.
        Assert.Null(image.SHA256);
    }

    [Fact]
    public void Overriding_only_the_tag_leaves_the_registry_null()
    {
        var model = ModelHarness.Build(Ex006_ImageRegistryTagAndDigest.Configure);
        var image = Image(model, "cache");

        // Rejects the lazy fix for fact 1: a learner who discovers WithImageRegistry
        // and applies it to every container gets Registry="ghcr.io" here and fails.
        // A null registry is a real, different meaning - resolve against whatever
        // default the container runtime is configured with.
        Assert.Null(image.Registry);
        Assert.Equal("redis", image.Image);

        // And rejects leaving the tag alone: an untouched container reads "latest",
        // which is the un-pinned answer this row exists to argue against.
        Assert.Equal("7.4", image.Tag);

        // Accepted equivalents, measured rather than assumed: AddContainer("cache",
        // "redis", "7.4") and AddContainer("cache", "redis:7.4") both produce this
        // exact annotation. They are other spellings of the same model, not cheats;
        // what is rejected is a registry that was never asked for and a tag that was
        // never set.
    }

    [Fact]
    public void A_digest_pin_clears_the_tag()
    {
        var model = ModelHarness.Build(Ex006_ImageRegistryTagAndDigest.Configure);
        var image = Image(model, "pinned");

        Assert.Equal("acme/tool", image.Image);
        Assert.Equal(
            "9f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0",
            image.SHA256);

        // The mutant: WithImageTag("sha256:9f1e...") - putting the digest where the
        // tag goes. That leaves SHA256 null and Tag non-null, and fails both of the
        // assertions around it. Measured on Aspire 13.5.3: WithImageSHA256 nulls the
        // Tag even when WithImageTag was called first, because a digest and a tag are
        // two conflicting ways of naming which image to pull.
        Assert.Null(image.Tag);
    }
}
