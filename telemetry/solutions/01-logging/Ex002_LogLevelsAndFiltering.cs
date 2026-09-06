using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 002 — LogLevelsAndFiltering (logging).
// Goal:   Decide what gets written with FILTER RULES, not with `if` statements at the
//         call site.
// Drills: AddFilter by category and level, the default minimum level, IsEnabled.
// Passes: EmitOnePerLevel always calls the logger once at each of the six levels -
//                     with a wide-open factory, six records arrive;
//         Configure leaves the category "Quiet" at Warning, so only Warning, Error
//                     and Critical survive there;
//         Configure lowers the category "Chatty" to Debug, so five survive there -
//                     Debug, Information, Warning, Error and Critical, but not Trace;
//         and logger.IsEnabled agrees with what actually arrives, for both categories.
//
// The last clause is the point. IsEnabled is what an expensive call site consults
// before building its arguments; if it disagrees with the filter that ultimately
// drops the record, you either pay to construct messages nobody keeps, or you skip
// records the operator asked for. Both are silent.
public static class Ex002_LogLevelsAndFiltering
{
    /// <summary>The category left at the default minimum level.</summary>
    public const string QuietCategory = "Quiet";

    /// <summary>The category that is deliberately lowered.</summary>
    public const string ChattyCategory = "Chatty";

    /// <summary>
    /// Add filter rules to <paramref name="builder"/> so that:
    ///
    ///   - the default minimum level for every category is Warning;
    ///   - the category <see cref="ChattyCategory"/> is lowered to Debug.
    ///
    /// Use filter rules. Do NOT change what <see cref="EmitOnePerLevel"/> writes.
    /// </summary>
    public static void Configure(ILoggingBuilder builder)
    {
        // SetMinimumLevel is the floor for every category that no rule matches.
        builder.SetMinimumLevel(LogLevel.Warning);

        // AddFilter installs a rule for one category prefix. It goes through the same
        // pipeline IsEnabled consults, which is why the call site and the sink agree.
        builder.AddFilter(ChattyCategory, LogLevel.Debug);
    }

    /// <summary>
    /// Write exactly one record at each of the six levels - Trace, Debug, Information,
    /// Warning, Error, Critical - with the message "level probe" and no arguments.
    ///
    /// Unconditionally. This method must not consult IsEnabled or the level itself:
    /// deciding here is the mistake the exercise is about.
    /// </summary>
    public static void EmitOnePerLevel(ILogger logger)
    {
        // No guards, no branching on the level. What survives is the filter's job, and
        // keeping that decision in one configurable place is the whole point.
        logger.LogTrace("level probe");
        logger.LogDebug("level probe");
        logger.LogInformation("level probe");
        logger.LogWarning("level probe");
        logger.LogError("level probe");
        logger.LogCritical("level probe");
    }
}
