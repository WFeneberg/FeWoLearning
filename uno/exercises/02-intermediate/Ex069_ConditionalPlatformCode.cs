// Exercise 069 - Conditional Platform Code (intermediate).
// Goal:   Keep per-platform code at the edge, and the logic that uses it testable.
// Drills: an interface as the platform seam, one implementation per platform behind a
//         factory, and the difference between "#if in the middle of a method" and "#if
//         around a whole file".
// Passes: dotnet test --filter FullyQualifiedName~Ex069_
//
// Uno's usual pattern is a partial class with one file per platform, selected by the build.
// The reason is not aesthetics: a method with #if branches inside it compiles on one
// platform at a time, so most of it is never even type-checked by your local build, and
// none of it is testable. One interface later, the platform-specific part is a handful of
// lines and everything above it runs everywhere - including in a test.

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>What the app needs to know about where it is running.</summary>
public interface IEx069_PlatformInfo
{
    /// <summary>A short platform name, e.g. "windows", "android", "browser".</summary>
    string Name { get; }

    /// <summary>Whether the platform can open a second window.</summary>
    bool SupportsMultipleWindows { get; }
}

/// <summary>
/// The logic that depends on the platform without knowing which one it is on.
/// </summary>
public sealed class Ex069_ConditionalPlatformCode
{
    private readonly IEx069_PlatformInfo _platform;

    public Ex069_ConditionalPlatformCode(IEx069_PlatformInfo platform) => _platform = platform;

    /// <summary>
    /// Where a newly opened document should go: <c>"window"</c> when the platform can open
    /// another window, <c>"tab"</c> otherwise.
    /// </summary>
    public string OpenDocumentTarget =>
        throw new NotImplementedException("TODO: Ex069 - choose a target for the platform");

    /// <summary>
    /// A telemetry tag: the platform name, lower-cased, with the window capability appended
    /// as "+multiwindow" when it is there. E.g. "windows+multiwindow", "android".
    /// </summary>
    public string TelemetryTag =>
        throw new NotImplementedException("TODO: Ex069 - build the telemetry tag");
}

/// <summary>
/// The seam's only platform-specific member. In a real project the body would live in one
/// file per platform (PlatformInfo.windows.cs, PlatformInfo.android.cs, ...) and this
/// method would not exist; the conditional is here so the exercise stays in one file.
/// </summary>
public static class Ex069_PlatformInfoFactory
{
    /// <summary>The platform this build is for.</summary>
    public static IEx069_PlatformInfo Current =>
#if WINDOWS || HAS_UNO_SKIA
        Desktop;
#else
        Mobile;
#endif

    /// <summary>Windows and the desktop Skia heads: several windows, "desktop".</summary>
    public static IEx069_PlatformInfo Desktop { get; } = new StaticPlatformInfo("desktop", supportsMultipleWindows: true);

    /// <summary>Phones and tablets: one window, "mobile".</summary>
    public static IEx069_PlatformInfo Mobile { get; } = new StaticPlatformInfo("mobile", supportsMultipleWindows: false);

    private sealed class StaticPlatformInfo(string name, bool supportsMultipleWindows) : IEx069_PlatformInfo
    {
        public string Name { get; } = name;

        public bool SupportsMultipleWindows { get; } = supportsMultipleWindows;
    }
}
