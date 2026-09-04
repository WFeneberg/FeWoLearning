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
        throw new NotImplementedException("TODO: Ex051 - apply the forward delegate");

    /// <summary>
    /// Applies the reverse delegate, or returns <see cref="DependencyProperty.UnsetValue"/>
    /// when there is none - a one-way converter says "no answer", it does not invent one.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException("TODO: Ex051 - apply the reverse delegate if there is one");
}

/// <summary>Factory helpers, so a call site reads as one expression.</summary>
public static class Ex051_Convert
{
    /// <summary>A one-way converter from <paramref name="forward"/>.</summary>
    public static IValueConverter OneWay<TFrom, TTo>(Func<TFrom, TTo> forward) =>
        throw new NotImplementedException("TODO: Ex051 - build a one-way converter");

    /// <summary>A two-way converter from both directions.</summary>
    public static IValueConverter TwoWay<TFrom, TTo>(Func<TFrom, TTo> forward, Func<TTo, TFrom> backward) =>
        throw new NotImplementedException("TODO: Ex051 - build a two-way converter");
}
