using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 099 - CustomHeadlessTestHarness (expert).
/// Goal:   Compose the AppBuilder a headless test suite needs and understand every
///         piece of it - because this track's own harness is exactly this shape and
///         nothing in it is decoration.
/// Drills: AppBuilder.Configure, UseWindowingSubsystem and UseRenderingSubsystem,
///         reading a builder's configuration back, Application.Styles.
/// Passes: dotnet test --filter FullyQualifiedName~Ex099_
///
/// WHY THE SUBSYSTEMS ARE PASSED IN RATHER THAN NAMED. The real installers live in
/// Avalonia.Headless and Avalonia.Skia, and those belong to a TEST project, not to
/// a library of application code - which is the architectural point of the row as
/// much as the builder is. So Build takes them as delegates, exactly the way
/// UseHeadless and UseSkia hand them to the builder underneath, and a caller that
/// has those packages supplies the real thing.
///
/// For the record, what the real harness passes and why each matters:
///   - a HEADLESS windowing subsystem: windows without a window manager;
///   - a REAL rendering subsystem, Skia, with UseHeadlessDrawing turned off. Leave
///     the drawing headless and CaptureRenderedFrame refuses outright - that is
///     ex098's whole subject;
///   - a theme in Application.Styles, or every control's Template is null and half
///     this track's assertions become meaningless;
///   - and ReactiveUI initialised SEPARATELY, through RxAppBuilder in a
///     ModuleInitializer. It is deliberately not part of the AppBuilder, which is
///     worth knowing so you look in the right place when WhenAnyValue throws.
///
/// YOU MUST NOT START IT. A builder can be composed inside a process that already
/// has an Application - measured, Configure works fine here - but Start or
/// SetupWithoutStarting would fight the app the test runs in. Compose it and hand
/// it back; the test reads it.
public static class Ex099_CustomHeadlessTestHarness
{
    /// <summary>
    /// A builder for an <see cref="Ex099_TestApplication"/> with the two given
    /// subsystems installed under the given names, composed and NOT started.
    /// </summary>
    public static AppBuilder Build(
        Action installWindowing,
        string windowingName,
        Action installRendering,
        string renderingName) =>
        throw new NotImplementedException(
            "TODO: Ex099 - AppBuilder.Configure<Ex099_TestApplication>(), then " +
            "UseWindowingSubsystem(installWindowing, windowingName) and " +
            "UseRenderingSubsystem(installRendering, renderingName). Do not start it");
}

/// <summary>
/// The Application the builder configures. Its Initialize has to add a theme, or
/// nothing in the suite gets a control template.
/// </summary>
public class Ex099_TestApplication : Application
{
    /// <summary>
    /// Given. Do not change. Stands in for the FluentTheme the real harness adds -
    /// a theme package is a test-project dependency, and this exercise stays in
    /// application code.
    /// </summary>
    public static Style MinimalTheme { get; } = new(x => x.OfType<ContentControl>());

    /// <summary>Given. Do not change.</summary>
    public bool ThemeWasAdded => Styles.Count > 0;

    public override void Initialize() =>
        throw new NotImplementedException(
            "TODO: Ex099 - add MinimalTheme to Styles. An Application with no styles " +
            "gives every control a null Template, which is the failure this line " +
            "prevents");
}
