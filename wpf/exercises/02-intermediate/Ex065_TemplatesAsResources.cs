// Exercise 065 - Templates as resources (intermediate).
// Goal:   Where the dictionary lives, and what a collision does. Row 041 already established the
//         one fact every implicit DataTemplate lookup needs -
//         the key is System.Windows.DataTemplateKey(type), never the bare Type - and used it to
//         drive a single ContentControl from one adjacent dictionary. This row does not re-teach
//         that; it does not even repeat row 041's ContentControl/live-Binding shape at all. What
//         is left, and what this row actually measures: the implicit lookup walks MULTIPLE levels
//         of ancestry, not just the immediate parent's Resources; a template reachable only
//         through an ancestor's MergedDictionaries is found exactly as if it were a direct entry
//         (the same merge-reachability row 026 already proved for styles, now confirmed for
//         templates too); and on a genuine collision - the SAME type templated at two different
//         ancestor levels - the NEARER ancestor wins, a completely different axis from row 026's
//         "last dictionary added to MergedDictionaries wins" rule, which only ever concerned
//         several dictionaries merged at the SAME level. And as row 023 already established for
//         implicit styles: this harness has no Application, so the real chain's final
//         Application.Current.Resources stop is simply absent here too - every dictionary in this
//         row's tests lives on an element somewhere in the tree, never anywhere else.
// Drills: registering a DataTemplate directly into a given ResourceDictionary, keyed by the
//         template's OWN DataType property (the same DataTemplateKey wrapper row 041 established -
//         not a separately-passed Type the way row 041's RegisterViewTemplate took one); and
//         registering a template inside a FRESH ResourceDictionary that is then merged into a
//         host's own MergedDictionaries - the shape templates usually ship in for real, reused
//         across an app - rather than written directly into the host's own dictionary. The
//         discriminator between the two, and against a mutant that quietly merges its own
//         dictionary in either way: ResourceDictionary.Keys (unlike Contains or the indexer) never
//         sees through MergedDictionaries - already the case row 026 rests on for the same reason.
// Passes: dotnet test --filter FullyQualifiedName~Ex065_

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex065_TemplatesAsResources
{
    /// <summary>
    /// Adds <paramref name="template"/> directly into <paramref name="resources"/>, keyed by the
    /// template's own DataType.
    /// </summary>
    public static void RegisterImplicit(ResourceDictionary resources, DataTemplate template)
        => throw new NotImplementedException("TODO: Ex065 - add template into resources itself, keyed by the wrapper built from template's own DataType (never the bare Type) - directly into the dictionary actually passed in, never a dictionary of your own making, and never merged in through MergedDictionaries either (that is RegisterInMergedDictionary's job, not this one's)");

    /// <summary>
    /// Builds a FRESH ResourceDictionary containing <paramref name="template"/> (keyed the same
    /// way as <see cref="RegisterImplicit"/>), then merges that new dictionary into
    /// <paramref name="hostResources"/>.MergedDictionaries - the "templates ship in their own
    /// dictionary, merged into a host" shape, not written directly into hostResources itself.
    /// </summary>
    public static void RegisterInMergedDictionary(ResourceDictionary hostResources, DataTemplate template)
        => throw new NotImplementedException("TODO: Ex065 - build a brand-new ResourceDictionary holding template, keyed the same way RegisterImplicit keys it; then merge that fresh dictionary into hostResources' own MergedDictionaries collection - the template must actually become reachable through hostResources this way, not merely exist somewhere nothing references");
}
