using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex085_
public static class Ex085_VirtualizationBudget
{
    /// <summary>Given. Do not change.</summary>
    public static IReadOnlyList<string> Rows { get; } = BuildRows();

    private static string[] BuildRows()
    {
        var rows = new string[500];

        for (var i = 0; i < rows.Length; i++)
        {
            rows[i] = $"row {i}";
        }

        return rows;
    }

    // No ItemsPanel: a ListBox already virtualizes, and leaving its default alone
    // is the correct answer rather than a lazy one.
    public static ListBox BuildVirtualizing(double viewportHeight) =>
        new()
        {
            ItemsSource = Rows,
            Width = 180,
            Height = viewportHeight,
        };

    public static ListBox BuildNonVirtualizing(double viewportHeight) =>
        new()
        {
            ItemsSource = Rows,
            Width = 180,
            Height = viewportHeight,
            ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel()),
        };
}
