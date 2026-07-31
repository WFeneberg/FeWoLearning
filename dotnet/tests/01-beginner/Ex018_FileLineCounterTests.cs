using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex018_FileLineCounterTests
{
    [Fact]
    public void WriteAndCountNonEmptyLines_CountsOnlyNonEmptyLines()
    {
        var path = Path.Combine(Path.GetTempPath(), $"fewo-ex018-{Guid.NewGuid():N}.txt");
        try
        {
            var lines = new[] { "first", "", "second", "   ", "third", "\t" };

            var count = FileLineCounter.WriteAndCountNonEmptyLines(path, lines);

            Assert.Equal(3, count);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [Fact]
    public void WriteAndCountNonEmptyLines_AllLinesEmpty_ReturnsZero()
    {
        var path = Path.Combine(Path.GetTempPath(), $"fewo-ex018-{Guid.NewGuid():N}.txt");
        try
        {
            var lines = new[] { "", "   ", "\t", "" };

            var count = FileLineCounter.WriteAndCountNonEmptyLines(path, lines);

            Assert.Equal(0, count);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
