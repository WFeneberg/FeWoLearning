using Avalonia;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex062_
public class Ex062_AttachedPropertyAuthoring : AvaloniaObject
{
    public static readonly AttachedProperty<int> BadgeCountProperty =
        AvaloniaProperty.RegisterAttached<Ex062_AttachedPropertyAuthoring, Control, int>(
            "BadgeCount");

    static Ex062_AttachedPropertyAuthoring() =>
        BadgeCountProperty.Changed.AddClassHandler<Control, int>((target, e) =>
        {
            var count = e.NewValue.GetValueOrDefault();
            ToolTip.SetTip(target, count > 0 ? $"{count} items" : null);
        });

    public static int GetBadgeCount(Control target) => target.GetValue(BadgeCountProperty);

    public static void SetBadgeCount(Control target, int value) =>
        target.SetValue(BadgeCountProperty, value);
}
