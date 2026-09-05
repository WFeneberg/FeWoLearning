// Exercise 026 - Action Parameters (beginner).
// Goal:   Learn the two other ways an attach expression's parameter can resolve, beyond ex025's
//         quoted literal: a bare identifier names an ELEMENT (not a view-model property), and the
//         value MessageBinder pulls off it is coerced to whatever type the target method declares.
// Drills: cal:Message.Attach="Method(ElementName)" resolving ElementName against the view's named
//         elements, never against the view model; that a TextBox's convention parameter property
//         is Text; MessageBinder coercing that string to the parameter's declared CLR type for
//         free, with no extra syntax on the attach string itself.
// Passes: dotnet test --filter FullyQualifiedName~Ex026_
//
// Measured on this machine (Caliburn.Micro 5.0.258): a TextBox x:Name="Box" and two buttons whose
// cal:Message.Attach reads Box as a bare identifier - FromElement(Box) and Coerced(Box) - both
// hosted with Show: FromElement(string value), whose parameter is a string, received Box.Text
// verbatim. Coerced(int value), whose parameter is an int, received Box.Text parsed into a real
// Int32 - MessageBinder converts the element's string value to whatever type the method's
// parameter declares. The bare identifier is NOT a view-model property reference - it is an
// ELEMENT name, and Caliburn reads that element's own convention parameter property (Text, for a
// TextBox) at the moment the action fires, not at bind time: changing Box.Text after the view is
// built and hosted, but before the click, changes what arrives - proving this is a live read of
// the element, not a value captured once when the attach string was parsed.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex026_ActionParameters
{
    /// <summary>
    /// Builds a view carrying a TextBox named Box (initial Text "42") plus two buttons whose
    /// cal:Message.Attach reads Box as a bare identifier - one calling FromElement(Box), the
    /// other Coerced(Box) - and gives the view the supplied viewModel's DataContext.
    /// </summary>
    public (FrameworkElement View, TextBox Box, Button FromElementButton, Button CoercedButton) BuildView(object viewModel) =>
        throw new NotImplementedException("TODO: Ex026 - XamlReader.Parse a TextBox 'Box' plus buttons attached to FromElement(Box) and Coerced(Box); set view.DataContext = viewModel");
}

/// <summary>A view model with two methods of different parameter types, both fed from the same element.</summary>
public class Ex026_Vm : PropertyChangedBase
{
    /// <summary>What FromElement(string) last received.</summary>
    public string? FromElementValue { get; private set; }

    /// <summary>How many times FromElement actually ran.</summary>
    public int FromElementCallCount { get; private set; }

    /// <summary>What Coerced(int) last received - a real Int32, not a string.</summary>
    public int CoercedValue { get; private set; }

    /// <summary>How many times Coerced actually ran.</summary>
    public int CoercedCallCount { get; private set; }

    public void FromElement(string value)
    {
        FromElementValue = value;
        FromElementCallCount++;
    }

    public void Coerced(int value)
    {
        CoercedValue = value;
        CoercedCallCount++;
    }
}
