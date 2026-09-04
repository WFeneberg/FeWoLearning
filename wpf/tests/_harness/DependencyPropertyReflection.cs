using System.Reflection;
using System.Windows;

namespace FeWoLearning.Wpf.Tests;

/// <summary>
/// Shared reflection helpers for reaching a dependency-property registration that a stub
/// does not define yet, so a red run fails on the TODO rather than on a compile error.
/// ex001 and ex002 each wrote their own private copy of the "public static readonly
/// DependencyProperty field" shape below - a private static property in one test class, a
/// private static method in the other. Row 006 (`RegisterReadOnly`, `DependencyPropertyKey`)
/// needs a third shape: the private key a read-only property's owner keeps to itself. This
/// is where that shape - and anything after it that needs the same kind of reflection -
/// lives, instead of a fourth private copy.
/// </summary>
public static class DependencyPropertyReflection
{
    /// <summary>
    /// Reads a public static readonly <see cref="DependencyProperty"/> field named
    /// <paramref name="fieldName"/> off <paramref name="ownerType"/>. Takes a
    /// <see cref="Type"/> rather than a type parameter so a static attached-property host
    /// (ex007, ex008 - C# forbids a static class as a generic type argument) works the same
    /// way as an ordinary owner class.
    /// </summary>
    public static DependencyProperty Property(Type ownerType, string fieldName)
    {
        var field = ownerType.GetField(
            fieldName,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        Assert.NotNull(field);
        Assert.True(field!.IsInitOnly, $"{fieldName} must be readonly - the field is the property's identity.");
        Assert.Equal(typeof(DependencyProperty), field.FieldType);

        var value = field.GetValue(null) as DependencyProperty;
        Assert.NotNull(value);
        return value!;
    }

    /// <summary>
    /// Reads a static readonly <see cref="DependencyPropertyKey"/> field named
    /// <paramref name="fieldName"/> off <paramref name="ownerType"/> - the handle a
    /// read-only property's owner keeps to itself so only it can write through
    /// <c>SetValue</c>. Looks at non-public members too: the whole point of a read-only
    /// property is that the key is private, and a test still has to reach it to prove the
    /// write side works at all.
    /// </summary>
    public static DependencyPropertyKey Key(Type ownerType, string fieldName)
    {
        var field = ownerType.GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        Assert.NotNull(field);
        Assert.True(field!.IsInitOnly, $"{fieldName} must be readonly - the key is the property's only write capability.");
        Assert.Equal(typeof(DependencyPropertyKey), field.FieldType);

        var value = field.GetValue(null) as DependencyPropertyKey;
        Assert.NotNull(value);
        return value!;
    }
}
