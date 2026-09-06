using System.Text.Json;
using System.Text.Json.Nodes;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex052;

/// <summary>
/// The shape today's code works with. v1 had no Currency at all; the upcaster supplies
/// one, so nothing downstream ever has to know v1 existed.
/// </summary>
public sealed record OrderPlaced(string OrderId, decimal Amount, string Currency);

public sealed class EventSchemaException(string message) : Exception(message);

// Exercise 052 — EventSchemaEvolution (services-data).
// Goal:   Read events written by older and newer versions of the code than the one
//         reading them, because in a running system all three exist at once.
// Drills: versioned events, upcasting, the tolerant reader.
// Passes: current       - a v2 payload reads straight through.
//         older         - a v1 payload (no "currency") is UPCAST, gaining the default
//                         "EUR"; the caller cannot tell.
//         THE ONE        - a payload carrying a field this build has never heard of does
//                         NOT throw. It is read for what is understood and the rest is
//                         ignored.
//         malformed     - a payload missing "orderId" throws EventSchemaException naming
//                         the field. Tolerant is not the same as credulous.
//
// The unknown-field clause is the tolerant reader, and it is the one that decides whether
// a rolling deployment is possible at all. During any rollout, instances running the NEW
// code publish events with the new field while instances running the OLD code are still
// consuming - and if the old consumers reject what they do not recognise, every deploy is
// an outage, or a stop-the-world affair with a maintenance window.
//
// Strict deserialisation is the wrong mechanism here, and it looks like rigour. It reads
// a v1 payload perfectly, reads a v2 payload perfectly, and takes the system down the
// first time somebody adds a field.
public static class Ex052_EventSchemaEvolution
{
    public const string DefaultCurrency = "EUR";

    /// <summary>Read a payload of any version into today's shape.</summary>
    public static OrderPlaced Read(string json) =>
        throw new NotImplementedException(
            "TODO: Ex052 - read orderId and amount, default a missing currency, ignore unknown fields, and reject a missing orderId by name");
}
