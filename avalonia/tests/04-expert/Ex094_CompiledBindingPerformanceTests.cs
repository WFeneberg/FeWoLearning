using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Expert;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex094_CompiledBindingPerformanceTests
{
    private static Ex094_CompiledBindingPerformance Shown()
    {
        var view = ViewHarness.Show(new Ex094_CompiledBindingPerformance(), 300, 200);
        Dispatcher.UIThread.RunJobs();
        return view;
    }

    private static string? Text(Ex094_CompiledBindingPerformance view, string name) =>
        view.FindControl<TextBlock>(name)?.Text;

    private static Ex094_ReportViewModel Vm(Ex094_CompiledBindingPerformance view) =>
        (Ex094_ReportViewModel)view.DataContext!;

    // When the path is right the two are indistinguishable, which is worth
    // establishing first: the case for compiled bindings is not that they read a
    // different value.
    [AvaloniaFact]
    public void Both_Kinds_Read_The_Same_Value()
    {
        var view = Shown();

        Assert.Equal("real", Text(view, "Compiled"));
        Assert.Equal("real", Text(view, "Reflection"));
    }

    [AvaloniaFact]
    public void Both_Kinds_Stay_Live()
    {
        var view = Shown();

        Vm(view).Title = "changed";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("changed", Text(view, "Compiled"));
        Assert.Equal("changed", Text(view, "Reflection"));
    }

    // The actual difference, and the reason the row exists: a reflection binding
    // to a property that does not exist renders NOTHING and says nothing. No
    // exception, no log a user will read - just an empty label that looks like
    // missing data rather than a bug.
    [AvaloniaFact]
    public void A_Misspelt_Reflection_Binding_Fails_Silently()
    {
        var view = Shown();

        Assert.True(string.IsNullOrEmpty(Text(view, "Misspelt")),
            $"expected the misspelt binding to render nothing, got \"{Text(view, "Misspelt")}\"");
    }

    [AvaloniaFact]
    public void The_Silent_Failure_Stays_Silent_When_The_Real_Property_Changes()
    {
        var view = Shown();

        Vm(view).Title = "changed";
        Dispatcher.UIThread.RunJobs();

        Assert.True(string.IsNullOrEmpty(Text(view, "Misspelt")));
    }

    // A FallbackValue is the only thing that turns that silence into something a
    // reader can act on - and it is opt-in, per binding, which is exactly why the
    // compiled variant's build error is worth more.
    [AvaloniaFact]
    public void A_FallbackValue_Is_The_Reflection_Bindings_Only_Safety_Net()
    {
        var view = Shown();

        Assert.Equal("unavailable", Text(view, "Guarded"));
    }

    // The compiled binding cannot be misspelt at all: the same typo written as a
    // CompiledBinding is AVLN2100 at build time, so it can never appear in this
    // view and there is nothing to assert about it at run time. Stated as a test
    // so the claim lives next to the ones that are checked, rather than only in a
    // comment: what IS checkable is that the compiled path resolved a real value,
    // which it could only do because the compiler agreed the property exists.
    [AvaloniaFact]
    public void The_Compiled_Binding_Resolved_Against_A_Type_The_Compiler_Checked()
    {
        var view = Shown();

        Assert.False(string.IsNullOrEmpty(Text(view, "Compiled")));
        Assert.Equal(Vm(view).Title, Text(view, "Compiled"));
    }
}
