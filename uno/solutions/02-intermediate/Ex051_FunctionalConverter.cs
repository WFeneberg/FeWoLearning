// Exercise 051 - Functional Converter (intermediate).
// Goal:   Stop writing one IValueConverter class per formatting rule.
// Drills: a generic converter parameterised by delegates, typed guards at the boundary,
//         and DependencyProperty.UnsetValue for input that does not fit.
// Passes: dotnet test --filter FullyQualifiedName~Ex051_
//
// Every app accumulates a dozen converters that differ by one expression. One generic
// converter plus a factory method turns those into one line each - and the type parameters
// are what move the "is this the right kind of value" check from every implementation into
// this single place.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Converts <typeparamref name="TFrom"/> to <typeparamref name="TTo"/> with the delegate it
/// was built with, and back again when a reverse delegate was supplied.
/// </summary>
public sealed class Ex051_FunctionalConverter<TFrom, TTo> : IValueConverter
{
    private readonly Func<TFrom, TTo> _forward;
    private readonly Func<TTo, TFrom>? _backward;

    public Ex051_FunctionalConverter(Func<TFrom, TTo> forward, Func<TTo, TFrom>? backward = null)
    {
        _forward = forward;
        _backward = backward;
    }

    /// <summary>
    /// Applies the forward delegate. Anything that is not a <typeparamref name="TFrom"/> -
    /// including null when the type cannot hold it - returns
    /// <see cref="DependencyProperty.UnsetValue"/> rather than throwing: a binding that
    /// briefly sees the wrong type must not take the app down.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, string language) =>
        // The pattern match is the type guard: for TFrom = int it rejects null and strings
        // alike, for TFrom = string? it lets null through. One check, written once, instead
        // of once per converter.
        value is TFrom typed ? _forward(typed)! : Guarded(value);

    /// <summary>
    /// Applies the reverse delegate, or returns <see cref="DependencyProperty.UnsetValue"/>
    /// when there is none - a one-way converter says "no answer", it does not invent one.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        _backward is not null && value is TTo typed
            ? _backward(typed)!
            // A one-way converter says "no answer". Returning the input unchanged, or
            // default(TFrom), would write a made-up value back into the source.
            : DependencyProperty.UnsetValue;

    /// <summary>
    /// Handles the one case the pattern match above cannot: a nullable TFrom, where null is
    /// a legitimate input but `value is TFrom` is false for it.
    /// </summary>
    private object Guarded(object value) =>
        value is null && default(TFrom) is null
            ? _forward(default!)!
            : DependencyProperty.UnsetValue;
}

/// <summary>Factory helpers, so a call site reads as one expression.</summary>
public static class Ex051_Convert
{
    /// <summary>A one-way converter from <paramref name="forward"/>.</summary>
    public static IValueConverter OneWay<TFrom, TTo>(Func<TFrom, TTo> forward) =>
        new Ex051_FunctionalConverter<TFrom, TTo>(forward);

    /// <summary>A two-way converter from both directions.</summary>
    public static IValueConverter TwoWay<TFrom, TTo>(Func<TFrom, TTo> forward, Func<TTo, TFrom> backward) =>
        new Ex051_FunctionalConverter<TFrom, TTo>(forward, backward);
}
