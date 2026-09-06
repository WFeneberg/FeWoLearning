namespace FeWoLearning.Architecture.Exercises.Runtime.Ex096;

/// <summary>
/// How good the answer is. The caller is told, because "personalised" and "generic" are
/// different products even when both are a list of recommendations.
/// </summary>
public enum Quality
{
    Full,
    Degraded,
    Minimal,
}

public sealed record Answer(string Value, Quality Quality, string Source, IReadOnlyList<string> Tried);

/// <summary>One way of producing an answer, in descending order of goodness.</summary>
public sealed record Source(string Name, Quality Quality, Func<string> Produce);

public sealed class NoFallbackLeftException(IReadOnlyList<string> tried)
    : Exception($"Every source failed: {string.Join(", ", tried)}.")
{
    public IReadOnlyList<string> Tried { get; } = tried;
}

// Exercise 096 — GracefulDegradation (runtime).
// Goal:   Answer with something worse rather than with nothing, and be honest about which
//         one happened.
// Drills: fallback chains, declared quality levels, reporting the degradation.
// Passes: best      - the first source that works answers, and the ones after it are NOT
//                     tried.
//         falling   - when it fails, the next one is tried, and so on down the chain.
//         THE ONE    - the answer CARRIES its quality and its source. A fallback nobody can
//                     see is a system that silently got worse, and the graph that would
//                     have shown it is the one nobody built because everything looked
//                     fine.
//         attempts  - every source tried is listed, in order, including the failures - so
//                     "we degraded" comes with "and here is what broke".
//         exhausted - when everything fails, NoFallbackLeftException names all of them.
//                     A silent empty result is indistinguishable from "there is nothing",
//                     which is a different answer with different consequences.
//
// The dangerous thing about a fallback chain is that it works. Personalised
// recommendations fail, the generic ones go out, conversion drops four percent, and
// nothing anywhere is red - because from the outside the system is serving 200s with
// plausible content. Six weeks later somebody notices the revenue.
//
// That is why the quality is part of the ANSWER rather than a log line. A log line is
// something you have to think to look for; a field on the response is something the caller
// can put on a dashboard, alert on, and count.
public static class Ex096_GracefulDegradation
{
    /// <summary>Try each source in order until one produces an answer.</summary>
    public static Answer Resolve(IReadOnlyList<Source> sources) =>
        throw new NotImplementedException(
            "TODO: Ex096 - try the sources in order, stop at the first that works, report its quality, source and everything tried, and throw NoFallbackLeftException when none do");
}
