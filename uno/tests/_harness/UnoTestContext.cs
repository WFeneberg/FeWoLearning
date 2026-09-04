using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace FeWoLearning.Uno.Tests;

/// <summary>
/// Base class for every exercise test: gives a laid-out visual tree and a few
/// tree-walking helpers, so tests can assert on what the framework actually produced
/// instead of on the properties they just set themselves.
/// </summary>
public abstract class UnoTestContext
{
    protected UnoTestContext() => UnoHeadlessRuntime.Boot();

    /// <summary>
    /// Runs a real measure/arrange pass. Nothing about a <see cref="FrameworkElement"/>
    /// is trustworthy before this: DesiredSize is zero, ActualWidth/Height are zero,
    /// templates are not applied and template children do not exist yet.
    /// </summary>
    protected static T Layout<T>(T element, double width = 400, double height = 400)
        where T : UIElement
    {
        element.Measure(new Size(width, height));
        element.Arrange(new Rect(0, 0, width, height));
        return element;
    }

    /// <summary>
    /// Where the layout pass actually put an element, relative to its parent. Only
    /// meaningful after <see cref="Layout"/>.
    /// </summary>
    /// <remarks>
    /// Read off <c>ActualOffset</c> rather than <c>TransformToVisual</c>: the latter needs
    /// render state that a windowless tree does not have, and quietly returns the origin.
    /// </remarks>
    protected static Point Offset(UIElement element) =>
        new(element.ActualOffset.X, element.ActualOffset.Y);

    /// <summary>Depth-first walk of the visual tree, template children included.</summary>
    protected static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            yield return child;

            foreach (var grandChild in Descendants(child))
            {
                yield return grandChild;
            }
        }
    }

    /// <summary>First descendant of type <typeparamref name="T"/>, optionally by x:Name.</summary>
    protected static T FindDescendant<T>(DependencyObject root, string? name = null)
        where T : FrameworkElement
    {
        foreach (var candidate in Descendants(root).OfType<T>())
        {
            if (name is null || candidate.Name == name)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException(
            $"No {typeof(T).Name}{(name is null ? "" : $" named '{name}'")} in the visual tree of {root.GetType().Name}.");
    }
}
