using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex017_DashboardUrlsTests
{
    private const string DocsUrl = "https://docs.contoso.com/storefront";

    /// <summary>The address Aspire would have allocated for the "http" endpoint.</summary>
    private const string EndpointUrl = "http://localhost:33445";

    /// <summary>
    /// What Aspire does once endpoints have been allocated: every endpoint gets a
    /// ResourceUrlAnnotation of its own, then every ResourceUrlsCallbackAnnotation on
    /// the resource runs over the whole list and may add to it or edit it. L1 has no
    /// allocated endpoints, so the endpoint's url is seeded here by hand with the one
    /// field the callbacks can match on - Endpoint - and a placeholder address; a
    /// callback that respects the address it was given leaves that placeholder alone.
    /// </summary>
    private static List<ResourceUrlAnnotation> RunUrlCallbacks(IResource resource)
    {
        var urls = resource.Annotations.OfType<ResourceUrlAnnotation>().ToList();
        urls.Add(new ResourceUrlAnnotation
        {
            Url = EndpointUrl,
            Endpoint = new EndpointReference((IResourceWithEndpoints)resource, "http")
        });

        var context = new ResourceUrlsCallbackContext(
            new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run),
            resource,
            urls);

        foreach (var callback in resource.Annotations.OfType<ResourceUrlsCallbackAnnotation>())
        {
            callback.Callback(context).GetAwaiter().GetResult();
        }

        return context.Urls;
    }

    [Fact]
    public void WithUrl_records_a_url_now_while_WithUrlForEndpoint_records_only_a_callback()
    {
        var model = ModelHarness.Build(Ex017_DashboardUrls.Configure);
        var storefront = model.Resource("storefront");

        // EXACTLY one, and that is the discriminating count. Measured on 13.5.3:
        // WithUrl writes its annotation immediately, WithUrlForEndpoint writes none.
        // So the plausible wrong answer - a second WithUrl("http://localhost:8080",
        // "Storefront") standing in for the endpoint label - produces TWO url
        // annotations here and fails, while still looking right on the dashboard the
        // first time the ports happen to line up.
        var docs = Assert.Single(storefront.Annotations.OfType<ResourceUrlAnnotation>());

        // Three fields, three separate assertions. Passing the display text as the
        // address (or the address as the display text) satisfies "there is a url
        // annotation" perfectly well and is exactly what the catalog row calls out.
        Assert.Equal(DocsUrl, docs.Url);
        Assert.Equal("API docs", docs.DisplayText);
        Assert.Null(docs.Endpoint);

        // The deferred half has to be there too, or nothing decorates the endpoint.
        Assert.Single(storefront.Annotations.OfType<ResourceUrlsCallbackAnnotation>());
    }

    [Fact]
    public void The_endpoint_url_gets_its_display_text_from_the_callback_and_keeps_its_address()
    {
        var model = ModelHarness.Build(Ex017_DashboardUrls.Configure);
        var urls = RunUrlCallbacks(model.Resource("storefront"));

        var endpointUrl = Assert.Single(urls, u => u.Endpoint is not null);
        Assert.Equal("http", endpointUrl.Endpoint!.EndpointName);

        // The label the row asks for.  WithUrlForEndpoint("https", ...) - the wrong
        // endpoint name - is a silent no-op on 13.5.3 (Aspire logs a warning and
        // moves on), so the DisplayText would still be null here.
        Assert.Equal("Storefront", endpointUrl.DisplayText);

        // And the callback did not rewrite where the link goes. `url.Url =
        // "Storefront"` - the display text put into the address field - is the
        // mistake this pins, and it produces a dashboard link that 404s while the
        // text above still reads correctly.
        Assert.Equal(EndpointUrl, endpointUrl.Url);
    }

    [Fact]
    public void The_two_links_stay_two_links_with_the_endpoint_on_exactly_one_of_them()
    {
        var model = ModelHarness.Build(Ex017_DashboardUrls.Configure);
        var urls = RunUrlCallbacks(model.Resource("storefront"));

        // Two, not three: the endpoint's own url plus the docs link. The other
        // WithUrlForEndpoint overload - the Func<EndpointReference,
        // ResourceUrlAnnotation> one - ADDS a url for the endpoint rather than
        // customising the generated one, so it lands a third entry here.
        Assert.Equal(2, urls.Count);

        // Stated as absences, so a later edit cannot quietly drop one half of the
        // "display text and endpoint are two different fields" claim: no link points
        // at the docs while claiming to be the endpoint's, and no link labelled
        // "Storefront" is floating free of the endpoint it names.
        Assert.DoesNotContain(urls, u => u.Endpoint is not null && u.Url == DocsUrl);
        Assert.DoesNotContain(urls, u => u.Endpoint is null && u.DisplayText == "Storefront");
    }
}
