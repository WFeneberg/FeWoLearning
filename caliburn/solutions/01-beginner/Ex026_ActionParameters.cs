// Exercise 026 - Action Parameters (beginner).
// Goal:   Learn the two other ways an attach expression's parameter can resolve, beyond ex025's
//         quoted literal: a bare identifier names an ELEMENT (not a view-model property), and the
//         value MessageBinder pulls off it is coerced to whatever type the target method declares.
// Drills: cal:Message.Attach="Method(ElementName)" resolving ElementName against the view's named
//         elements, never against the view model; that a TextBox's convention parameter property
//         is Text; MessageBinder coercing that string to the parameter's declared CLR type for
//         free, with no extra syntax on the attach string itself.
// Passes: dotnet test --filter FullyQualifiedName~Ex026_

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex026_ActionParameters
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                     xmlns:cal="clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro.Platform">
          <StackPanel>
            <TextBox x:Name="Box" Text="42" />
            <Button x:Name="FromElementButton" Content="From" cal:Message.Attach="FromElement(Box)" />
            <Button x:Name="CoercedButton" Content="Coerced" cal:Message.Attach="Coerced(Box)" />
          </StackPanel>
        </UserControl>
        """;

    public (FrameworkElement View, TextBox Box, Button FromElementButton, Button CoercedButton) BuildView(object viewModel)
    {
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        view.DataContext = viewModel;
        var box = (TextBox)view.FindName("Box")!;
        var fromElementButton = (Button)view.FindName("FromElementButton")!;
        var coercedButton = (Button)view.FindName("CoercedButton")!;
        return (view, box, fromElementButton, coercedButton);
    }
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
