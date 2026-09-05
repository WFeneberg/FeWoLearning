// Exercise 025 - Message Attach Explicit (beginner).
// Goal:   Learn the explicit alternative to ex022's naming convention: cal:Message.Attach names
//         the method directly in XAML, so the element's x:Name no longer has to match it - and
//         can carry a literal parameter besides.
// Drills: The exact runtime XAML namespace cal:Message.Attach needs to resolve; that the
//         resulting element's DataContext (not its x:Name) is what lets the attached action
//         find its target; single quotes delimiting a literal string parameter.
// Passes: dotnet test --filter FullyQualifiedName~Ex025_

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex025_MessageAttachExplicit
{
    /// <summary>Gives the view the DataContext its cal:Message.Attach actions need in order to resolve their target.</summary>
    public void AttachViewModel(FrameworkElement view, object viewModel) => view.DataContext = viewModel;
}

/// <summary>A view model whose method takes a parameter - invoked explicitly, never by x:Name convention.</summary>
public class Ex025_Vm : PropertyChangedBase
{
    /// <summary>How many times WithParam actually ran.</summary>
    public int Count { get; private set; }

    /// <summary>The parameter value passed on the most recent invocation, if any.</summary>
    public string? LastParam { get; private set; }

    public void WithParam(string value)
    {
        Count++;
        LastParam = value;
    }
}
