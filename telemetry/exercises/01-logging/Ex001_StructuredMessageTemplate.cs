using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 001 — StructuredMessageTemplate (logging).
// Goal:   Log one payment failure so that a machine can query it, not only a human
//         read it.
// Drills: message templates vs string interpolation, named placeholders, the
//         {OriginalFormat} entry every template leaves behind.
// Passes: the record carries the named fields OrderId, Amount and Reason, whose
//         values are the arguments;
//         the rendered message reads exactly
//                     "Payment for order O-42 of 19.99 failed: insufficient funds";
//         two calls with DIFFERENT arguments leave the SAME {OriginalFormat} value;
//         and that value contains the literal text "{OrderId}".
//
// The last two clauses are the ones that matter. $"Payment for order {orderId} ..."
// renders identical text and carries no named fields at all - so the log is
// unqueryable, and every call site invents its own new "template". Anything that
// aggregates, alerts on, or filters these logs works on the fields and on the
// constant template, never on the sentence.
public static class Ex001_StructuredMessageTemplate
{
    /// <summary>
    /// Write ONE Information-level record describing a failed payment.
    ///
    /// The rendered message must read
    /// "Payment for order {OrderId} of {Amount} failed: {Reason}" with the three
    /// values substituted, and the record must carry those three names as fields.
    /// </summary>
    public static void LogPaymentFailed(ILogger logger, string orderId, decimal amount, string reason) =>
        throw new NotImplementedException(
            "TODO: Ex001 - log the failure with a message template, not an interpolated string");
}
