using System;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Expert;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex091_AppShellArchitectureTests
{
    private static Ex091_AppShellArchitecture Wired()
    {
        var shell = new Ex091_AppShellArchitecture();
        shell.Wire();
        return shell;
    }

    [AvaloniaFact]
    public void Navigating_Pushes_The_Page_Onto_The_Router()
    {
        var shell = Wired();

        var page = shell.NavigateTo<Ex091_HomeViewModel>();

        Assert.Single(shell.Router.NavigationStack);
        Assert.Same(page, shell.Router.NavigationStack[0]);
        Assert.Equal(["home"], shell.Visited);
    }

    // The container half: neither page has a parameterless constructor, so a
    // shell that news them up cannot compile - and one that resolves them gets
    // the shell wired in as IScreen for free.
    [AvaloniaFact]
    public void A_Resolved_Page_Is_Given_The_Shell_As_Its_Screen()
    {
        var shell = Wired();

        var page = shell.NavigateTo<Ex091_DetailViewModel>();

        Assert.Same(shell, page.HostScreen);
    }

    // The journal is a singleton, so both pages write to the same object. That is
    // the thing injection buys and constructing-in-place loses.
    [AvaloniaFact]
    public void Both_Pages_Share_One_Journal()
    {
        var shell = Wired();

        var home = shell.NavigateTo<Ex091_HomeViewModel>();
        var detail = shell.NavigateTo<Ex091_DetailViewModel>();

        Assert.Same(home.Journal, detail.Journal);
    }

    // ...while the pages themselves are transient, so navigating twice does not
    // hand back the same page with stale state.
    [AvaloniaFact]
    public void Navigating_Twice_Yields_Two_Different_Pages()
    {
        var shell = Wired();

        var first = shell.NavigateTo<Ex091_DetailViewModel>();
        var second = shell.NavigateTo<Ex091_DetailViewModel>();

        Assert.NotSame(first, second);
        Assert.Equal(2, shell.Router.NavigationStack.Count);
    }

    [AvaloniaFact]
    public void Going_Back_Pops_And_Records_What_Is_Now_On_Top()
    {
        var shell = Wired();

        shell.NavigateTo<Ex091_HomeViewModel>();
        shell.NavigateTo<Ex091_DetailViewModel>();
        shell.GoBack();

        Assert.Single(shell.Router.NavigationStack);
        Assert.Equal(["home", "detail", "home"], shell.Visited);
    }

    [AvaloniaFact]
    public void Going_Back_Off_The_Root_Records_The_Empty_Stack()
    {
        var shell = Wired();

        shell.NavigateTo<Ex091_HomeViewModel>();
        shell.GoBack();

        Assert.Empty(shell.Router.NavigationStack);
        Assert.Equal(["home", "(root)"], shell.Visited);
    }

    // A page the container does not know about must fail loudly rather than
    // returning null and leaving the router holding nothing.
    //
    // Note how this one goes red against the untouched stub: not by an unhandled
    // throw but by an exception-type mismatch, since the stub throws
    // NotImplementedException where InvalidOperationException is expected. xunit
    // prints the TODO in the failure either way, so the cause is still obvious.
    [AvaloniaFact]
    public void An_Unregistered_Page_Is_Refused()
    {
        var shell = new Ex091_AppShellArchitecture();

        Assert.Throws<InvalidOperationException>(() => shell.NavigateTo<Ex091_HomeViewModel>());
    }

    // Activation is not construction: the page exists and has been navigated to,
    // and still nothing has started.
    [AvaloniaFact]
    public void Navigating_Alone_Activates_Nothing()
    {
        var shell = Wired();

        var page = shell.NavigateTo<Ex091_DetailViewModel>();
        page.SetUpActivation();

        Assert.Equal(0, page.Activations);
        Assert.Equal(0, page.Deactivations);
        Assert.Empty(page.Journal.Entries);
    }

    [AvaloniaFact]
    public void Activating_Runs_The_Block_And_Journals_It()
    {
        var shell = Wired();
        var page = shell.NavigateTo<Ex091_DetailViewModel>();
        page.SetUpActivation();

        page.Activator.Activate();

        Assert.Equal(1, page.Activations);
        Assert.Equal(0, page.Deactivations);
        Assert.Equal(["detail activated"], page.Journal.Entries);
    }

    // The half that is easy to leave out: a WhenActivated block which registers
    // nothing for disposal never stops what it started, and only deactivation
    // shows it up.
    [AvaloniaFact]
    public void Deactivating_Disposes_What_Activation_Registered()
    {
        var shell = Wired();
        var page = shell.NavigateTo<Ex091_DetailViewModel>();
        page.SetUpActivation();

        page.Activator.Activate();
        page.Activator.Deactivate();

        Assert.Equal(1, page.Activations);
        Assert.Equal(1, page.Deactivations);
        Assert.Equal(["detail activated", "detail deactivated"], page.Journal.Entries);
    }

    [AvaloniaFact]
    public void A_Second_Activation_Runs_The_Block_Again()
    {
        var shell = Wired();
        var page = shell.NavigateTo<Ex091_DetailViewModel>();
        page.SetUpActivation();

        page.Activator.Activate();
        page.Activator.Deactivate();
        page.Activator.Activate();

        Assert.Equal(2, page.Activations);
        Assert.Equal(1, page.Deactivations);
        Assert.Equal(
            ["detail activated", "detail deactivated", "detail activated"],
            page.Journal.Entries);
    }
}
