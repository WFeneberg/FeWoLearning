using System.Text.RegularExpressions;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 055 — Regex Log Parser (reference solution).
public static class RegexLogParser
{
    public record LogEntry(string Date, string Level, string Message);

    private static readonly Regex LogPattern = new(
        @"^(?<date>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) \[(?<level>[A-Z]+)\] (?<message>.+)$",
        RegexOptions.Compiled);

    public static LogEntry? Parse(string line)
    {
        var match = LogPattern.Match(line);
        if (!match.Success)
        {
            return null;
        }

        return new LogEntry(
            match.Groups["date"].Value,
            match.Groups["level"].Value,
            match.Groups["message"].Value);
    }
}
