// Exercise 026 - Control Template Basics (beginner).
// Goal:   Replace a control's whole appearance without touching the control.
// Drills: ControlTemplate, TemplateBinding from a template child back to the templated
//         parent, and ContentPresenter as the hole the content goes into.
// Passes: dotnet test --filter FullyQualifiedName~Ex026_
//
// A DataTemplate says how *data* looks; a ControlTemplate says how a *control* looks. The
// control keeps its behaviour and its properties either way - a Button with a new template
// is still a Button that clicks.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex026_ControlTemplateBasics
{
    /// <summary>
    /// A template for a <see cref="ContentControl"/>: a Border named "PART_Root" whose
    /// Background and Padding follow the templated parent's, holding a
    /// <see cref="ContentPresenter"/> whose Content follows too. Parsed once and cached.
    /// </summary>
    /// <remarks>
    /// Both the default namespace and the <c>x</c> namespace have to be declared in a
    /// runtime-loaded template, or <c>x:Name</c> is an XML error rather than a name.
    /// </remarks>
    public static ControlTemplate CardTemplate =>
        // TODO: parse a ControlTemplate with TargetType="ContentControl" whose content is
        //   <Border x:Name="PART_Root" Background="{TemplateBinding Background}"
        //           Padding="{TemplateBinding Padding}">
        //     <ContentPresenter Content="{TemplateBinding Content}" />
        //   </Border>
        // and cache it in a static field.
        throw new NotImplementedException("TODO: Ex026 - parse and cache the card template");

    /// <summary>
    /// A ContentControl showing <paramref name="content"/> through
    /// <see cref="CardTemplate"/>, with no styling set here.
    /// </summary>
    public static ContentControl CreateCard(object content) =>
        throw new NotImplementedException("TODO: Ex026 - apply the template to a content control");
}
