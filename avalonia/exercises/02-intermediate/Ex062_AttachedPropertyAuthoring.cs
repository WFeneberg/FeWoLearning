using Avalonia;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 062 - AttachedPropertyAuthoring (intermediate).
/// Goal:   Register your OWN attached property - the mechanism that lets one type
///         decorate controls it does not own - and give it a change handler, so
///         BadgeCount can be set on any Control and the target's ToolTip.Tip
///         follows. ex034 consumed somebody else's attached property; this one is
///         about authoring one.
/// Drills: AvaloniaProperty.RegisterAttached, AttachedProperty<T>, the static
///         Get/Set accessor pair, AvaloniaProperty.Changed with AddClassHandler,
///         and the static constructor that wires it up.
///
/// The obvious wrong answer is a static Dictionary<Control, int> behind
/// GetBadgeCount/SetBadgeCount. It round-trips values and it even updates the
/// tooltip, so the tests deliberately cross the two APIs: they write with
/// SetValue and read with GetBadgeCount, then write with SetBadgeCount and read
/// with GetValue. A side table satisfies neither direction, and work performed
/// inside SetBadgeCount rather than in a change handler is bypassed entirely by
/// the SetValue path.
///
/// NOTE ON THE STUB SHAPE: BadgeCountProperty is a throwing PROPERTY here purely
/// so the untouched stub fails with NotImplementedException. A static readonly
/// FIELD whose initializer throws would surface as TypeInitializationException
/// instead, which hides the cause. Your solution should make it the idiomatic
/// `public static readonly AttachedProperty<int> BadgeCountProperty = ...` field -
/// the tests bind to the name, not to the member kind.
/// Passes: dotnet test --filter FullyQualifiedName~Ex062_
public class Ex062_AttachedPropertyAuthoring : AvaloniaObject
{
    public static AttachedProperty<int> BadgeCountProperty =>
        throw new NotImplementedException(
            "TODO: Ex062 - register an attached property named \"BadgeCount\" of type " +
            "int, owned by Ex062_AttachedPropertyAuthoring and attachable to Control, " +
            "with AvaloniaProperty.RegisterAttached");

    public static int GetBadgeCount(Control target) =>
        throw new NotImplementedException(
            "TODO: Ex062 - read BadgeCountProperty off the target through the property " +
            "system (target.GetValue), not out of a side table");

    public static void SetBadgeCount(Control target, int value) =>
        throw new NotImplementedException(
            "TODO: Ex062 - write BadgeCountProperty onto the target through the property " +
            "system (target.SetValue), and do NOT update the tooltip from here - that " +
            "belongs in the change handler, or the SetValue path bypasses it");

    // TODO: Ex062 - add a static constructor that subscribes to
    // BadgeCountProperty.Changed with AddClassHandler<Control, int>, setting the
    // target's ToolTip.Tip to $"{count} items" for a positive count and to null
    // for a count of zero.
}
