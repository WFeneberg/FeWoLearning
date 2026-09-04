// Exercise 026 - Control Template Basics (beginner).
// Goal:   Replace a control's whole appearance without touching the control.
// Drills: ControlTemplate, TemplateBinding from a template child back to the templated
//         parent, and ContentPresenter as the hole the content goes into.
// Passes: dotnet test --filter FullyQualifiedName~Ex026_

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex026_ControlTemplateBasics
{
    private static readonly ControlTemplate Template = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="ContentControl">
            <Border x:Name="PART_Root"
                    Background="{TemplateBinding Background}"
                    Padding="{TemplateBinding Padding}">
                <ContentPresenter Content="{TemplateBinding Content}" />
            </Border>
        </ControlTemplate>
        """);

    /// <summary>
    /// A template for a <see cref="ContentControl"/>: a Border named "PART_Root" whose
    /// Background and Padding follow the templated parent's, holding a
    /// <see cref="ContentPresenter"/> whose Content follows too. Parsed once and cached.
    /// </summary>
    /// <remarks>
    /// Both the default namespace and the <c>x</c> namespace have to be declared in a
    /// runtime-loaded template, or <c>x:Name</c> is an XML error rather than a name.
    /// </remarks>
    public static ControlTemplate CardTemplate => Template;

    /// <summary>
    /// A ContentControl showing <paramref name="content"/> through
    /// <see cref="CardTemplate"/>, with no styling set here.
    /// </summary>
    public static ContentControl CreateCard(object content) => new()
    {
        Content = content,

        // The three TemplateBindings are the contract: Background, Padding and Content are
        // properties of the *control*, and the template decides which of its own elements
        // they land on. That is why restyling never has to touch the control's API - and
        // why a template that forgets to bind Content silently shows nothing.
        Template = CardTemplate,
    };
}
