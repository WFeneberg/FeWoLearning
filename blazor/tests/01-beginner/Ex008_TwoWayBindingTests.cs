using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex008_TwoWayBindingTests : BunitContext
{
    [Fact]
    public void Renders_The_Value_In_The_Input_And_The_Echo()
    {
        var cut = Render<Ex008_TwoWayBinding>(p => p.Add(c => c.Value, "Ada"));

        Assert.Equal("Ada", cut.Find("#name").GetAttribute("value"));
        Assert.Equal("Ada", cut.Find("#echo").TextContent);
    }

    [Fact]
    public void Bound_Change_Flows_Back_To_The_Parents_Local_And_Rerenders_The_Echo()
    {
        var current = "Ada";
        var cut = Render<Ex008_TwoWayBinding>(p => p.Bind(c => c.Value, current, v => current = v));

        cut.Find("#name").Change("Grace");
        Assert.Equal("Grace", current);

        // Bind() only wires ValueChanged to update the captured local - as in a real
        // Blazor app, it is the parent's own next render that feeds the updated value
        // back down as a parameter. Simulate that render here.
        cut.Render(p => p.Add(c => c.Value, current));

        Assert.Equal("Grace", cut.Find("#echo").TextContent);
    }

    [Fact]
    public void Without_A_ValueChanged_Handler_The_Component_Never_Writes_To_Its_Own_Value()
    {
        var cut = Render<Ex008_TwoWayBinding>(p => p.Add(c => c.Value, "Ada"));

        cut.Find("#name").Change("Grace");

        Assert.Equal("Ada", cut.Find("#echo").TextContent);
    }
}
