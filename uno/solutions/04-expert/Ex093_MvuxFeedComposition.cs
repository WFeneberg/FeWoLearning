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
        // Select maps the data axis only. A message with no data - an error, or a
        // deliberate None - passes through without the projection running, which is why no
        // downstream code has to check for a value that is not there.
        source.Select(value =>
        {
            Projections++;
            return $"#{value}";
        });

    /// <summary>
    /// A feed carrying only the values of <paramref name="source"/> that are at least
    /// <paramref name="minimum"/>.
    /// </summary>
    // An excluded value becomes None - "there deliberately is no value" - not Undefined
    // and not an error.
    public static IFeed<int> AtLeast(IFeed<int> source, int minimum) =>
        source.Where(value => value >= minimum);

    /// <summary>
    /// A feed of <c>"name=value"</c> from the two sources, one message per pair.
    /// </summary>
    public static IFeed<string> Describe(IFeed<string> name, IFeed<int> value) =>
        // One message per pair, and either failure fails the pair - the projection below
        // therefore only ever sees two real values.
        Feed.Combine(name, value).Select(pair => $"{pair.Item1}={pair.Item2}");

    /// <summary>
    /// Flattens <paramref name="message"/>: the value, whether the absence was deliberate,
    /// and the error.
    /// </summary>
    public static Ex093_Outcome Describe(Message<string> message)
    {
        var entry = message.Current;

        return new Ex093_Outcome(
            Value: entry.Data.IsSome(out var value) ? value : "",
            HasValue: entry.Data.IsSome(out _),
            IsEmpty: entry.Data.Type == OptionType.None,
            Error: entry.Error?.Message);
    }
}
