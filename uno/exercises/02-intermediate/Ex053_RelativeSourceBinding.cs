// Exercise 053 - Relative Source Binding (intermediate).
// Goal:   Bind to the element itself, or to the control a template belongs to.
// Drills: RelativeSourceMode.Self for a self-referential binding, TemplatedParent from
//         inside a ControlTemplate, and how TemplateBinding relates to both.
// Passes: dotnet test --filter FullyQualifiedName~Ex053_
//
// TemplateBinding is a shorthand for a one-way TemplatedParent binding, and it is the only
// one of the two that the XAML compiler can resolve at build time. The long form buys what
// the shorthand cannot do: a converter, a two-way mode, a nested path.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public static class Ex053_RelativeSourceBinding
{
    /// <summary>
    /// Test fixture: a template whose two labels reach the templated parent, one through
    /// TemplateBinding and one through the long form with a converter attached.
    /// </summary>
    public static readonly ControlTemplate ParentAwareTemplate = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="ContentControl">
            <StackPanel>
                <TextBlock x:Name="PART_Short" Text="{TemplateBinding Tag}" />
                <TextBlock x:Name="PART_Long"
                           Text="{Binding Tag, RelativeSource={RelativeSource TemplatedParent}}" />
            </StackPanel>
        </ControlTemplate>
        """);

    /// <summary>
    /// A TextBlock whose Text is bound to its own Tag - the element is its own source, with
    /// nothing else in the tree involved.
    /// </summary>
    public static TextBlock CreateSelfBoundLabel(string tag) =>
        // TODO: set Tag, then bind TextProperty to the path "Tag" with
        // RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self }.
        throw new NotImplementedException("TODO: Ex053 - bind the element to itself");

    /// <summary>
    /// A templated ContentControl carrying <paramref name="tag"/> in its Tag, so both labels
    /// in <see cref="ParentAwareTemplate"/> can reach it.
    /// </summary>
    public static ContentControl CreateTemplatedCard(string tag) =>
        throw new NotImplementedException("TODO: Ex053 - build the templated card");
}
