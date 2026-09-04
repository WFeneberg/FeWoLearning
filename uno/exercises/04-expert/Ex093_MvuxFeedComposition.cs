// Exercise 093 - MVUX Feed Composition (expert).
// Goal:   Build a feed out of other feeds, and know what travels along each axis.
// Drills: Select, Where and Feed.Combine; a projection that is not called when the source
//         failed; and the difference between None and Undefined in the result.
// Passes: dotnet test --filter FullyQualifiedName~Ex093_
//
// Composition is the reason feeds beat a hand-rolled loading state machine. A projection
// maps the data axis and leaves the other two alone: an error travels through untouched
// and the projection never runs, so no downstream code has to check for a value that is
// not there. Combining two feeds gives one message per pair, and either failure fails the
// pair.
//
// The last distinction is the one to take away. A Where that excludes a value produces
// None - "there deliberately is no value" - while a failure produces Undefined. A view can
// show "no matches" for the first and "something went wrong" for the second, which a single
// null could never express.

using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>What one composed message turned out to be.</summary>
/// <param name="Value">The data, when there was any.</param>
/// <param name="HasValue">Whether <paramref name="Value"/> means anything.</param>
/// <param name="IsEmpty">Whether the absence was deliberate - a None.</param>
/// <param name="Error">The failure message, or null.</param>
public sealed record Ex093_Outcome(string Value, bool HasValue, bool IsEmpty, string? Error);

public static class Ex093_MvuxFeedComposition
{
    /// <summary>How many times the projection given to <see cref="Format"/> has run.</summary>
    public static int Projections { get; private set; }

    /// <summary>Resets the counter between tests.</summary>
    public static void ResetProjections() => Projections = 0;

    /// <summary>
    /// A feed of <c>"#n"</c> for each value of <paramref name="source"/>, counting each
    /// projection in <see cref="Projections"/>.
    /// </summary>
    public static IFeed<string> Format(IFeed<int> source) =>
        // TODO: Select maps the data axis. Counting inside the projection is how the test
        // proves it is not called for a message that carried no data.
        throw new NotImplementedException("TODO: Ex093 - project the source's data");

    /// <summary>
    /// A feed carrying only the values of <paramref name="source"/> that are at least
    /// <paramref name="minimum"/>.
    /// </summary>
    public static IFeed<int> AtLeast(IFeed<int> source, int minimum) =>
        throw new NotImplementedException("TODO: Ex093 - filter the source");

    /// <summary>
    /// A feed of <c>"name=value"</c> from the two sources, one message per pair.
    /// </summary>
    public static IFeed<string> Describe(IFeed<string> name, IFeed<int> value) =>
        // TODO: Feed.Combine pairs them into a feed of tuples; project that into the string.
        throw new NotImplementedException("TODO: Ex093 - combine the two feeds");

    /// <summary>
    /// Flattens <paramref name="message"/>: the value, whether the absence was deliberate,
    /// and the error.
    /// </summary>
    public static Ex093_Outcome Describe(Message<string> message) =>
        throw new NotImplementedException("TODO: Ex093 - flatten the composed message");
}
