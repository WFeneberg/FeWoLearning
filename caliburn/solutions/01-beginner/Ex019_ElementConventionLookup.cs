// Exercise 019 - Element Convention Lookup (beginner).
// Goal:   Learn what ConventionManager.GetElementConvention actually returns for a given
//         element TYPE - not a nullable "maybe", but a WALK up the type hierarchy that
//         always lands on something, even for a type Caliburn has never heard of.
// Drills: CheckBox served by the ToggleButton convention, ComboBox/ListBox served by
//         Selector's, and the walk continuing past a custom subclass straight to its
//         registered base; every FrameworkElement ultimately matches the Visibility
//         fallback instead of returning null - so "no convention" is never an error, it is
//         silent.
// Passes: dotnet test --filter FullyQualifiedName~Ex019_

using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex019_ElementConventionLookup
{
    /// <summary>Delegates to Caliburn's own convention lookup - never null for a FrameworkElement.</summary>
    public ElementConvention Lookup(Type elementType) => ConventionManager.GetElementConvention(elementType);
}

/// <summary>A CheckBox subclass Caliburn has never seen - proves the walk goes past it too.</summary>
public class Ex019_CustomCheckBox : CheckBox;

/// <summary>A FrameworkElement Caliburn has never seen at all - proves the fallback, not null.</summary>
public class Ex019_NeverSeenElement : FrameworkElement;
