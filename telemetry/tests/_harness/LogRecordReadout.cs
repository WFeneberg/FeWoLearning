using OpenTelemetry.Logs;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Reads the parts of an OpenTelemetry <see cref="LogRecord"/> that rows 036 and 037
/// grade, through the awkward corners of its API.
///
/// Unlike <see cref="MetricReadout"/> this is a convenience rather than a necessity:
/// measured 2026-09-06, <see cref="LogRecord"/> objects are NOT reused between exports
/// (two records exported from one factory are two distinct instances), so reading them
/// later is safe. The metric side is the exception, not the rule.
/// </summary>
public static class LogRecordReadout
{
    /// <summary>
    /// The value of one attribute, or null when the record carries no such key.
    ///
    /// Measured: the structured fields AND the <c>{OriginalFormat}</c> entry arrive here
    /// with no opt-in at all - <c>ParseStateValues</c> is not needed for a normal message
    /// template.
    /// </summary>
    public static string? Attribute(LogRecord record, string key) =>
        record.Attributes?.FirstOrDefault(a => a.Key == key).Value?.ToString();

    /// <summary>The raw value of one attribute, for facts that care about its TYPE.</summary>
    public static object? RawAttribute(LogRecord record, string key) =>
        record.Attributes?.FirstOrDefault(a => a.Key == key).Value;

    /// <summary>Every attribute key on the record, in order.</summary>
    public static IReadOnlyList<string> AttributeKeys(LogRecord record) =>
        record.Attributes?.Select(a => a.Key).ToArray() ?? [];

    /// <summary>
    /// Every scope value on the record, flattened to "key=value", outermost first.
    ///
    /// Measured: this is EMPTY unless the pipeline was built with
    /// <c>IncludeScopes = true</c> - which is the opposite of FakeLogger's behaviour in
    /// block 01, where scopes are captured unconditionally. Two different libraries, two
    /// different defaults, and only one of them tells you.
    /// </summary>
    public static IReadOnlyList<string> Scopes(LogRecord record)
    {
        var values = new List<string>();

        record.ForEachScope(
            (scope, into) =>
            {
                foreach (KeyValuePair<string, object?> pair in scope) into.Add($"{pair.Key}={pair.Value}");
            },
            values);

        return values;
    }
}
