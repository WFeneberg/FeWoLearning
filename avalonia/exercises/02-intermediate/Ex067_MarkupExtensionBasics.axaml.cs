using System.Collections.Generic;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex067_
public partial class Ex067_MarkupExtensionBasics : UserControl
{
    public Ex067_MarkupExtensionBasics()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex067 - use Ex067_Abbreviation twice, on TextBlocks named Mvvm and " +
            "Oaph, with Key=mvvm and Key=oaph");
    }
}

/// <summary>
/// The markup extension. No base class and no interface: a public ProvideValue is
/// the whole contract.
/// </summary>
public class Ex067_Abbreviation
{
    public string Key { get; set; } = "";

    public object ProvideValue() =>
        throw new NotImplementedException(
            "TODO: Ex067 - return Ex067_Glossary.Expand(Key)");
}

/// <summary>
/// Given. Do not change. A mutable lookup, so that what the extension produces
/// depends on run-time state rather than on anything a literal could copy.
/// </summary>
public static class Ex067_Glossary
{
    public static Dictionary<string, string> Entries { get; } = new()
    {
        ["mvvm"] = "Model View ViewModel",
        ["oaph"] = "ObservableAsPropertyHelper",
    };

    public static string Expand(string key) =>
        Entries.TryGetValue(key, out var value) ? value : $"<{key}?>";
}
