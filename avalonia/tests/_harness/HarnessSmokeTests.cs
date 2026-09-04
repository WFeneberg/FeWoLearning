using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Tests;

/// <summary>
/// Not an exercise: proves the harness itself still works. ReactiveUI initialization and
/// the layout pass are prerequisites for every exercise in the track, so when one of
/// them breaks these fail first and every exercise failure after them is noise.
/// </summary>
public class HarnessSmokeTests
{
    private sealed class Probe : ReactiveObject
    {
        private int _value;
        public int Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }
    }

    [Fact]
    public void ReactiveUI_Is_Initialized_So_WhenAnyValue_Works()
    {
        var probe = new Probe();
        var seen = new List<int>();
        using var sub = probe.WhenAnyValue(x => x.Value).Subscribe(seen.Add);

        probe.Value = 7;

        // WhenAnyValue emits the current value on subscribe, then each change.
        Assert.Equal(new[] { 0, 7 }, seen);
    }

    [Fact]
    public void RxVoid_Is_The_Unit_Type_And_Commands_Gate_On_CanExecute()
    {
        var probe = new Probe();
        ReactiveCommand<RxVoid, RxVoid> command = ReactiveCommand.Create(
            () => { probe.Value = 0; },
            probe.WhenAnyValue(x => x.Value).Select(v => v != 0));

        Assert.False(((System.Windows.Input.ICommand)command).CanExecute(null));
        probe.Value = 3;
        Assert.True(((System.Windows.Input.ICommand)command).CanExecute(null));
    }

    [AvaloniaFact]
    public void Show_Drives_A_Full_Layout_Pass_On_Children()
    {
        var inner = new Border { Name = "Inner", Height = 30 };
        var view = new UserControl { Content = new StackPanel { Children = { inner } } };

        ViewHarness.Show(view, 200, 100);

        Assert.Equal(200, view.Bounds.Width);
        Assert.Equal(100, view.Bounds.Height);
        Assert.Equal(30, inner.Bounds.Height);
        Assert.Equal(200, inner.Bounds.Width);
    }

    [AvaloniaFact]
    public void RunJobs_Drains_Dispatcher_Work_Queued_By_Bindings()
    {
        var probe = new Probe();
        var text = new TextBlock();
        text.Bind(TextBlock.TextProperty,
            probe.WhenAnyValue(x => x.Value).Select(v => v.ToString()));
        var view = new UserControl { Content = text };
        ViewHarness.Show(view, 200, 100);

        probe.Value = 42;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("42", text.Text);
    }
}
