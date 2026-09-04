// Exercise 001 - Hello Property (beginner).
// Goal:   Give this class a real dependency property instead of a plain CLR property.
// Drills: DependencyProperty.Register, PropertyMetadata default values, GetValue/SetValue,
//         and why the class has to be `partial`.
// Passes: dotnet test --filter FullyQualifiedName~Ex001_
//
// Note: `partial` is not decoration. Uno generates the DependencyObject plumbing into a
// second half of this class, so dropping it fails the build with CS0260.

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex001_HelloProperty : DependencyObject
{
    // TODO: register a dependency property for Level: name "Level", type int, owner
    // Ex001_HelloProperty, default value 5. Expose it as a public static readonly field
    // called LevelProperty - the field IS the identity of the property, so register once.

    /// <summary>Fill level, 5 unless somebody sets it.</summary>
    public int Level
    {
        // TODO: read the value out of the dependency property, do not add a backing field.
        get => throw new NotImplementedException("TODO: Ex001 - read Level from the dependency property");

        // TODO: write the value into the dependency property.
        set => throw new NotImplementedException("TODO: Ex001 - write Level into the dependency property");
    }
}
