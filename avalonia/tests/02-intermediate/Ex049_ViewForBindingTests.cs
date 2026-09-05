using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex049_ViewForBindingTests
{
    [AvaloniaFact]
    public void Setting_ViewModel_Syncs_DataContext_And_The_Bound_TextBlock_Follows_It()
    {
        var view = new Ex049_ViewForBinding();
        var vm = new Ex049_ViewForBindingViewModel { Greeting = "Hallo Welt" };

        view.ViewModel = vm;

        // ReactiveUserControl<T> keeps DataContext synced to ViewModel on its own -
        // this is given/base-class behaviour, not something the exercise's XAML
        // wires up. Asserting it here documents the mechanism the binding relies on.
        Assert.Same(vm, view.DataContext);

        ViewHarness.Show(view);

        var textBlock = view.FindControl<TextBlock>("GreetingText");
        Assert.NotNull(textBlock);
        Assert.Equal("Hallo Welt", textBlock!.Text);

        // Changing the view model after the initial show, without touching the view
        // at all: a cheat that copies Greeting into Text once (e.g. from a
        // code-behind ViewModel setter) rather than genuinely binding would leave
        // the old text in place here.
        vm.Greeting = "Guten Tag";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Guten Tag", textBlock.Text);
    }

    // A second, distinctly-valued view model from scratch: guards against a
    // solution whose XAML happens to hard-code the first test's literal text.
    [AvaloniaFact]
    public void A_Second_Distinctly_Valued_ViewModel_Also_Binds_Correctly()
    {
        var view = new Ex049_ViewForBinding();
        var vm = new Ex049_ViewForBindingViewModel { Greeting = "A completely different value" };
        view.ViewModel = vm;

        ViewHarness.Show(view);

        var textBlock = view.FindControl<TextBlock>("GreetingText");
        Assert.Equal("A completely different value", textBlock!.Text);
    }

    // Structural check, in the spirit of "a view exercise needs one structural
    // assertion, not only behavioural ones": pins that this is genuinely an
    // IViewFor<T> - not merely a plain UserControl that happens to expose a
    // same-shaped ViewModel property.
    [AvaloniaFact]
    public void The_View_Implements_The_IViewFor_Contract_Structurally()
    {
        var view = new Ex049_ViewForBinding();
        Assert.IsAssignableFrom<IViewFor<Ex049_ViewForBindingViewModel>>(view);

        var vm = new Ex049_ViewForBindingViewModel();
        ((IViewFor)view).ViewModel = vm;
        Assert.Same(vm, view.ViewModel);
    }
}
