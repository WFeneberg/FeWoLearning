using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex027_MatrixIndexerTests
{
    [Fact]
    public void Indexer_SetThenGet_ReturnsSameValue()
    {
        var matrix = new MatrixIndexer(3, 3);

        matrix[0, 0] = 1;
        matrix[1, 2] = 42;
        matrix[2, 1] = -7;

        Assert.Equal(1, matrix[0, 0]);
        Assert.Equal(42, matrix[1, 2]);
        Assert.Equal(-7, matrix[2, 1]);
        Assert.Equal(0, matrix[0, 1]);
    }

    [Fact]
    public void Indexer_OverwriteValue_ReflectsLatestSet()
    {
        var matrix = new MatrixIndexer(2, 2);

        matrix[1, 1] = 5;
        matrix[1, 1] = 9;

        Assert.Equal(9, matrix[1, 1]);
    }

    [Fact]
    public void Dimensions_MatchConstructorArguments()
    {
        var matrix = new MatrixIndexer(4, 6);

        Assert.Equal(4, matrix.Rows);
        Assert.Equal(6, matrix.Cols);
    }
}
