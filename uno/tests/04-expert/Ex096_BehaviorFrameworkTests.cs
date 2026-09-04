using FeWoLearning.Uno.Exercises.Expert;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex096_BehaviorFrameworkTests : UnoTestContext
{
    private sealed class RecordingBehavior : Ex096_Behavior
    {
        public List<string> Log { get; } = [];

        protected override void OnAttached() => Log.Add($"attached:{AssociatedObject?.Name}");

        protected override void OnDetaching() => Log.Add($"detaching:{AssociatedObject?.Name}");
    }

    private static Border Element(string name = "target") => new() { Name = name, Width = 10, Height = 10 };

    [Fact]
    public void Assigning_A_Collection_Attaches_Its_Behaviours()
    {
        var element = Element();
        var behavior = new RecordingBehavior();

        Ex096_BehaviorFramework.SetBehaviors(element, [behavior]);

        Assert.Same(element, behavior.AssociatedObject);
        Assert.Equal(1, behavior.Attachments);
    }

    [Fact]
    public void The_Behaviour_Sees_The_Element_While_Attaching()
    {
        var element = Element("card");
        var behavior = new RecordingBehavior();

        Ex096_BehaviorFramework.SetBehaviors(element, [behavior]);

        // Set before OnAttached runs, or a behaviour cannot subscribe to anything.
        Assert.Equal(["attached:card"], behavior.Log);
    }

    [Fact]
    public void Attaching_Twice_Does_Nothing_The_Second_Time()
    {
        var element = Element();
        var behavior = new RecordingBehavior();
        behavior.Attach(element);

        behavior.Attach(element);

        // A behaviour attached twice subscribes twice, and every event fires twice.
        Assert.Equal(1, behavior.Attachments);
    }

    [Fact]
    public void Detaching_Clears_The_Element()
    {
        var element = Element();
        var behavior = new RecordingBehavior();
        behavior.Attach(element);

        behavior.Detach();

        Assert.Null(behavior.AssociatedObject);
        Assert.Equal(1, behavior.Detachments);
    }

    [Fact]
    public void The_Behaviour_Still_Sees_The_Element_While_Detaching()
    {
        var element = Element("card");
        var behavior = new RecordingBehavior();
        behavior.Attach(element);
        behavior.Log.Clear();

        behavior.Detach();

        // Cleared after OnDetaching, because unsubscribing needs the element that was
        // subscribed to.
        Assert.Equal(["detaching:card"], behavior.Log);
    }

    [Fact]
    public void Detaching_Twice_Does_Nothing_The_Second_Time()
    {
        var element = Element();
        var behavior = new RecordingBehavior();
        behavior.Attach(element);

        behavior.Detach();
        behavior.Detach();

        Assert.Equal(1, behavior.Detachments);
    }

    [Fact]
    public void Replacing_The_Collection_Detaches_The_Old_Behaviours()
    {
        var element = Element();
        var first = new RecordingBehavior();
        Ex096_BehaviorFramework.SetBehaviors(element, [first]);

        Ex096_BehaviorFramework.SetBehaviors(element, [new RecordingBehavior()]);

        Assert.Equal(1, first.Detachments);
        Assert.Null(first.AssociatedObject);
    }

    [Fact]
    public void Replacing_The_Collection_Attaches_The_New_Behaviours()
    {
        var element = Element();
        Ex096_BehaviorFramework.SetBehaviors(element, [new RecordingBehavior()]);
        var second = new RecordingBehavior();

        Ex096_BehaviorFramework.SetBehaviors(element, [second]);

        Assert.Same(element, second.AssociatedObject);
    }

    [Fact]
    public void Clearing_The_Collection_Detaches_Everything()
    {
        var element = Element();
        var behavior = new RecordingBehavior();
        Ex096_BehaviorFramework.SetBehaviors(element, [behavior]);

        Ex096_BehaviorFramework.SetBehaviors(element, null);

        Assert.Equal(1, behavior.Detachments);
    }

    [Fact]
    public void Every_Behaviour_In_A_Collection_Is_Attached()
    {
        var element = Element();
        var first = new RecordingBehavior();
        var second = new RecordingBehavior();

        Ex096_BehaviorFramework.SetBehaviors(element, [first, second]);

        Assert.Equal(1, first.Attachments);
        Assert.Equal(1, second.Attachments);
    }

    [Fact]
    public void Attaching_To_Something_That_Is_Not_An_Element_Is_Ignored()
    {
        var behavior = new RecordingBehavior();

        Ex096_BehaviorFramework.SetBehaviors(new Microsoft.UI.Xaml.Media.SolidColorBrush(), [behavior]);

        // Markup can attach this anywhere, and a cast would take the app down at parse
        // time for a typo in a style.
        Assert.Equal(0, behavior.Attachments);
    }
}
