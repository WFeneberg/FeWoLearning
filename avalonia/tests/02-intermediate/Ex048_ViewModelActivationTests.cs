using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex048_ViewModelActivationTests
{
    // Test-only helper view - not part of the exercise, and the exercise's view
    // model has no way to reach it. ReactiveUserControl<T> already implements
    // IActivatableView; this constructor's own WhenActivated call is what forwards
    // the view's real Loaded/Unloaded lifecycle into the view model's Activator,
    // exactly as a real desktop app would. Measured on this machine: Loaded only
    // fires after Dispatcher.UIThread.RunJobs() drains the queue following Show().
    private sealed class HostView : ReactiveUserControl<Ex048_ViewModelActivationViewModel>
    {
        public HostView()
        {
            Content = new TextBlock { Text = "host" };
            this.WhenActivated(register => register(ViewModel!.Activator.Activate()));
        }
    }

    [AvaloniaFact]
    public void Showing_The_View_Activates_The_ViewModel_And_Removing_It_Disposes()
    {
        var vm = new Ex048_ViewModelActivationViewModel();
        var view = new HostView { ViewModel = vm };

        // Nothing has happened yet - a solution that increments ActivationCount (or
        // flips DisposableWasDisposed) straight in the constructor, without ever
        // going through WhenActivated, is caught right here.
        Assert.Equal(0, vm.ActivationCount);
        Assert.False(vm.DisposableWasDisposed);

        var window = new Window { Content = view, Width = 200, Height = 100 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, vm.ActivationCount);
        Assert.False(vm.DisposableWasDisposed);

        window.Content = null;
        Dispatcher.UIThread.RunJobs();

        Assert.True(vm.DisposableWasDisposed);
    }

    // A second, independent view model/view pair through the same cycle: guards
    // against a solution whose registered disposable only works once (e.g. captured
    // static/shared state instead of a fresh disposable per activation).
    [AvaloniaFact]
    public void A_Second_Independent_ViewModel_Activates_And_Disposes_Correctly_Too()
    {
        var vm = new Ex048_ViewModelActivationViewModel();
        var view = new HostView { ViewModel = vm };
        var window = new Window { Content = view, Width = 200, Height = 100 };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(1, vm.ActivationCount);
        Assert.False(vm.DisposableWasDisposed);

        window.Content = null;
        Dispatcher.UIThread.RunJobs();
        Assert.True(vm.DisposableWasDisposed);
    }
}
