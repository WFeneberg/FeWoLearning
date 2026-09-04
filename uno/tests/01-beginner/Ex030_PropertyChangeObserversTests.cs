using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex030_PropertyChangeObserversTests : UnoTestContext
{
    [Fact]
    public void Records_Nothing_Until_Something_Changes()
    {
        var border = new Border { Width = 10 };

        using var observer = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        // The current value is not a change - a watcher that reports one on subscribe makes
        // every caller special-case its first callback.
        Assert.Empty(observer.Values);
    }

    [Fact]
    public void Records_Each_New_Value()
    {
        var border = new Border();
        using var observer = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        border.Width = 10;
        border.Width = 20;

        Assert.Equal([10d, 20d], observer.Values);
    }

    [Fact]
    public void Watches_A_Property_It_Does_Not_Own()
    {
        var border = new Border();
        using var observer = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        border.Width = 10;

        // Nobody registered Width, and nobody can change its metadata - this is the only
        // way to hear about it from the outside.
        Assert.Equal([10d], observer.Values);
    }

    [Fact]
    public void Sees_Writes_Through_SetValue_Too()
    {
        var border = new Border();
        using var observer = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        border.SetValue(FrameworkElement.WidthProperty, 33d);

        Assert.Equal([33d], observer.Values);
    }

    [Fact]
    public void Stops_Recording_After_Dispose()
    {
        var border = new Border();
        var observer = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        border.Width = 10;
        observer.Dispose();
        border.Width = 20;

        Assert.Equal([10d], observer.Values);
    }

    [Fact]
    public void Disposing_Twice_Is_Harmless()
    {
        var border = new Border();
        var observer = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        observer.Dispose();
        observer.Dispose();

        Assert.Empty(observer.Values);
    }

    [Fact]
    public void Two_Observers_On_One_Property_Both_Hear_It()
    {
        var border = new Border();
        using var first = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);
        using var second = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        border.Width = 5;

        // Unlike the metadata callback, which exists once per property, there is no limit
        // here - and each token unsubscribes only its own.
        Assert.Equal([5d], first.Values);
        Assert.Equal([5d], second.Values);
    }

    [Fact]
    public void Disposing_One_Leaves_The_Other_Watching()
    {
        var border = new Border();
        var first = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);
        using var second = new Ex030_PropertyChangeObservers(border, FrameworkElement.WidthProperty);

        first.Dispose();
        border.Width = 5;

        Assert.Empty(first.Values);
        Assert.Equal([5d], second.Values);
    }

    [Fact]
    public void Watches_Any_Dependency_Property_On_Any_Object()
    {
        var text = new TextBlock();
        using var observer = new Ex030_PropertyChangeObservers(text, TextBlock.TextProperty);

        text.Text = "hello";

        Assert.Equal(["hello"], observer.Values);
    }
}
