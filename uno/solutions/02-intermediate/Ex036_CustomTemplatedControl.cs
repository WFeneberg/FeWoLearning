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
        // This is the lookup key, not a look. It tells the framework "when you go looking
        // for my default style, ask for this type" - without it the search is for Control,
        // whose style provides no template, and the control renders nothing.
        DefaultStyleKey = typeof(Ex036_CustomTemplatedControl);

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
    public static Style CreateDefaultStyle() => new(typeof(Ex036_CustomTemplatedControl))
    {
        // One setter, and it is the Template. Everything else a consumer might want to
        // change stays a property on the control.
        Setters = { new Setter(TemplateProperty, BadgeTemplate) },
    };

    /// <summary>
    /// A StackPanel with <see cref="CreateDefaultStyle"/> registered so that every
    /// <see cref="Ex036_CustomTemplatedControl"/> below it is styled without being told,
    /// holding <paramref name="badges"/>.
    /// </summary>
    public static StackPanel CreateHost(params Ex036_CustomTemplatedControl[] badges)
    {
        var host = new StackPanel();

        // Keyed by the control type, so every badge below finds it without being told. In a
        // real library this dictionary is merged into App.Resources instead - the mechanism
        // is identical, only the scope is wider.
        host.Resources[typeof(Ex036_CustomTemplatedControl)] = CreateDefaultStyle();

        foreach (var badge in badges)
        {
            host.Children.Add(badge);
        }

        return host;
    }
}
