using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

/// <summary>A plain type that exists only to give Ex003 a category to name.</summary>
public sealed class OrderProcessor;

/// <summary>A generic type, to show what the category of a generic logger becomes.</summary>
/// <typeparam name="T">Ignored; only its presence matters.</typeparam>
public sealed class Repository<T>;

// Exercise 003 — CategoriesAndTypedLogger (logging).
// Goal:   Understand what a logger's CATEGORY actually is, because every filter rule,
//         every dashboard row and every alert selects on it.
// Drills: ILogger<T> and its category naming, CreateLogger(string), filtering by
//         category.
// Passes: CreateForType gives a logger whose category is the full name of
//                     OrderProcessor, namespace included;
//         CreateForArea("Billing") gives the category "FeWo.Areas.Billing";
//         CreateForGeneric gives the category of Repository<Order> WITHOUT its type
//                     argument - the generic arity is dropped entirely;
//         and a filter rule naming OrderProcessor's category silences the typed
//                     logger while the area logger keeps writing.
//
// The last clause is what separates a real category from a string stored somewhere:
// the filter has to select on it. The third is the surprise - a category is a display
// name, not a CLR type name, so two different closed generics share one category and
// therefore one filter rule.
public static class Ex003_CategoriesAndTypedLogger
{
    /// <summary>The prefix every area logger's category starts with.</summary>
    public const string AreaPrefix = "FeWo.Areas.";

    /// <summary>Create the logger a class would inject for itself.</summary>
    public static ILogger CreateForType(ILoggerFactory factory) =>
        throw new NotImplementedException(
            "TODO: Ex003 - create the logger whose category names OrderProcessor");

    /// <summary>
    /// Create a logger for a named functional area, categorised
    /// <see cref="AreaPrefix"/> followed by <paramref name="area"/>.
    /// </summary>
    public static ILogger CreateForArea(ILoggerFactory factory, string area) =>
        throw new NotImplementedException(
            "TODO: Ex003 - create a logger categorised by area name");

    /// <summary>Create the logger a <c>Repository&lt;OrderProcessor&gt;</c> would inject.</summary>
    public static ILogger CreateForGeneric(ILoggerFactory factory) =>
        throw new NotImplementedException(
            "TODO: Ex003 - create the logger whose category names Repository<OrderProcessor>");

    /// <summary>Write one Information record reading "started". Used by every fact.</summary>
    public static void LogStarted(ILogger logger) =>
        throw new NotImplementedException("TODO: Ex003 - write one Information record reading \"started\"");
}
