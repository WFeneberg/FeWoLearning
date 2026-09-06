using System.Diagnostics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 017 — TagsBaggageEvents (diagnostics).
// Goal:   Tell apart the three things you can attach to an activity, because they have
//         three different lifetimes and only one of them travels.
// Drills: SetTag, AddBaggage, AddEvent.
// Passes: the outer activity carries the region TAG; the inner one does not;
//         the inner activity can read the tenant BAGGAGE the outer set;
//         the inner carries one "retry.scheduled" event per retry, each tagged with
//                     its attempt number;
//         and the tenant never appears as a tag on either activity.
//
// The first two clauses are the distinction people get wrong, in both directions.
//
// A TAG belongs to one activity. It does not inherit: setting deployment.region on a
// parent does nothing for its children, so a child span arrives at the backend without
// it and the dashboard filtered on region silently loses half its rows.
//
// BAGGAGE inherits. It rides down the whole subtree, and across process boundaries if
// a propagator carries it. That is what makes it useful and what makes it dangerous:
// everything you put in it is copied onto every outbound request for the rest of the
// trace, so a tenant id is fine and anything personal is a leak with a long reach.
//
// The last clause is the one that catches the shortcut. Baggage is NOT automatically a
// span attribute - it is context, not data, and no backend indexes it unless something
// deliberately copies it onto a span. Doing both here would hide whether the learner
// understood which mechanism did the work.
//
// An EVENT is a timestamped note inside one activity. It is the right shape for
// "this happened, n times, at these moments" - a retry, a cache miss, a lock wait -
// where a tag would only be able to say "n" and lose the when.
public static class Ex017_TagsBaggageEvents
{
    /// <summary>The name this exercise's source is registered under.</summary>
    public const string SourceName = "fewolearning.telemetry.ex017";

    /// <summary>The baggage key carrying the tenant. Inherits down the subtree.</summary>
    public const string TenantBaggageKey = "tenant.id";

    /// <summary>The tag key carrying the region. Belongs to one activity only.</summary>
    public const string RegionTagKey = "deployment.region";

    /// <summary>The name of the event recorded once per retry.</summary>
    public const string RetryEventName = "retry.scheduled";

    /// <summary>The tag on each retry event carrying which attempt it was.</summary>
    public const string AttemptTagKey = "retry.attempt";

    /// <summary>The name of the outer activity.</summary>
    public const string RequestName = "request";

    /// <summary>The name of the inner activity.</summary>
    public const string HandlerName = "handler";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Start a <see cref="RequestName"/> activity that:
    ///
    ///   - carries <paramref name="region"/> as the tag <see cref="RegionTagKey"/>;
    ///   - carries <paramref name="tenantId"/> as the baggage
    ///     <see cref="TenantBaggageKey"/>, and NOT as a tag.
    ///
    /// Inside it, start a <see cref="HandlerName"/> activity that records
    /// <paramref name="retries"/> events named <see cref="RetryEventName"/>, the
    /// n-th tagged <see cref="AttemptTagKey"/> with n, counting from 1. Add no tags of
    /// its own to the handler.
    /// </summary>
    public static void HandleRequest(string tenantId, string region, int retries) =>
        throw new NotImplementedException(
            "TODO: Ex017 - tag the outer, put the tenant in baggage, and record one event per retry on the inner");
}
