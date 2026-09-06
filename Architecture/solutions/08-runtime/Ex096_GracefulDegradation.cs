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

// Exercise 096 — GracefulDegradation (reference solution).
public static class Ex096_GracefulDegradation
{
    public static Answer Resolve(IReadOnlyList<Source> sources)
    {
        ArgumentNullException.ThrowIfNull(sources);

        var tried = new List<string>();

        foreach (var source in sources)
        {
            // Recorded BEFORE the attempt, so a source that threw still appears in the
            // list. "We degraded" is only actionable with "and here is what broke".
            tried.Add(source.Name);

            try
            {
                // The quality travels on the ANSWER, not in a log line. A log line is
                // something somebody has to think to look for; a field on the response is
                // something the caller can put on a dashboard, alert on, and count.
                return new Answer(source.Produce(), source.Quality, source.Name, tried);
            }
            catch
            {
                // Swallowed on purpose - this source's failure is the next one's cue, and
                // it is reported through `tried` rather than by propagating.
            }
        }

        // Named, not silent. An empty result is indistinguishable from "there is nothing",
        // which is a different answer with different consequences.
        throw new NoFallbackLeftException(tried);
    }
}
