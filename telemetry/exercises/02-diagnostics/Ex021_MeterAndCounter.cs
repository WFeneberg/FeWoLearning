using System.Diagnostics.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 021 — MeterAndCounter (diagnostics).
// Goal:   Count things in a way you can still slice a year from now.
// Drills: Meter, Counter<T>, units, tags as dimensions.
// Passes: every call records 1 on a counter named "orders.processed", with the unit
//                     "{order}";
//         each measurement carries the outcome and the region as TAGS;
//         the meter publishes exactly ONE instrument, whatever the tag values are;
//         and two calls differing only in outcome produce two measurements on that one
//                     instrument, distinguishable by their tags.
//
// The third clause is the exercise, and the mistake it prevents is everywhere. The
// tempting shape is a counter per case - "orders.processed.accepted",
// "orders.processed.rejected" - because it is easy to write and reads fine in a list.
// It is also a dead end. Nobody can ask "how many orders in total", because that is
// now a sum over instrument NAMES that no query language addresses; adding a third
// outcome means shipping code; and every new dimension multiplies the instrument count
// instead of adding to it.
//
// A tag is a dimension. One instrument, sliced at query time by whatever combination
// the question needs. The cost is that dimensions with unbounded values - a user id, an
// order id, a raw URL - multiply the stored series instead, which is the failure mode
// row 050 is about. Bounded sets only.
//
// A Meter is created once and shared, like an ActivitySource: it is the unit a listener
// subscribes to, so a per-call instance would be a subscription nobody has.
public static class Ex021_MeterAndCounter
{
    /// <summary>The name this exercise's meter is registered under.</summary>
    public const string MeterName = "fewolearning.telemetry.ex021";

    /// <summary>The one instrument this exercise publishes.</summary>
    public const string InstrumentName = "orders.processed";

    /// <summary>What the counter counts, in UCUM's curly-brace form for a plain thing.</summary>
    public const string InstrumentUnit = "{order}";

    /// <summary>The dimension carrying how the order ended.</summary>
    public const string OutcomeTag = "order.outcome";

    /// <summary>The dimension carrying where it was processed.</summary>
    public const string RegionTag = "deployment.region";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Record that one order finished.
    ///
    /// Add 1 to a single <see cref="Counter{T}"/> of <see cref="long"/> named
    /// <see cref="InstrumentName"/> with unit <see cref="InstrumentUnit"/>, tagged
    /// <see cref="OutcomeTag"/> and <see cref="RegionTag"/>.
    ///
    /// One instrument. The outcome is a tag, never part of the name.
    /// </summary>
    public static void RecordProcessed(string outcome, string region) =>
        throw new NotImplementedException(
            "TODO: Ex021 - add 1 to one counter, with the outcome and region as tags");
}
