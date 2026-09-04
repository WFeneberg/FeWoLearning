// Exercise 079 - Composite Content Model (advanced).
// Goal:   Give a control three content slots instead of one.
// Drills: several ContentPresenters in one template, a slot that disappears when it is
//         empty, and object-typed content that takes a string as readily as an element.
// Passes: dotnet test --filter FullyQualifiedName~Ex079_
//
// ContentControl has one slot; a real card has a header, a body and a footer. The mechanism
// is unchanged - a property per slot, a presenter per property - and the part worth getting
// right is the empty case: a header slot that keeps its padding when there is no header
// leaves a gap nobody can explain from the markup.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Advanced;

public partial class Ex079_CompositeContentModel : Control
{
    /// <summary>
    /// Test fixture: three presenters, each in its own named Border so a test can see which
    /// slots are showing.
    /// </summary>
    public static readonly ControlTemplate CardTemplate = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <StackPanel>
                <Border x:Name="PART_HeaderHost" Padding="4">
                    <ContentPresenter Content="{Binding Header, RelativeSource={RelativeSource TemplatedParent}}" />
                </Border>
                <Border x:Name="PART_BodyHost" Padding="4">
                    <ContentPresenter Content="{Binding Body, RelativeSource={RelativeSource TemplatedParent}}" />
                </Border>
                <Border x:Name="PART_FooterHost" Padding="4">
                    <ContentPresenter Content="{Binding Footer, RelativeSource={RelativeSource TemplatedParent}}" />
                </Border>
            </StackPanel>
        </ControlTemplate>
        """);

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(Ex079_CompositeContentModel),
            new PropertyMetadata(null, OnSlotChanged));

    public static readonly DependencyProperty BodyProperty =
        DependencyProperty.Register(
            nameof(Body),
            typeof(object),
            typeof(Ex079_CompositeContentModel),
            new PropertyMetadata(null, OnSlotChanged));

    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register(
            nameof(Footer),
            typeof(object),
            typeof(Ex079_CompositeContentModel),
            new PropertyMetadata(null, OnSlotChanged));

    public Ex079_CompositeContentModel() => DefaultStyleKey = typeof(Ex079_CompositeContentModel);

    /// <summary>Anything: a string, an element, a view model with a template.</summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? Body
    {
        get => GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// Collapses the host of every empty slot and shows the rest. Empty means null; an
    /// empty string is content, and a zero is content too.
    /// </summary>
    public void UpdateSlotVisibility()
    {
        Apply("PART_HeaderHost", Header);
        Apply("PART_BodyHost", Body);
        Apply("PART_FooterHost", Footer);
    }

    private void Apply(string partName, object? content)
    {
        // A replacement template may provide fewer hosts, and a control that insists on
        // all three crashes an app that only changed a style.
        if (GetTemplateChild(partName) is not FrameworkElement host)
        {
            return;
        }

        // Null is empty; "" and 0 are content. A control that treats an empty string as
        // absent cannot show a blank header on purpose - and Collapsed rather than a
        // transparent host is what takes the padding away with it.
        host.Visibility = content is null ? Visibility.Collapsed : Visibility.Visible;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateSlotVisibility();
    }

    private static void OnSlotChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex079_CompositeContentModel)sender).UpdateSlotVisibility();
}
