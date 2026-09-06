using System.Collections.Generic;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex096_
public class Ex096_MultiWindowLifetime
{
    /// <summary>Given. Do not change.</summary>
    public Window Shell { get; } = new() { Width = 200, Height = 140 };

    /// <summary>Given. Do not change.</summary>
    public List<string> CloseAttempts { get; } = [];

    /// <summary>Given. Do not change.</summary>
    public bool Confirmed { get; set; }

    public Window? Tool { get; protected set; }

    public void Open()
    {
        var tool = new Window { Width = 120, Height = 90 };

        tool.Closing += (_, e) =>
        {
            CloseAttempts.Add("attempt");

            // Cancelling here is the whole "are you sure?" mechanism: the window
            // stays visible and stays owned.
            e.Cancel = !Confirmed;
        };

        Tool = tool;

        // The owner overload is what makes it a child rather than a second
        // top-level window.
        tool.Show(Shell);
    }

    public void RequestClose() => Tool?.Close();
}
