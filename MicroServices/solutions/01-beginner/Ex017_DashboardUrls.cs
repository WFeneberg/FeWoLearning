using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Give one resource two dashboard links that mean different things: a fixed
///         link to documentation that lives outside the model entirely, and a
///         friendly label on the link the resource's own HTTP endpoint already
///         generates.
/// Drills: `WithUrl` versus `WithUrlForEndpoint`, and the fields of
///         ResourceUrlAnnotation. DisplayText is what the dashboard prints; Url is
///         where the link goes; Endpoint says WHICH endpoint the link belongs to and
///         is null for a link that belongs to none. Those are three fields, not one
///         string, and the row is graded on keeping them apart.
/// Passes: At model time "storefront" carries exactly ONE ResourceUrlAnnotation - the
///         docs link, Url "https://docs.contoso.com/storefront", DisplayText
///         "API docs", Endpoint null - plus exactly one ResourceUrlsCallbackAnnotation.
///         Running that callback over the URL Aspire generates for the "http"
///         endpoint leaves the URL's address untouched and its DisplayText set to
///         "Storefront".
/// Note:   Measured on 13.5.3, and it is the reason this row runs a callback instead
///         of reading annotations only: WithUrl writes its ResourceUrlAnnotation
///         IMMEDIATELY, while WithUrlForEndpoint writes NO url annotation at all -
///         just a ResourceUrlsCallbackAnnotation that Aspire runs later, once
///         endpoints have been allocated and the endpoint's own url exists to be
///         decorated. So a learner who reaches for WithUrl twice produces two url
///         annotations at model time, both with a null Endpoint, and has attached
///         nothing to the endpoint.
/// </summary>
public static class Ex017_DashboardUrls
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        builder.AddContainer("storefront", "nginx")
               .WithHttpEndpoint(targetPort: 8080, name: "http")

               // A link that belongs to no endpoint. It is known at model time, so
               // Aspire records it as a ResourceUrlAnnotation right away, with
               // Endpoint left null. The second argument is the DISPLAY TEXT, not a
               // second address - swapping the two produces a dashboard entry
               // labelled with a URL that points at "API docs".
               .WithUrl("https://docs.contoso.com/storefront", "API docs")

               // A label on the link the "http" endpoint already generates. The
               // address is not known yet - the host port is allocated at run time -
               // so this cannot be an annotation now, and is registered as a callback
               // Aspire runs once the endpoint has an address. The callback only
               // touches DisplayText; the Url it was handed stays exactly as
               // allocated, which is what keeps the link working.
               .WithUrlForEndpoint("http", url => url.DisplayText = "Storefront");
    }
}
