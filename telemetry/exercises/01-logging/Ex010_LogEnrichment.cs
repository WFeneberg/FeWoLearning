using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 010 — LogEnrichment (logging).
// Goal:   Attach the facts that are true for the whole PROCESS once, in one place,
//         instead of asking every call site to remember them.
// Drills: decorating ILogger, rebuilding the log state, delegating the rest.
// Passes: every record written through the wrapper carries ServiceVersion and
//                     MachineName as named fields;
//         the record still carries its own fields and its rendered message is
//                     unchanged;
//         two different call sites both get the ambient fields;
//         {OriginalFormat} is still the call site's own constant template;
//         and IsEnabled and BeginScope are delegated to the inner logger.
//
// The fourth clause is what separates enrichment from string-mangling. An
// implementation that appends " (v1.4.0 on BUILD-07)" to the message looks right in a
// console and is a disaster in a backend: the template changes, so every record
// becomes its own event type, and the two values are text rather than queryable
// fields. Enrichment adds STATE, it does not edit the sentence.
//
// The fifth clause is the quiet one. A decorator that forgets to forward IsEnabled
// answers "yes" to everything, so every filter rule in the application stops working -
// and nothing anywhere reports an error. One that swallows BeginScope silently drops
// all the context from ex004.
public static class Ex010_LogEnrichment
{
    /// <summary>The field name carrying the deployed version.</summary>
    public const string VersionField = "ServiceVersion";

    /// <summary>The field name carrying the host the process runs on.</summary>
    public const string MachineField = "MachineName";

    /// <summary>
    /// Wrap <paramref name="inner"/> so that every record written through the returned
    /// logger also carries <see cref="VersionField"/> and <see cref="MachineField"/>,
    /// with the values given here, without any call site mentioning them.
    ///
    /// The call site's own fields, its constant template and its rendered message must
    /// all survive untouched, and everything that is not logging - IsEnabled,
    /// BeginScope - must reach <paramref name="inner"/>.
    /// </summary>
    public static ILogger Enrich(ILogger inner, string serviceVersion, string machineName) =>
        throw new NotImplementedException(
            "TODO: Ex010 - return a decorator that adds ambient fields to the state and delegates the rest");
}
