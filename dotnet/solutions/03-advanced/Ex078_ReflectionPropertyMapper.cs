using System.Reflection;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 078 — Reflection-based property mapper (reference solution).
// Reads all public instance properties of TSource and, for each one whose
// name matches a public, writable instance property on TTarget with an
// assignable type, copies the value across via reflection.
public static class ReflectionPropertyMapper
{
    public static TTarget Map<TSource, TTarget>(TSource source)
        where TTarget : new()
    {
        var target = new TTarget();
        Map(source, target);
        return target;
    }

    public static void Map<TSource, TTarget>(TSource source, TTarget target)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (target is null) throw new ArgumentNullException(nameof(target));

        var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.Ordinal);

        foreach (var sourceProp in sourceProps)
        {
            if (!sourceProp.CanRead) continue;
            if (!targetProps.TryGetValue(sourceProp.Name, out var targetProp)) continue;
            if (!targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType)) continue;

            var value = sourceProp.GetValue(source);
            targetProp.SetValue(target, value);
        }
    }
}
