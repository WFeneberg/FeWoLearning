using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex055_RegexLogParserTests
{
    [Theory]
    [InlineData(
        "2024-03-12 14:22:05 [INFO] Server started",
        "2024-03-12 14:22:05", "INFO", "Server started")]
    [InlineData(
        "2023-11-01 09:00:00 [ERROR] Connection refused by host",
        "2023-11-01 09:00:00", "ERROR", "Connection refused by host")]
    [InlineData(
        "2025-01-30 23:59:59 [WARN] Disk usage above threshold",
        "2025-01-30 23:59:59", "WARN", "Disk usage above threshold")]
    public void Parse_ExtractsDateLevelAndMessage(string line, string expectedDate, string expectedLevel, string expectedMessage)
    {
        var entry = RegexLogParser.Parse(line);

        Assert.NotNull(entry);
        Assert.Equal(expectedDate, entry!.Date);
        Assert.Equal(expectedLevel, entry.Level);
        Assert.Equal(expectedMessage, entry.Message);
    }

    [Fact]
    public void Parse_ReturnsNull_ForMalformedLine()
    {
        var entry = RegexLogParser.Parse("this is not a log line");

        Assert.Null(entry);
    }
}
