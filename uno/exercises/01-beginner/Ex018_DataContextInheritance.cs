// Exercise 018 - Data Context Inheritance (beginner).
// Goal:   Set a source once and let a whole subtree bind against it.
// Drills: DataContext as an inherited property, a binding with no Source using it, and a
//         child overriding it for its own subtree.
// Passes: dotnet test --filter FullyQualifiedName~Ex018_
//
// This is why real markup almost never sets Binding.Source: the context comes down the
// tree, so a template written once works for every item it is applied to.

using Microsoft.UI.Xaml.Controls;

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
    public static Border CreateNestedLabels(object outer, object inner) =>
        // TODO: build the tree, set the two DataContexts, and bind both TextBlocks to
        // "Caption" without a Source. Set each TextBlock's Name so the test can find it.
        throw new NotImplementedException("TODO: Ex018 - bind two labels through the data context");
}
