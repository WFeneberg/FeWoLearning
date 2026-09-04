using FeWoLearning.Uno.Exercises.Intermediate;
using FeWoLearning.Uno.Support;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex052_BindingFallbacksTests : UnoTestContext
{
    private static string Text(object source, string path) =>
        Ex052_BindingFallbacks.CreateLabel(source, path, fallback: "no binding", whenNull: "no value").Text;

    [Fact]
    public void A_Resolving_Binding_Shows_The_Value()
    {
        Assert.Equal("hello", Text(new CaptionSource { Caption = "hello" }, "Caption"));
    }

    [Fact]
    public void A_Path_That_Does_Not_Exist_Shows_The_Fallback()
    {
        // The typo case. Without FallbackValue the label is simply empty, which reads as a
        // data problem and sends people looking in the wrong place.
        Assert.Equal("no binding", Text(new CaptionSource(), "Kaption"));
    }

    [Fact]
    public void A_Null_Value_Shows_The_Null_Substitute()
    {
        Assert.Equal("no value", Text(new CaptionSource { Caption = null! }, "Caption"));
    }

    [Fact]
    public void The_Two_Failures_Are_Different()
    {
        var missing = Text(new CaptionSource(), "Kaption");
        var isNull = Text(new CaptionSource { Caption = null! }, "Caption");

        // TargetNullValue does not cover a broken path: nothing ever produced a null to
        // substitute, so only FallbackValue applies.
        Assert.NotEqual(missing, isNull);
    }

    [Fact]
    public void A_Value_Arriving_Later_Replaces_The_Null_Substitute()
    {
        var source = new CaptionSource { Caption = null! };
        var label = Ex052_BindingFallbacks.CreateLabel(source, "Caption", "no binding", "no value");

        source.Caption = "arrived";

        Assert.Equal("arrived", label.Text);
    }

    [Fact]
    public void A_Value_Going_Null_Returns_To_The_Substitute()
    {
        var source = new CaptionSource { Caption = "hello" };
        var label = Ex052_BindingFallbacks.CreateLabel(source, "Caption", "no binding", "no value");

        source.Caption = null!;

        Assert.Equal("no value", label.Text);
    }

    [Fact]
    public void An_Empty_String_Is_A_Value_Not_A_Null()
    {
        Assert.Equal("", Text(new CaptionSource { Caption = "" }, "Caption"));
    }

    [Fact]
    public void The_Label_Lays_Out_With_Its_Fallback()
    {
        var label = Layout(Ex052_BindingFallbacks.CreateLabel(new CaptionSource(), "Kaption", "no binding", "no value"));

        Assert.True(label.DesiredSize.Width > 0);
    }
}
