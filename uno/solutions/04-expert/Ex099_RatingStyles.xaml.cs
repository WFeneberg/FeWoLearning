// Exercise 099 - Capstone Control (expert), the shipped styles.
// See Ex099_CapstoneControl.cs for the exercise notes.

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Expert;

public sealed partial class Ex099_RatingStyles : ResourceDictionary
{
    public Ex099_RatingStyles() => InitializeComponent();

    /// <summary>Merges the styles into a scope, as a consumer's App.xaml would.</summary>
    public static void MergeInto(FrameworkElement scope) =>
        scope.Resources.MergedDictionaries.Add(new Ex099_RatingStyles());
}
