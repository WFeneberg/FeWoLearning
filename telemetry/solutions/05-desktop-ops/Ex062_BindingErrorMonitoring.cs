using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 062 — BindingErrorMonitoring (desktop-ops).
// Goal:   Find out about the failures WPF reports to nobody.
// Drills: PresentationTraceSources, a TraceListener, turning a trace message into a
//         structured record.
// Passes: a failing binding produces exactly ONE record carrying the engine's message as
//                     a field;
//         a binding that resolves produces none;
//         a second failing binding produces a second record - this is per failure, not
//                     once ever;
//         and disposing the capture stops it and puts the trace source back as it was.
//
// A WPF binding that cannot resolve its path does not throw, does not fail the build, and
// does not stop the application. It writes a line to the debug output and leaves the
// control empty - so the label is blank, the user reports "the name is missing", and
// nothing anywhere recorded that anything went wrong. On a developer's machine you see it
// in the Output window. In production nobody has an Output window.
//
// Turning it into a log record is the whole row, and it costs about fifteen lines. It is
// also the only telemetry in this track that finds bugs in code nobody instrumented -
// including XAML written years ago by somebody else.
//
// Four things about the plumbing that are not obvious, all measured on this machine
// (2026-09-06), and each of which silently produces zero records if you get it wrong:
//
//   - PresentationTraceSources.Refresh() must be called FIRST. It re-reads configuration,
//     so a listener added or a switch level set before it is simply undone.
//   - The switch level must be raised. It defaults to Off and nothing arrives.
//   - Override TraceEvent, NOT Write or WriteLine. The binding engine calls TraceEvent;
//     measured, Write was called zero times while TraceEvent got the whole message.
//   - No Window and no FrameworkElement are needed. A binding on a plain DependencyObject
//     reports exactly the same way.
public static class Ex062_BindingErrorMonitoring
{
    /// <summary>The category binding failures are written under.</summary>
    public const string CategoryName = "fewolearning.telemetry.ex062";

    /// <summary>The field carrying what the binding engine said.</summary>
    public const string ErrorField = "BindingError";

    /// <summary>The constant template every binding failure uses.</summary>
    public const string Template = "WPF binding failed: {BindingError}";

    /// <summary>
    /// Start turning WPF binding failures into Error records on
    /// <paramref name="logger"/>, using <see cref="Template"/> with the engine's own
    /// message as <see cref="ErrorField"/>.
    ///
    /// Disposing the returned handle stops the capture and leaves the trace source
    /// exactly as it was found - listeners and switch level both.
    /// </summary>
    public static IDisposable CaptureBindingErrors(ILogger logger) => new Capture(logger);

    private sealed class Capture : IDisposable
    {
        private readonly Listener _listener;
        private readonly SourceLevels _previousLevel;

        public Capture(ILogger logger)
        {
            // FIRST. Refresh re-reads configuration, so a listener added or a level set
            // before it is simply undone - and the result is silence with no error.
            PresentationTraceSources.Refresh();

            _listener = new Listener(logger);
            _previousLevel = PresentationTraceSources.DataBindingSource.Switch.Level;

            PresentationTraceSources.DataBindingSource.Listeners.Add(_listener);

            // Defaults to Off. Errors only: the source is chatty at higher levels and this
            // row is about failures.
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
        }

        public void Dispose()
        {
            // The trace source is process-global, so leaving a listener attached would
            // report into this logger for the life of the process.
            PresentationTraceSources.DataBindingSource.Listeners.Remove(_listener);
            PresentationTraceSources.DataBindingSource.Switch.Level = _previousLevel;
        }
    }

    /// <summary>
    /// Overrides TraceEvent, NOT Write. Measured: the binding engine calls TraceEvent and
    /// never Write, so a listener built on Write records absolutely nothing.
    /// </summary>
    private sealed class Listener(ILogger logger) : TraceListener
    {
        public override void Write(string? message)
        {
        }

        public override void WriteLine(string? message)
        {
        }

        public override void TraceEvent(
            TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? message)
        {
            if (eventType is TraceEventType.Error or TraceEventType.Critical)
            {
                logger.LogError(Template, message);
            }
        }

        public override void TraceEvent(
            TraceEventCache? eventCache,
            string source,
            TraceEventType eventType,
            int id,
            string? format,
            params object?[]? args) =>
            TraceEvent(
                eventCache,
                source,
                eventType,
                id,
                format is null ? null : string.Format(format, args ?? []));
    }
}
