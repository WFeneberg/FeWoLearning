namespace FeWoLearning.Architecture.Exercises.Runtime.Ex098;

public enum Verdict
{
    /// <summary>This message is broken. Retrying it will never work.</summary>
    Poison,

    /// <summary>Something around it is broken. This message deserves another chance.</summary>
    Transient,
}

public sealed record Attempt(string MessageId, string ExceptionType, int AttemptNumber);

// Exercise 098 — PoisonPillDetection (runtime).
// Goal:   Tell a message that will never work apart from a message that arrived during a
//         bad ten minutes.
// Drills: classifying failures, per-message vs system-wide evidence, avoiding both mistakes.
// Passes: shape     - a deserialisation or validation failure is Poison on the FIRST
//                     attempt. The payload will not improve; retrying it three times just
//                     delays the same answer.
//         infra     - a timeout or a connection failure is Transient. Those recover.
//         THE ONE    - a message failing repeatedly with a TRANSIENT error is still
//                      Transient while OTHER messages are failing too, and becomes Poison
//                      only once it is failing ALONE. A queue where everything times out
//                      is a broken dependency, and dead-lettering the whole queue turns a
//                      ten-minute outage into a day of manual replay.
//         alone     - the same message failing while its neighbours succeed is Poison,
//                      whatever the exception says.
//         evidence  - the classification uses the recent window, not the whole history.
//
// This sits between exercises 047 and 048 and decides which of them should run. A poison
// message routed to retry burns the whole retry budget and lands in the dead-letter queue
// anyway, minutes later; a transient failure routed to the dead-letter queue takes a
// perfectly good message out of the system because a database was restarting.
//
// The system-wide evidence is what makes the hard case decidable. Nothing about ONE
// timeout says whether the message or the world is at fault - the exception type is the
// same either way. What separates them is whether anybody else is succeeding.
public sealed class FailureClassifier(int transientAttemptsBeforePoison)
{
    private readonly List<(string MessageId, bool Succeeded)> _recent = [];

    /// <summary>Record an outcome, so the classifier knows what else is happening.</summary>
    public void RecordOutcome(string messageId, bool succeeded) => _recent.Add((messageId, succeeded));

    /// <summary>Exception types that mean the payload itself is wrong.</summary>
    public static bool IsShapeFailure(string exceptionType) =>
        exceptionType is "JsonException" or "FormatException" or "ValidationException";

    public Verdict Classify(Attempt attempt) =>
        throw new NotImplementedException(
            "TODO: Ex098 - a shape failure is Poison at once; a transient one is Poison only after enough attempts AND only if other messages are succeeding");
}
