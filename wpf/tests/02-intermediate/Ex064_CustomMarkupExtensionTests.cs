using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex064_CustomMarkupExtensionTests : WpfTestContext
{
    // Hand-built IServiceProvider/IProvideValueTarget - the code-only stand-in for what the XAML
    // parser would otherwise supply. See Ex025/Ex058 for the same substitution in this XAML-free
    // tier, and this row's own Goal comment for why XamlReader.Parse is deliberately not used here.
    private sealed class FakeProvideValueTarget(object targetObject, object targetProperty) : IProvideValueTarget
    {
        public object TargetObject { get; } = targetObject;
        public object TargetProperty { get; } = targetProperty;
    }

    private sealed class FakeServiceProvider(IProvideValueTarget target) : IServiceProvider
    {
        public object? GetService(Type serviceType) => serviceType == typeof(IProvideValueTarget) ? target : null;
    }

    private static object? ProvideValueFor(DependencyObject targetObject, DependencyProperty targetProperty)
    {
        var extension = new Ex064_PropertyDefaultExtension();
        var serviceProvider = new FakeServiceProvider(new FakeProvideValueTarget(targetObject, targetProperty));
        return extension.ProvideValue(serviceProvider);
    }

    [WpfFact]
    public void Resolves_To_The_Target_Propertys_Own_Default_For_A_String_Property()
    {
        var result = ProvideValueFor(new TextBox(), TextBox.TextProperty);

        Assert.Equal(string.Empty, result);
    }

    [WpfFact]
    public void Resolves_To_The_Target_Propertys_Own_Default_For_A_Nullable_Bool_Property()
    {
        // A different target, a different property, a different default shape (bool, not
        // string) - a mutant that assumes every target property is string-shaped fails here.
        var result = ProvideValueFor(new CheckBox(), ToggleButton.IsCheckedProperty);

        Assert.Equal(false, result);
    }

    [WpfFact]
    public void Resolves_To_The_Target_Propertys_Own_Default_For_A_Visibility_Property()
    {
        // A third target/property pair, a third default shape (an enum) - a mutant returning a
        // single hard-coded constant cannot satisfy all three of these tests at once.
        var result = ProvideValueFor(new TextBlock(), UIElement.VisibilityProperty);

        Assert.Equal(Visibility.Visible, result);
    }

    [WpfFact]
    public void Resolves_To_The_Target_Propertys_Own_Default_For_A_Numeric_Property()
    {
        var result = ProvideValueFor(new Slider(), RangeBase.MinimumProperty);

        Assert.Equal(0.0, result);
    }
}
