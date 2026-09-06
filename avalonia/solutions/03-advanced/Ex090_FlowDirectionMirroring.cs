using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex090_
public static class Ex090_FlowDirectionMirroring
{
    /// <summary>Given. Do not change.</summary>
    public const double LabelWidth = 120;

    /// <summary>Given. Do not change.</summary>
    public const string LabelText = "abc";

    public static Control BuildHost(FlowDirection direction) =>
        new StackPanel
        {
            FlowDirection = direction,
            Children = { Label("Label") },
        };

    public static Control BuildMixedHost()
    {
        var optedOut = Label("OptedOut");

        // An explicit local value outranks the inherited one, which is how a URL or
        // a serial number stays readable on a mirrored page.
        optedOut.FlowDirection = FlowDirection.LeftToRight;

        return new StackPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Children = { Label("Inherited"), optedOut },
        };
    }

    private static TextBlock Label(string name) =>
        new()
        {
            Name = name,
            Text = LabelText,
            Width = LabelWidth,
        };
}
