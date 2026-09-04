// Exercise 018 - Data Context Inheritance (beginner).
// Goal:   Set a source once and let a whole subtree bind against it.
// Drills: DataContext as an inherited property, a binding with no Source using it, and a
//         child overriding it for its own subtree.
// Passes: dotnet test --filter FullyQualifiedName~Ex018_

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex018_DataContextInheritance
{
    /// <summary>
    /// A Border whose DataContext is <paramref name="outer"/>, containing a StackPanel with
    /// two TextBlocks:
    /// <list type="bullet">
    ///   <item>one named "Outer" bound to the path <c>Caption</c> with no Source of its own,
    ///     so it reads the inherited context,</item>
    ///   <item>one named "Inner" whose own DataContext is <paramref name="inner"/>, bound to
    ///     the same path.</item>
    /// </list>
    /// Neither binding may name a Source: both paths are resolved against whatever context
    /// reaches the element.
    /// </summary>
    public static Border CreateNestedLabels(object outer, object inner)
    {
        var outerLabel = new TextBlock { Name = "Outer" };
        outerLabel.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("Caption") });

        var innerLabel = new TextBlock
        {
            Name = "Inner",

            // Setting DataContext here stops the inheritance at this element and starts a
            // new one for its subtree. The binding below is written identically to the
            // outer one and still resolves against a different object.
            DataContext = inner,
        };
        innerLabel.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("Caption") });

        var panel = new StackPanel();
        panel.Children.Add(outerLabel);
        panel.Children.Add(innerLabel);

        return new Border
        {
            // One assignment, and every descendant that did not override it can bind.
            DataContext = outer,
            Child = panel,
        };
    }
}
