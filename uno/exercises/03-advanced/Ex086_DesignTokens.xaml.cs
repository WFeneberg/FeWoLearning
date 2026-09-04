// Exercise 086 - Design Tokens (advanced).
// Goal:   Layer a theme so that a consumer names one thing and gets the right answer.
// Drills: primitive tokens in ThemeDictionaries, semantic styles outside them, BasedOn
//         across the two layers, and merging the dictionary into a scope.
// Passes: dotnet test --filter FullyQualifiedName~Ex086_
//
// The layering is what makes a design system survivable: primitives (a colour, a spacing
// step) are per-theme, semantics (what a card looks like) are not, and application code
// references only semantics. Skip the split and every screen ends up naming colours - and
// then a rebrand is a search-and-replace across the app.
//
// A ResourceDictionary with x:Class is a normal type: `new Ex086_DesignTokens()` and merge
// it wherever it is needed.

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Advanced;

public sealed partial class Ex086_DesignTokens : ResourceDictionary
{
    public Ex086_DesignTokens() => InitializeComponent();

    /// <summary>
    /// Merges a fresh token dictionary into <paramref name="scope"/>'s resources, so
    /// everything below it can name the semantic styles.
    /// </summary>
    public static void MergeInto(FrameworkElement scope) =>
        throw new NotImplementedException("TODO: Ex086 - merge the tokens into the scope");
}
