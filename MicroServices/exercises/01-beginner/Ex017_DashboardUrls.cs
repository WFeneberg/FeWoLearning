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
        => throw new NotImplementedException(
            "TODO: ex017 - add a container \"storefront\" on image \"nginx\" with an "
            + "http endpoint named \"http\" on target port 8080; give it a fixed "
            + "documentation link to \"https://docs.contoso.com/storefront\" "
            + "displayed as \"API docs\"; and label the link Aspire generates for the "
            + "\"http\" endpoint \"Storefront\" without changing where that link "
            + "points.");
}
