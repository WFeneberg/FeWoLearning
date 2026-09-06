using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex011_ProjectResourcesTests
{
    /// <summary>
    /// The endpoints on a resource, keyed by name, as (scheme, port) pairs - the two
    /// fields a launch profile's applicationUrl actually determines.
    /// </summary>
    private static Dictionary<string, (string Scheme, int? Port)> EndpointsOf(IResource resource)
        => resource.Annotations.OfType<EndpointAnnotation>()
                   .ToDictionary(e => e.Name, e => (e.UriScheme, e.Port));

    [Fact]
    public void Both_services_enter_the_model_as_project_resources()
    {
        var model = ModelHarness.Build(Ex011_ProjectResources.Configure);

        // The TYPE is the first grade. AddContainer("catalog", "mcr.microsoft.com/...")
        // would satisfy a name-only assertion, and would be a container that Aspire
        // pulls rather than a project it builds, runs and discovers. ProjectResource
        // is also the only one of the two that is IResourceWithServiceDiscovery, which
        // is what row 056 later builds on.
        var catalog = Assert.IsType<ProjectResource>(model.Resource("catalog"));
        var orders = Assert.IsType<ProjectResource>(model.Resource("orders"));

        // IProjectMetadata is how the model records WHICH project. Its ProjectPath is
        // the fully resolved ABSOLUTE path - AddProject resolves whatever it was given
        // against builder.AppHostDirectory before storing it - so the assertion is on
        // rootedness plus the tail, never on a whole literal that would differ between
        // machines.
        var catalogPath = Assert.Single(catalog.Annotations.OfType<IProjectMetadata>()).ProjectPath;
        var ordersPath = Assert.Single(orders.Annotations.OfType<IProjectMetadata>()).ProjectPath;

        Assert.True(Path.IsPathRooted(catalogPath), $"expected an absolute path, got '{catalogPath}'");
        Assert.True(File.Exists(catalogPath), $"'{catalogPath}' does not exist");
        Assert.True(File.Exists(ordersPath), $"'{ordersPath}' does not exist");

        // ...and that the two name DIFFERENT projects, the near-miss being a
        // copy-pasted ServiceProject(builder, "Catalog") on both lines. Everything
        // else in this file would still pass with Orders' endpoints coming out of
        // Catalog's launchSettings.json, because the two files are deliberately
        // similar; only this pair of assertions notices.
        Assert.EndsWith(Path.Combine("services", "Catalog", "Catalog.csproj"), catalogPath);
        Assert.EndsWith(Path.Combine("services", "Orders", "Orders.csproj"), ordersPath);

        // Not asserted, and deliberately: Aspire adds a second, hidden resource per
        // project - "catalog-rebuilder", a ProjectRebuilderResource - all by itself.
        // Measured on 13.5.3. Nobody asked for it, so it grades nothing; it is
        // recorded here only so a later reader is not surprised by a model that holds
        // four resources when the exercise wrote two.
    }

    [Fact]
    public void The_default_launch_profile_supplies_catalogs_only_endpoint()
    {
        var model = ModelHarness.Build(Ex011_ProjectResources.Configure);
        var catalog = model.Resource("catalog");

        // Catalog's launchSettings.json has two profiles; passing no launchProfileName
        // takes the FIRST, "http", whose applicationUrl is a single URL. One URL, one
        // endpoint: a learner who reached for the "https" profile here gets two and
        // fails on the count.
        var endpoints = EndpointsOf(catalog);
        Assert.Equal(new[] { "http" }, endpoints.Keys.Order().ToArray());
        Assert.Equal(("http", (int?)5080), endpoints["http"]);

        // The mutant this rejects, and it is the plausible one - a learner who could
        // not get the profile to apply and hand-rolled the endpoint instead:
        //
        //     builder.AddProject("catalog", path, launchProfileName: null)
        //            .WithHttpEndpoint(port: 5080, name: "http");
        //
        // Measured: that produces an EndpointAnnotation identical to the profile's in
        // EVERY observable field - same name, same scheme, same Port, TargetPort still
        // null, IsProxied still true - so nothing above can see the difference. What
        // it also produces is an ExcludeLaunchProfileAnnotation, which is the only
        // trace left of "I opted out of launchSettings.json". Hence this line.
        Assert.Empty(catalog.Annotations.OfType<ExcludeLaunchProfileAnnotation>());
    }

    [Fact]
    public void Naming_the_https_profile_is_what_gives_orders_two_endpoints()
    {
        var model = ModelHarness.Build(Ex011_ProjectResources.Configure);
        var orders = model.Resource("orders");

        // Passing launchProfileName records the choice. The default-profile case above
        // carries no LaunchProfileAnnotation at all, so this assertion is what
        // separates "asked for the https profile" from "took whatever was first".
        Assert.Equal(
            "https",
            Assert.Single(orders.Annotations.OfType<LaunchProfileAnnotation>()).LaunchProfileName);

        // Orders' "https" profile lists two URLs in one applicationUrl
        // ("https://localhost:7081;http://localhost:5081"), and Aspire turns each into
        // its own EndpointAnnotation named after its scheme. Both halves are pinned:
        //   * the default-profile mutant (no launchProfileName) yields ONE endpoint,
        //     http on 5081, and fails on the key set;
        //   * the hand-rolled mutant - WithHttpsEndpoint(7081).WithHttpEndpoint(5081)
        //     over launchProfileName: null - yields the right two and fails on the
        //     LaunchProfileAnnotation above;
        //   * pointing this resource at Catalog's project file yields 7080/5080 and
        //     fails on the ports.
        var endpoints = EndpointsOf(orders);
        Assert.Equal(new[] { "http", "https" }, endpoints.Keys.Order().ToArray());
        Assert.Equal(("https", (int?)7081), endpoints["https"]);
        Assert.Equal(("http", (int?)5081), endpoints["http"]);
    }
}
