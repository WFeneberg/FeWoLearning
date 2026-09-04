// Exercise 036 - Custom Templated Control (intermediate).
// Goal:   Ship a control that brings its own look, and works with no configuration.
// Drills: a Control subclass with its own dependency properties, DefaultStyleKey, a Style
//         whose Setter supplies the Template, and registering that style implicitly so a
//         consumer only writes the element.
// Passes: dotnet test --filter FullyQualifiedName~Ex036_
//
// This is the shape of every control in every control library: the type declares the API
// and the parts it needs, and a style declares what those parts look like. The two halves
// meet through the resource lookup, never through a constructor.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public partial class Ex036_CustomTemplatedControl : Control
{
    /// <summary>
    /// Test fixture: the look. A Border holding two TextBlocks named "PART_Caption" and
    /// "PART_Count", each bound back to the templated parent.
    /// </summary>
    public static readonly ControlTemplate BadgeTemplate = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <StackPanel>
                <TextBlock x:Name="PART_Caption" Text="{Binding Caption, RelativeSource={RelativeSource TemplatedParent}}" />
                <TextBlock x:Name="PART_Count" Text="{Binding Count, RelativeSource={RelativeSource TemplatedParent}}" />
            </StackPanel>
        </ControlTemplate>
        """);

    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(
            nameof(Caption),
            typeof(string),
            typeof(Ex036_CustomTemplatedControl),
            new PropertyMetadata(""));

    public static readonly DependencyProperty CountProperty =
        DependencyProperty.Register(
            nameof(Count),
            typeof(int),
            typeof(Ex036_CustomTemplatedControl),
            new PropertyMetadata(0));

    public Ex036_CustomTemplatedControl() =>
        // TODO: set DefaultStyleKey to this type. It is what makes the framework look for
        // this control's default style instead of Control's.
        throw new NotImplementedException("TODO: Ex036 - declare which type's default style to look for");

    /// <summary>
    /// Test hook: <see cref="Control.DefaultStyleKey"/> is protected, so this exposes what
    /// the constructor declared. Not something a real control would publish.
    /// </summary>
    public object? DeclaredStyleKey => DefaultStyleKey;

    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    /// <summary>
    /// The style a consumer is expected to have in scope: it sets nothing but
    /// <see cref="Control.Template"/>, to <see cref="BadgeTemplate"/>.
    /// </summary>
    public static Style CreateDefaultStyle() =>
        throw new NotImplementedException("TODO: Ex036 - build the style that supplies the template");

    /// <summary>
    /// A StackPanel with <see cref="CreateDefaultStyle"/> registered so that every
    /// <see cref="Ex036_CustomTemplatedControl"/> below it is styled without being told,
    /// holding <paramref name="badges"/>.
    /// </summary>
    public static StackPanel CreateHost(params Ex036_CustomTemplatedControl[] badges) =>
        throw new NotImplementedException("TODO: Ex036 - register the style implicitly on a host");
}
