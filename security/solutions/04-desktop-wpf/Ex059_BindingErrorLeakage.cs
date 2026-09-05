using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 059 — BindingErrorLeakage (reference solution).
public static class Ex059_BindingErrorLeakage
{
    public static void Bind(TextBlock target, object source, string path, string fallback)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(fallback);

        target.ToolTip = null;

        var binding = new Binding(path)
        {
            Source = source,
            Mode = BindingMode.OneWay,
            FallbackValue = fallback,
        };

        BindingOperations.SetBinding(target, TextBlock.TextProperty, binding);
    }
}
