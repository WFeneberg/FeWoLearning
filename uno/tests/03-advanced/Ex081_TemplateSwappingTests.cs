using FeWoLearning.Uno.Exercises.Advanced;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex081_TemplateSwappingTests : UnoTestContext
{
    private static Ex081_TemplateSwapping Control(bool highlighted = false) =>
        Layout(new Ex081_TemplateSwapping
        {
            Template = Ex081_TemplateSwapping.FirstTemplate,
            IsHighlighted = highlighted,
        });

    private static void SwapTo(Ex081_TemplateSwapping control, Microsoft.UI.Xaml.Controls.ControlTemplate template)
    {
        control.Template = template;
        Layout(control);
    }

    [Fact]
    public void The_First_Template_Is_Picked_Up()
    {
        var control = Control();

        Assert.NotNull(control.Fill);
        Assert.Equal(1, control.TemplatesApplied);
    }

    [Fact]
    public void A_Swap_Applies_The_Second_Template()
    {
        var control = Control();

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);

        Assert.Equal(2, control.TemplatesApplied);
    }

    [Fact]
    public void A_Swap_Produces_A_New_Part()
    {
        var control = Control();
        var firstPart = control.Fill;

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);

        Assert.NotSame(firstPart, control.Fill);
    }

    [Fact]
    public void The_Old_Part_Is_Released()
    {
        var control = Control();

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);

        Assert.Equal(1, control.PartsReleased);
    }

    [Fact]
    public void The_Control_Stops_Writing_To_The_Old_Part()
    {
        var control = Control();
        var firstPart = control.Fill!;

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);
        control.DimCurrentPart();

        // The old part is detached but still reachable from anything that captured it. A
        // control still writing to it is leaking the old tree and showing nothing.
        Assert.Equal(1, firstPart.Opacity, 2);
        Assert.Equal(0.5, control.Fill!.Opacity, 2);
    }

    [Fact]
    public void Releasing_Happens_Before_The_New_Lookup()
    {
        var control = Control();
        var firstPart = control.Fill!;
        control.DimCurrentPart();

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);

        // Restored on the way out. Releasing after the lookup would reset the *new* part
        // instead - and leave the old one dimmed for ever.
        Assert.Equal(1, firstPart.Opacity, 2);
        Assert.Equal(1, control.Fill!.Opacity, 2);
    }

    [Fact]
    public void A_Highlighted_Control_Comes_Back_Highlighted()
    {
        var control = Control(highlighted: true);

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);

        // The state groups belong to the template and start empty, so the control has to
        // say what it is again - otherwise a theme switch quietly resets every state.
        Assert.Equal(60, control.Fill!.Width, 1);
    }

    [Fact]
    public void An_Unhighlighted_Control_Comes_Back_Normal()
    {
        var control = Control();

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);

        Assert.Equal(20, control.Fill!.Width, 1);
    }

    [Fact]
    public void Swapping_Back_And_Forth_Keeps_The_Books_Straight()
    {
        var control = Control(highlighted: true);

        SwapTo(control, Ex081_TemplateSwapping.SecondTemplate);
        SwapTo(control, Ex081_TemplateSwapping.FirstTemplate);

        Assert.Equal(3, control.TemplatesApplied);
        Assert.Equal(2, control.PartsReleased);
        Assert.Equal(60, control.Fill!.Width, 1);
    }
}
