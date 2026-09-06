using Serilog;
using Serilog.Events;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 013 — SerilogRollingFileAndOverrides (logging).
// Goal:   Write to a local file that can never fill the disk, and turn one noisy
//         component down without turning everything down.
// Drills: the rolling file sink, size limits, retention, MinimumLevel.Override.
// Passes: writing well past the size limit produces MORE than one file;
//         the directory never holds more than RetainedFileCountLimit of them;
//         a record from NoisySource at Information does NOT reach the file, while the
//                     same source at Warning does;
//         and a record from any other source at Information does.
//
// The first two clauses are the same guarantee from both ends, and shipping only one
// of them is the classic desktop-support disaster: a log that rolls but never deletes
// fills the user's disk over a year, and a log that deletes but never rolls throws
// away the one file you needed. Both are silent until they are not.
//
// The third and fourth are what an override is FOR. The tempting fix for a chatty
// component is to raise the global minimum level, which also silences everything you
// actually wanted. The override names one source and leaves the rest alone.
//
// Serilog's file sink buffers. The logger must be disposed before its file is read -
// which is also the honest lesson: the last records only exist once you close it.
public static class Ex013_SerilogRollingFileAndOverrides
{
    /// <summary>Roll to a new file once the current one passes this size.</summary>
    public const long FileSizeLimitBytes = 2048;

    /// <summary>Never keep more files than this, counting the current one.</summary>
    public const int RetainedFileCountLimit = 3;

    /// <summary>The source context that is turned down to Warning.</summary>
    public const string NoisySource = "Noisy.Component";

    /// <summary>
    /// Build a Serilog logger writing to <paramref name="logFilePath"/>, which:
    ///
    ///   - rolls to a new file when the current one passes
    ///     <see cref="FileSizeLimitBytes"/>;
    ///   - keeps at most <see cref="RetainedFileCountLimit"/> files;
    ///   - writes Information and above by default;
    ///   - writes only Warning and above for the source context
    ///     <see cref="NoisySource"/>.
    ///
    /// The caller disposes the returned logger, and must, before reading the files.
    /// </summary>
    public static Serilog.Core.Logger Create(string logFilePath) =>
        new LoggerConfiguration()
            .MinimumLevel.Information()
            // Names ONE source and leaves every other one alone. Raising the global
            // minimum level instead would also silence the components you still want.
            .MinimumLevel.Override(NoisySource, LogEventLevel.Warning)
            .WriteTo.File(
                logFilePath,
                fileSizeLimitBytes: FileSizeLimitBytes,
                // Without this the size limit is a ceiling that simply STOPS the log
                // rather than rolling it - a quiet way to lose everything after
                // Tuesday.
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: RetainedFileCountLimit)
            .CreateLogger();
}
