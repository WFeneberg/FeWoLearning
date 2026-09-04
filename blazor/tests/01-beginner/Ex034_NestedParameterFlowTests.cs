using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex034_NestedParameterFlowTests : BunitContext
{
    [Fact]
    public void Message_Flows_Through_All_Three_Levels_To_The_Leaf()
    {
        var cut = Render<Ex034_NestedParameterFlow>(p => p.Add(c => c.Message, "deep"));

        Assert.Equal("deep", cut.Find("#leaf").TextContent);
        Assert.Equal("SPAN", cut.Find(".level-1 .level-2 #leaf").TagName);
    }

    [Fact]
    public void Changing_Message_After_Render_Updates_The_Leaf()
    {
        var cut = Render<Ex034_NestedParameterFlow>(p => p.Add(c => c.Message, "deep"));

        cut.Render(p => p.Add(c => c.Message, "deeper"));

        Assert.Equal("deeper", cut.Find("#leaf").TextContent);
    }
}
