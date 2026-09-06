namespace FeWoLearning.Architecture.Exercises.Runtime.Ex098;

public enum Verdict
{
    /// <summary>This message is broken. Retrying it will never work.</summary>
    Poison,

    /// <summary>Something around it is broken. This message deserves another chance.</summary>
    Transient,
}

public sealed record Attempt(string MessageId, string ExceptionType, int AttemptNumber);

// Exercise 098 — PoisonPillDetection (reference solution).
public sealed class FailureClassifier(int transientAttemptsBeforePoison)
{
    private readonly List<(string MessageId, bool Succeeded)> _recent = [];

    public void RecordOutcome(string messageId, bool succeeded) => _recent.Add((messageId, succeeded));

    public static bool IsShapeFailure(string exceptionType) =>
        exceptionType is "JsonException" or "FormatException" or "ValidationException";

    public Verdict Classify(Attempt attempt)
    {
        // The payload will not improve. Retrying it three times just delays the same
        // answer while holding a consumer slot.
        if (IsShapeFailure(attempt.ExceptionType))
            return Verdict.Poison;

        if (attempt.AttemptNumber < transientAttemptsBeforePoison)
            return Verdict.Transient;

        // The evidence that makes the hard case decidable. Nothing about ONE timeout says
        // whether the message or the world is at fault - the exception type is identical
        // either way. What separates them is whether anybody ELSE is getting through.
        var othersSucceeding = _recent.Any(o => o.MessageId != attempt.MessageId && o.Succeeded);

        // A queue where everything times out is a broken dependency, and dead-lettering
        // the whole queue turns a ten-minute outage into a day of manual replay.
        return othersSucceeding ? Verdict.Poison : Verdict.Transient;
    }
}
