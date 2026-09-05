// Exercise 025 - Message Attach Explicit (beginner).
// Goal:   Learn the explicit alternative to ex022's naming convention: cal:Message.Attach names
//         the method directly in XAML, so the element's x:Name no longer has to match it - and
//         can carry a literal parameter besides.
// Drills: The exact runtime XAML namespace cal:Message.Attach needs to resolve; that the
//         resulting element's DataContext (not its x:Name) is what lets the attached action
//         find its target; single quotes delimiting a literal string parameter.
// Passes: dotnet test --filter FullyQualifiedName~Ex025_
//
// Measured on this machine (Caliburn.Micro 5.0.258): Message, ActionMessage and the View helper
// all live in the assembly Caliburn.Micro.Platform, so runtime-parsed XAML declaring cal must
// say exactly:
//   xmlns:cal="clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro.Platform"
//
// A Button whose x:Name is deliberately NOT a method name ("NotAMethodName"), carrying
// cal:Message.Attach="WithParam('abcd')", still gets a real trigger - Interaction.GetTriggers
// on it holds a Microsoft.Xaml.Behaviors.EventTrigger whose action is an ActionMessage with
// MethodName "WithParam" - and that trigger exists as soon as the XAML is parsed, before any
// window and before this exercise's AttachViewModel has even run. What AttachViewModel supplies
// is the DataContext the ActionMessage needs to find WithParam on: without it, raising Click
// (even once the view is shown) invokes nothing, because there is no view model to resolve the
// method against. Single quotes in the attach string mark a literal - 'abcd' arrives at
// WithParam as the literal string "abcd", not a binding to some property named abcd.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex025_MessageAttachExplicit
{
    /// <summary>Gives the view the DataContext its cal:Message.Attach actions need in order to resolve their target.</summary>
    public void AttachViewModel(FrameworkElement view, object viewModel) =>
        throw new NotImplementedException("TODO: Ex025 - view.DataContext = viewModel");
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
