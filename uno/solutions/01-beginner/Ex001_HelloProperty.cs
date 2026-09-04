// Exercise 001 - Hello Property (beginner).
// Goal:   Give this class a real dependency property instead of a plain CLR property.
// Drills: DependencyProperty.Register, PropertyMetadata default values, GetValue/SetValue,
//         and why the class has to be `partial`.
// Passes: dotnet test --filter FullyQualifiedName~Ex001_

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex001_HelloProperty : DependencyObject
{
    // Registering returns the property's identity. Everything that wants to talk about
    // "Level" without going through the CLR property - bindings, styles, animations,
    // ClearValue - needs this instance, which is why it is public, static and readonly.
    public static readonly DependencyProperty LevelProperty =
        DependencyProperty.Register(
            nameof(Level),
            typeof(int),
            typeof(Ex001_HelloProperty),
            new PropertyMetadata(5));

    /// <summary>Fill level, 5 unless somebody sets it.</summary>
    public int Level
    {
        // The CLR property is a typed façade over the property store, nothing more. No
        // backing field: a field would not participate in binding, styling or ClearValue,
        // and the two views of "Level" would drift apart.
        get => (int)GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }
}
