using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 007 — ExceptionLogging (logging).
// Goal:   Hand the exception to the logger as an EXCEPTION, not as text inside the
//         message.
// Drills: the exception parameter, keeping the message a constant template, the inner
//         exception chain.
// Passes: the record's Exception is the very instance that was passed in, and the
//                     level is Error;
//         the rendered message reads exactly "Import of orders.csv failed" - no stack
//                     trace, no exception text;
//         the record carries File as a named field and the template contains "{File}";
//         and when the failure is a wrapper around a real cause, the record's
//                     exception still has that cause as its InnerException.
//
// The last clause is where this goes wrong in real code. `ex.Message` reads fine in a
// console and throws away everything that matters: the type, the stack, and above all
// the inner exception - which is almost always the actual cause. "One or more errors
// occurred." is what an AggregateException's Message says, and it is the least useful
// sentence in .NET.
//
// Interpolating the exception into the message is worse still: every failure produces
// a different template, so the backend sees thousands of unrelated event types instead
// of one event with thousands of instances, and grouping and alerting stop working.
public static class Ex007_ExceptionLogging
{
    /// <summary>
    /// Write ONE Error record saying that importing <paramref name="file"/> failed,
    /// carrying <paramref name="error"/> as the record's exception.
    ///
    /// The message template is "Import of {File} failed" and nothing more: the
    /// exception's own text must not appear in it.
    /// </summary>
    public static void LogImportFailure(ILogger logger, string file, Exception error) =>
        throw new NotImplementedException(
            "TODO: Ex007 - pass the exception as the exception argument, and keep it out of the message");
}
