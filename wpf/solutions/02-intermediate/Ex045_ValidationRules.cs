// Exercise 045 - ValidationRule (intermediate). REFERENCE SOLUTION.
// Goal:   Validate a Binding's raw target-side text with a rule that lives entirely inside
//         WPF's own binding machinery - no INotifyDataErrorInfo, no IDataErrorInfo, and no
//         flag to remember to flip: a ValidationRule added to Binding.ValidationRules is
//         always active. Measured directly on this harness: with the rule attached and
//         UpdateSourceTrigger.PropertyChanged, typing an invalid value into the target both
//         surfaces Validation.HasError/Validation.GetErrors on the target AND stops the value
//         from ever reaching the source - the source keeps whatever it had, exactly like
//         row 017's UnsetValue "do not push" outcome, reached here by a completely different
//         mechanism.
// Drills: a ValidationRule subclass overriding Validate, wiring it into Binding.ValidationRules,
//         and Validation.GetErrors/Validation.HasError as what a rule's failure actually
//         produces on the target element.

using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>Rejects an empty or whitespace-only value.</summary>
public sealed class Ex045_NonEmptyValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var text = value as string;
        return string.IsNullOrWhiteSpace(text)
            ? new ValidationResult(false, "Required")
            : ValidationResult.ValidResult;
    }
}

public static class Ex045_ValidationRules
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text two-way to the property at
    /// <paramref name="propertyPath"/> on <paramref name="source"/>, PropertyChanged
    /// trigger, with a single Ex045_NonEmptyValidationRule attached to
    /// Binding.ValidationRules.
    /// </summary>
    public static void Bind(TextBox target, object source, string propertyPath)
    {
        target.DataContext = source;

        var binding = new Binding(propertyPath)
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        };
        binding.ValidationRules.Add(new Ex045_NonEmptyValidationRule());

        target.SetBinding(TextBox.TextProperty, binding);
    }
}
