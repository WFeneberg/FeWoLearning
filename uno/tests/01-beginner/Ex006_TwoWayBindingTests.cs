using FeWoLearning.Uno.Exercises.Beginner;
using FeWoLearning.Uno.Support;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex006_TwoWayBindingTests : UnoTestContext
{
    private static (Ex006_TwoWayBinding Editor, CaptionSource Source) Bound(string caption = "ready")
    {
        var source = new CaptionSource { Caption = caption };
        var editor = new Ex006_TwoWayBinding();
        editor.BindDraftTo(source);
        return (editor, source);
    }

    [Fact]
    public void Seeds_Itself_From_The_Source()
    {
        var (editor, _) = Bound();

        Assert.Equal("ready", editor.Draft);
    }

    [Fact]
    public void Still_Follows_The_Source()
    {
        var (editor, source) = Bound();

        source.Caption = "running";

        Assert.Equal("running", editor.Draft);
    }

    [Fact]
    public void Writes_Edits_Back_To_The_Source()
    {
        var (editor, source) = Bound();

        editor.Draft = "edited";

        // This is the half a OneWay binding does not do.
        Assert.Equal("edited", source.Caption);
    }

    [Fact]
    public void Survives_Changes_From_Both_Ends()
    {
        var (editor, source) = Bound();

        editor.Draft = "from the element";
        Assert.Equal("from the element", source.Caption);

        source.Caption = "from the source";
        Assert.Equal("from the source", editor.Draft);

        editor.Draft = "from the element again";
        Assert.Equal("from the element again", source.Caption);
    }

    [Fact]
    public void Does_Not_Echo_Its_Own_Write_Back_Forever()
    {
        var source = new CaptionSource { Caption = "ready" };
        var notifications = 0;
        source.PropertyChanged += (_, _) => notifications++;

        var editor = new Ex006_TwoWayBinding();
        editor.BindDraftTo(source);
        editor.Draft = "edited";

        // The source notifies, the binding pushes back the value already there, and the
        // equality check on both ends stops the ping-pong. One notification, not many.
        Assert.Equal(1, notifications);
    }
}
