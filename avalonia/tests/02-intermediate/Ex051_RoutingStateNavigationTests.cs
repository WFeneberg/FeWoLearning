using System.Collections.Generic;
using ReactiveUI;
using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex051_RoutingStateNavigationTests
{
    // Reads straight off the real RoutingState - a private Stack<T> field standing
    // in for navigation would never touch this, and NavigationStack would stay
    // empty.
    [Fact]
    public void NavigateTo_Pushes_Onto_The_Real_RoutingState()
    {
        var screen = new Ex051_ScreenViewModel();
        var foo = new Ex051_FooViewModel(screen);

        screen.NavigateTo(foo);

        Assert.Single(screen.Router.NavigationStack);
        Assert.Same(foo, screen.Router.NavigationStack[0]);
    }

    [Fact]
    public void NavigateTo_Twice_Then_GoBack_Reproduces_The_Measured_Stack_Depths()
    {
        var screen = new Ex051_ScreenViewModel();
        var foo = new Ex051_FooViewModel(screen);
        var bar = new Ex051_BarViewModel(screen);

        Assert.Empty(screen.Router.NavigationStack);

        screen.NavigateTo(foo);
        Assert.Single(screen.Router.NavigationStack);
        Assert.Same(foo, screen.Router.NavigationStack[0]);

        screen.NavigateTo(bar);
        Assert.Equal(2, screen.Router.NavigationStack.Count);
        Assert.Same(bar, screen.Router.NavigationStack[1]);

        screen.GoBack();
        Assert.Single(screen.Router.NavigationStack);
        Assert.Same(foo, screen.Router.NavigationStack[0]);
    }

    // The trap: CurrentViewModel is an IObservable, not a synchronously-readable
    // property. Measured emission sequence for navigate(foo), navigate(bar),
    // back: [null, foo, bar, foo] - it starts with null.
    [Fact]
    public void CurrentViewModel_Emits_Null_Then_Foo_Then_Bar_Then_Foo()
    {
        var screen = new Ex051_ScreenViewModel();
        var foo = new Ex051_FooViewModel(screen);
        var bar = new Ex051_BarViewModel(screen);
        var emissions = new List<IRoutableViewModel?>();
        screen.Router.CurrentViewModel.Subscribe(vm => emissions.Add(vm));

        screen.NavigateTo(foo);
        screen.NavigateTo(bar);
        screen.GoBack();

        Assert.Equal(new IRoutableViewModel?[] { null, foo, bar, foo }, emissions);
    }

    // GoBack past the root must not throw out of the exercise's own method.
    [Fact]
    public void GoBack_Past_The_Root_Does_Not_Throw()
    {
        var screen = new Ex051_ScreenViewModel();

        var ex = Record.Exception(() => screen.GoBack());

        Assert.Null(ex);
        Assert.Empty(screen.Router.NavigationStack);
    }
}
