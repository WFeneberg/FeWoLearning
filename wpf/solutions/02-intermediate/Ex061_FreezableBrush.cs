// Exercise 061 - Freezing a Freezable, and what freezing actually buys you (intermediate). REFERENCE SOLUTION.
// Goal:   An unfrozen Freezable (SolidColorBrush included) is a DispatcherObject with real
//         thread affinity: touching one - reading OR writing - from a thread other than the one
//         that created it throws. Freezing removes that affinity entirely, which is what makes a
//         frozen brush safe to hand to another thread at all. That negative (thread affinity,
//         and its removal) is this row's whole subject - reusing the SAME frozen brush across
//         many elements in a visual tree to cut allocations is a different story, told by row 077
//         (FrozenResources), not this one.
// Drills: Freezable.CanFreeze (false for a Freezable with an active animation or data binding -
//         Freeze() throws on one of those, it does not silently no-op), Freezable.Freeze() and
//         IsFrozen, and - measured directly, not assumed - that an unfrozen Freezable throws
//         InvalidOperationException the moment a different thread so much as READS one of its
//         properties, while the identical read from a FROZEN instance succeeds on any thread.
//         A plausible-looking bypass this row also rejects: calling Freeze() unconditionally and
//         swallowing whatever it throws ends up at the same IsFrozen outcome as genuinely
//         consulting CanFreeze first, so this row verifies CanFreeze is actually READ, not merely
//         implied by the result.

using System.Windows;
using System.Windows.Media;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex061_FreezableBrush
{
    /// <summary>
    /// Freezes <paramref name="freezable"/> if <see cref="System.Windows.Freezable.CanFreeze"/>
    /// allows it; otherwise leaves it exactly as it was. A caller must not assume this always
    /// succeeds - a Freezable with an active animation or data binding cannot freeze at all, and
    /// calling Freeze() unconditionally on one of those throws. Returns whether the object ends
    /// up frozen, so a caller can tell which happened without inspecting IsFrozen itself.
    /// </summary>
    public static bool FreezeIfPossible(Freezable freezable)
    {
        if (freezable.CanFreeze)
        {
            freezable.Freeze();
        }

        return freezable.IsFrozen;
    }

    /// <summary>
    /// Builds a new SolidColorBrush from <paramref name="color"/> and freezes it immediately - a
    /// plain brush with no bindings or animations attached can always freeze, so this never fails.
    /// </summary>
    public static SolidColorBrush CreateFrozenBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}
