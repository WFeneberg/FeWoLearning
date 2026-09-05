using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex067_MarkupExtensionBasicsTests
{
    private static Ex067_MarkupExtensionBasics Show()
    {
        var view = ViewHarness.Show(new Ex067_MarkupExtensionBasics(), 320, 120);
        Dispatcher.UIThread.RunJobs();
        return view;
    }

    private static string Text(Ex067_MarkupExtensionBasics view, string name) =>
        view.FindControl<TextBlock>(name)!.Text!;

    // The extension graded directly, with a key the glossary does not hold, so
    // the fallback branch is covered too.
    [AvaloniaFact]
    public void ProvideValue_Expands_The_Key_Through_The_Glossary()
    {
        Assert.Equal(
            "Model View ViewModel",
            new Ex067_Abbreviation { Key = "mvvm" }.ProvideValue());
        Assert.Equal("<nope?>", new Ex067_Abbreviation { Key = "nope" }.ProvideValue());
    }

    [AvaloniaFact]
    public void Both_Usages_Render_Their_Own_Expansion()
    {
        var view = Show();

        Assert.Equal("Model View ViewModel", Text(view, "Mvvm"));
        Assert.Equal("ObservableAsPropertyHelper", Text(view, "Oaph"));
    }

    // The discriminator. Two TextBlocks with hard-coded literals satisfy
    // everything above; they cannot follow the glossary being rewritten between
    // two constructions, because an extension is evaluated at parse time, which
    // is per instance. The entry is restored afterwards - the glossary is
    // process-wide static state, and the suite runs serially, so leaving it
    // rewritten would poison the test above depending on order.
    [AvaloniaFact]
    public void A_Later_View_Sees_A_Rewritten_Glossary()
    {
        var original = Ex067_Glossary.Entries["mvvm"];
        try
        {
            Assert.Equal(original, Text(Show(), "Mvvm"));

            Ex067_Glossary.Entries["mvvm"] = "Muster Voll Verdreht";

            Assert.Equal("Muster Voll Verdreht", Text(Show(), "Mvvm"));
            Assert.Equal("ObservableAsPropertyHelper", Text(Show(), "Oaph"));
        }
        finally
        {
            Ex067_Glossary.Entries["mvvm"] = original;
        }
    }
}
