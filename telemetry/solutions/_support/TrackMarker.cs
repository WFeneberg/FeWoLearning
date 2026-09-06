namespace FeWoLearning.Telemetry.Exercises.Support;

/// <summary>
/// A permanent, deliberately trivial public type that exists so the harness can
/// force the content library to load and then check which one it got.
///
/// It cannot be internal and it cannot be unused: a referenced assembly is loaded
/// lazily, so a canary that only walks <see cref="AppDomain.GetAssemblies"/> without
/// touching a type from the library finds an empty collection and fails for a reason
/// that has nothing to do with the track. Reading <see cref="TrackName"/> is what
/// makes the check honest.
///
/// This is a <c>_support/</c> fixture: never a TODO, never a catalog.md row.
/// </summary>
public static class TrackMarker
{
    /// <summary>
    /// The track this content library belongs to.
    ///
    /// Deliberately <c>static readonly</c> and NOT <c>const</c>. A const is baked
    /// into the call site by the compiler, so reading it never touches this assembly
    /// and never triggers the load - which is exactly the failure this type exists to
    /// prevent. Measured 2026-09-06: with a const, the canary still saw an empty
    /// collection.
    /// </summary>
    public static readonly string TrackName = "telemetry";
}
