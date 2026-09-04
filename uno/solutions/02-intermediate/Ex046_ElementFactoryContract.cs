// Exercise 046 - Element Factory Contract (intermediate).
// Goal:   Hand out elements and take them back, instead of building one per item.
// Drills: the GetElement/RecycleElement pair behind IElementFactory, pool accounting, and
//         resetting an element before it is reused.
// Passes: dotnet test --filter FullyQualifiedName~Ex046_
//
// A DataTemplate is really a factory with a fixed answer: build a fresh tree per item. A
// factory can do better - keep the trees and re-point them - which is what makes a long
// list affordable. The contract is small and the trap is state: an element that comes back
// still carries whatever the last item left on it.
//
// Two Uno limits shape this exercise (see uno/README.md). The public
// Microsoft.UI.Xaml.ElementFactoryGetArgs.Data is an unimplemented stub that throws, and
// Uno's ItemsRepeater refuses an ItemTemplate that is not a DataTemplate or its own
// internal shim. So the interface is implemented for real below - that is the shape you
// would write in an app - while the work sits in two plain methods the tests can drive.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// WinUI ships two pairs of these args types, one in Microsoft.UI.Xaml and one in
// Microsoft.UI.Xaml.Controls. IElementFactory uses the former; the aliases keep the
// ambiguity out of the signatures below.
using ElementFactoryGetArgs = Microsoft.UI.Xaml.ElementFactoryGetArgs;
using ElementFactoryRecycleArgs = Microsoft.UI.Xaml.ElementFactoryRecycleArgs;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Hands out <see cref="TextBlock"/>s showing the item's text, keeping recycled ones in a
/// pool instead of building new.
/// </summary>
public sealed class Ex046_ElementFactoryContract : IElementFactory
{
    private readonly Queue<TextBlock> _pool = new();

    /// <summary>How many elements this factory has ever constructed.</summary>
    public int Constructed { get; private set; }

    /// <summary>How many elements are waiting in the pool right now.</summary>
    public int Pooled => _pool.Count;

    /// <summary>
    /// Returns an element for <paramref name="data"/>: a pooled one if there is any,
    /// otherwise a new <see cref="TextBlock"/>. Either way its Text ends up being the
    /// item's <see cref="object.ToString"/>, and null becomes the empty string.
    /// </summary>
    public UIElement GetElement(object? data)
    {
        TextBlock element;

        if (_pool.Count > 0)
        {
            element = _pool.Dequeue();
        }
        else
        {
            element = new TextBlock();
            Constructed++;
        }

        // The one line that makes reuse work: the element is re-pointed at the new item
        // rather than rebuilt around it.
        element.Text = data?.ToString() ?? "";
        return element;
    }

    /// <summary>
    /// Takes <paramref name="element"/> back. It is cleared before it goes into the pool,
    /// so a reused one never shows the previous item's text. Anything that is not a
    /// <see cref="TextBlock"/> is dropped rather than pooled - a caller may hand back an
    /// element it got somewhere else.
    /// </summary>
    public void RecycleElement(UIElement element)
    {
        // A caller may hand back an element it obtained elsewhere - another factory, a
        // template. Pooling it would hand out something the next caller cannot use.
        if (element is not TextBlock textBlock)
        {
            return;
        }

        // Cleared on the way in, not on the way out: an element that keeps the old text
        // while it waits shows the previous item for a frame when it is re-attached.
        textBlock.Text = "";
        _pool.Enqueue(textBlock);
    }

    // The framework-facing half, given. Both members are pure adapters, which is true of a
    // real factory too: the interface is a calling convention, not the design.
    UIElement IElementFactory.GetElement(ElementFactoryGetArgs args) => GetElement(args.Data);

    void IElementFactory.RecycleElement(ElementFactoryRecycleArgs args) => RecycleElement(args.Element);
}
