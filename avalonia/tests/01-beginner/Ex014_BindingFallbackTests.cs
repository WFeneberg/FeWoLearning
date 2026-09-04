using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex014_BindingFallbackTests
{
    private static (Ex014_BindingFallback View, Ex014_BindingFallbackViewModel Vm) Arrange()
    {
        var vm = new Ex014_BindingFallbackViewModel(); // Inner and NullableLabel both start null.
        var view = ViewHarness.Show(new Ex014_BindingFallback { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    // FallbackValue fires only when the path cannot be resolved at all (Inner is
    // null, so Inner.Label has nothing to read from) - not merely when the
    // resolved value happens to be null. A learner who puts TargetNullValue on
    // this binding instead gets an empty string here, not "(fallback)".
    [AvaloniaFact]
    public void FallbackValue_Shows_When_The_Path_Cannot_Resolve()
    {
        var (view, vm) = Arrange();
        var inner = view.FindControl<TextBlock>("InnerText")!;

        Assert.Equal("(fallback)", inner.Text);

        vm.Inner = new Ex014_InnerViewModel { Label = "resolved-label" };
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("resolved-label", inner.Text);
    }

    // TargetNullValue fires only when the path resolves but the value itself is
    // null - not when the path fails to resolve. A learner who puts
    // FallbackValue on this binding instead gets an empty string here, not
    // "(none)": FallbackValue does not catch a resolved-but-null value either.
    [AvaloniaFact]
    public void TargetNullValue_Shows_When_The_Resolved_Value_Is_Null()
    {
        var (view, vm) = Arrange();
        var nullable = view.FindControl<TextBlock>("NullableText")!;

        Assert.Equal("(none)", nullable.Text);

        vm.NullableLabel = "actual-value";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("actual-value", nullable.Text);
    }

    // The two attributes do not overlap: a resolved intermediate whose own leaf
    // property is null is the TargetNullValue case, not the FallbackValue case -
    // Inner.Label's binding has no TargetNullValue, so this renders empty.
    [AvaloniaFact]
    public void FallbackValue_Does_Not_Fire_For_A_Resolved_Path_With_A_Null_Leaf()
    {
        var (view, vm) = Arrange();
        var inner = view.FindControl<TextBlock>("InnerText")!;

        vm.Inner = new Ex014_InnerViewModel { Label = null };
        Dispatcher.UIThread.RunJobs();

        Assert.Null(inner.Text);
    }
}
