// Exercise 027 - Action Special Values (beginner).
// Goal:   Learn the small set of magic tokens an attach expression can pass instead of a literal
//         or an element: $eventArgs, $dataContext, $source, $view - four of Caliburn's five
//         documented SpecialValues - plus $this, which is not one of those five keys yet still
//         resolves.
// Drills: cal:Message.Attach passing several special values to one method in a single call;
//         MessageBinder.SpecialValues being a lowercase-keyed, process-global dictionary this
//         exercise only READS, never mutates (ex069 owns adding to it); that $source is the
//         clicked element itself, $dataContext is the view model, and $view - measured below -
//         is NOT simply "the containing view".
// Passes: dotnet test --filter FullyQualifiedName~Ex027_

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Caliburn.Micro;
// FeWoLearning.Caliburn.Exercises.Beginner nests inside FeWoLearning.Caliburn, so a fully
// qualified Caliburn.Micro.Action reference resolves "Caliburn" against THIS namespace's own
// ancestor instead of the package root (CS0234) - the same trap avalonia/ hit with
// Avalonia.Media.TextWrapping (see the root CLAUDE.md). A using-alias is exempt.
using CaliburnAction = Caliburn.Micro.Action;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex027_ActionSpecialValues
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                     xmlns:cal="clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro.Platform">
          <StackPanel>
            <Button x:Name="Go" Content="Go" cal:Message.Attach="CaptureAll($eventArgs, $dataContext, $source, $view, $this)" />
          </StackPanel>
        </UserControl>
        """;

    public (FrameworkElement View, Button Button) BuildView(object viewModel)
    {
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var button = (Button)view.FindName("Go")!;
        // A plain view.DataContext = viewModel would leave $view identical to $source (see the
        // header comment) - SetTarget on the root is what makes $view resolve to the root itself.
        CaliburnAction.SetTarget(view, viewModel);
        return (view, button);
    }
}

/// <summary>A view model whose one method records every special value it was handed, in argument order.</summary>
public class Ex027_Vm : PropertyChangedBase
{
    public RoutedEventArgs? LastEventArgs { get; private set; }
    public object? LastDataContext { get; private set; }
    public object? LastSource { get; private set; }
    public object? LastView { get; private set; }
    public object? LastThis { get; private set; }
    public int CallCount { get; private set; }

    public void CaptureAll(RoutedEventArgs eventArgs, object dataContext, object source, object view, object self)
    {
        LastEventArgs = eventArgs;
        LastDataContext = dataContext;
        LastSource = source;
        LastView = view;
        LastThis = self;
        CallCount++;
    }
}
