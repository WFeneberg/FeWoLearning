using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex099_
public static class Ex099_CustomHeadlessTestHarness
{
    public static AppBuilder Build(
        Action installWindowing,
        string windowingName,
        Action installRendering,
        string renderingName) =>
        AppBuilder.Configure<Ex099_TestApplication>()
            .UseWindowingSubsystem(installWindowing, windowingName)
            .UseRenderingSubsystem(installRendering, renderingName);
}

public class Ex099_TestApplication : Application
{
    /// <summary>Given. Do not change.</summary>
    public static Style MinimalTheme { get; } = new(x => x.OfType<ContentControl>());

    /// <summary>Given. Do not change.</summary>
    public bool ThemeWasAdded => Styles.Count > 0;

    // Without this every control's Template is null.
    public override void Initialize() => Styles.Add(MinimalTheme);
}
