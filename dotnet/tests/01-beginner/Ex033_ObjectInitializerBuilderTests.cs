using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex033_ObjectInitializerBuilderTests
{
    [Fact]
    public void BuildTeam_ReturnsThreePeople()
    {
        var team = ObjectInitializerBuilder.BuildTeam();

        Assert.Equal(3, team.Count);
    }

    [Fact]
    public void BuildTeam_ReturnsExpectedPeopleInOrder()
    {
        var team = ObjectInitializerBuilder.BuildTeam();

        Assert.Equal("Alice", team[0].Name);
        Assert.Equal(30, team[0].Age);

        Assert.Equal("Bob", team[1].Name);
        Assert.Equal(25, team[1].Age);

        Assert.Equal("Charlie", team[2].Name);
        Assert.Equal(35, team[2].Age);
    }
}
