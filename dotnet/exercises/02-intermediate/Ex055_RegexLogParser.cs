using System.Text.RegularExpressions;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 055 — Regex Log Parser (intermediate).
// Goal:   Parse a log line of the form "2024-03-12 14:22:05 [INFO] Server started"
//         into its date, level, and message parts using a regex with NAMED capture
//         groups, returning null when the line does not match the expected shape.
// Drills: regular expressions, named capture groups, nullable records/tuples.
public static class RegexLogParser
{
    public record LogEntry(string Date, string Level, string Message);

    public static LogEntry? Parse(string line) => throw new NotImplementedException();
}
