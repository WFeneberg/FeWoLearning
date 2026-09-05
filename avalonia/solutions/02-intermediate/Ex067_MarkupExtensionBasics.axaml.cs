using System.Collections.Generic;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex067_
public partial class Ex067_MarkupExtensionBasics : UserControl
{
    public Ex067_MarkupExtensionBasics() => InitializeComponent();
}

public class Ex067_Abbreviation
{
    public string Key { get; set; } = "";

    public object ProvideValue() => Ex067_Glossary.Expand(Key);
}

/// <summary>Given. Do not change.</summary>
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
