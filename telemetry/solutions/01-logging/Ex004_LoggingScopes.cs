using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 004 — LoggingScopes (logging).
// Goal:   Attach the context that is true for a whole operation ONCE, instead of
//         repeating it on every message inside that operation.
// Drills: BeginScope, nested scopes, scope disposal, ambient vs per-record state.
// Passes: one Information record per line item, each carrying only its own Sku field;
//         every record carries exactly two scopes, outermost first - the tenant
//                     scope then the order scope;
//         the tenant scope carries TenantId and the order scope carries OrderId;
//         the per-item records do NOT carry TenantId or OrderId as their own fields;
//         and a record written after ProcessOrder returns carries no scopes at all.
//
// The fourth clause is the one that matters, and it is why this is not just "put the
// data somewhere". Repeating {TenantId} and {OrderId} on every message looks identical
// in a text log and costs a field per record forever, at every call site, including
// the ones a future maintainer adds and forgets. A scope says it once and covers
// everything inside it - including messages written by code you did not author.
//
// The fifth clause is the leak check: a scope that is never disposed silently attaches
// itself to every later record in the same execution context.
public static class Ex004_LoggingScopes
{
    /// <summary>
    /// For each SKU in <paramref name="lineItems"/>, write one Information record whose
    /// message is "picking {Sku}" - and nothing else.
    ///
    /// Wrap the whole loop in two nested scopes: an OUTER scope carrying the single
    /// named value TenantId, and an INNER scope carrying the single named value
    /// OrderId. Both scopes must be closed by the time this method returns.
    ///
    /// Either idiomatic scope shape works - a Dictionary&lt;string, object&gt; or
    /// BeginScope("...{TenantId}", tenantId) - as long as the name is carried.
    /// </summary>
    public static void ProcessOrder(
        ILogger logger, string tenantId, string orderId, IEnumerable<string> lineItems)
    {
        // Two nested `using` blocks, so both scopes are closed however the loop exits -
        // including on an exception, which is exactly when an undisposed scope would
        // start attaching itself to unrelated later records.
        using (logger.BeginScope(new Dictionary<string, object> { ["TenantId"] = tenantId }))
        using (logger.BeginScope(new Dictionary<string, object> { ["OrderId"] = orderId }))
        {
            foreach (var sku in lineItems)
            {
                // The message carries only what varies per record. Tenant and order are
                // true for the whole operation, so they are said once, above.
                logger.LogInformation("picking {Sku}", sku);
            }
        }
    }
}
