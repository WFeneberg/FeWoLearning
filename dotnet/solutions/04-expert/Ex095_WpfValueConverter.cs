using System.Globalization;

namespace FeWoLearning.Exercises.Expert;

// Exercise 095 — WPF-style value converter & binding (reference solution).
// Convert/ConvertBack are inverses of each other for the "normal" (non-
// inverted) direction; Hidden and Collapsed both collapse to false so the
// round trip bool -> Visibility -> bool is stable, while Visibility ->
// bool -> Visibility only round-trips exactly for Visible/Collapsed (Hidden
// is intentionally lossy, matching how a real bool-to-visibility binding
// behaves).
public enum Visibility
{
    Visible,
    Hidden,
    Collapsed,
}

public interface IValueConverter
{
    object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture);
}

public sealed class WpfValueConverter : IValueConverter
{
    public static readonly object DoNothing = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool flag)
            return DoNothing;

        if (IsInverted(parameter))
            flag = !flag;

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility)
            return DoNothing;

        var flag = visibility == Visibility.Visible;

        if (IsInverted(parameter))
            flag = !flag;

        return flag;
    }

    private static bool IsInverted(object? parameter)
        => parameter is string text && string.Equals(text, "Invert", StringComparison.OrdinalIgnoreCase);
}
