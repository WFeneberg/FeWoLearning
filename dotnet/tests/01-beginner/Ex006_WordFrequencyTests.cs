using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex006_WordFrequencyTests
{
    [Fact]
    public void Count_CountsRepeatedWords_CaseInsensitively()
    {
        var result = WordFrequency.Count("The cat sat on the mat. The Cat was happy.");

        Assert.Equal(3, result["the"]);
        Assert.Equal(2, result["cat"]);
        Assert.Equal(1, result["sat"]);
        Assert.Equal(1, result["on"]);
        Assert.Equal(1, result["mat"]);
        Assert.Equal(1, result["was"]);
        Assert.Equal(1, result["happy"]);
        Assert.Equal(7, result.Count);
    }
}
