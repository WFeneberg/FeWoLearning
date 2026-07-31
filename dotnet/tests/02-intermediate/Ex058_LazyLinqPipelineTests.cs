using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex058_LazyLinqPipelineTests
{
    [Fact]
    public void BuildDoublingQuery_DoesNotProjectBeforeEnumeration()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var projectionCount = 0;

        var query = LazyLinqPipeline.BuildDoublingQuery(source, _ => projectionCount++);

        Assert.Equal(0, projectionCount);
    }

    [Fact]
    public void BuildDoublingQuery_ProjectsOncePerElementPerEnumeration()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var projectionCount = 0;

        var query = LazyLinqPipeline.BuildDoublingQuery(source, _ => projectionCount++);

        Assert.Equal(0, projectionCount);

        var firstPass = query.ToList();
        Assert.Equal(new List<int> { 2, 4, 6, 8, 10 }, firstPass);
        Assert.Equal(5, projectionCount);

        var secondPass = query.ToList();
        Assert.Equal(new List<int> { 2, 4, 6, 8, 10 }, secondPass);
        Assert.Equal(10, projectionCount);
    }

    [Fact]
    public void BuildDoublingQuery_EmptySource_NeverProjects()
    {
        var projectionCount = 0;

        var query = LazyLinqPipeline.BuildDoublingQuery(Array.Empty<int>(), _ => projectionCount++);
        var result = query.ToList();

        Assert.Empty(result);
        Assert.Equal(0, projectionCount);
    }
}
