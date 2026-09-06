using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Themes.Fluent;
using ReactiveUI.Avalonia;
using ReactiveUI.Builder;

[assembly: AvaloniaTestApplication(typeof(FeWoLearning.Avalonia.Tests.TestAppBuilder))]

namespace FeWoLearning.Avalonia.Tests;

/// <summary>
/// The Application every [AvaloniaFact] runs inside. FluentTheme is added in code
/// rather than in an App.axaml, because the test project needs no XAML of its own.
/// </summary>
public class TestApp : Application
{
    public override void Initialize() => Styles.Add(new FluentTheme());
}

public static class TestAppBuilder
{
    /// <summary>
    /// Headless, but with a REAL drawing backend rather than the null one.
    ///
    /// UseHeadlessDrawing = false plus .UseSkia() is what
    /// TopLevel.GetLastRenderedFrame demands - measured, it refuses with
    /// NotSupportedException naming exactly these two otherwise - so ex098 cannot
    /// exist without them. It also means draw commands are really executed, which
    /// is why the rendering section of README.md had to be rewritten: pixels are
    /// now readable instead of being noise.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}

/// <summary>
/// ReactiveUI 24 does NOT self-initialize. Without this, the first WhenAnyValue in
/// any exercise throws TypeInitializationException -> InvalidOperationException
/// ("ReactiveUI has not been initialized"), and every exercise goes red for the
/// wrong reason, silently destroying the red/green invariant.
/// </summary>
internal static class ReactiveUiInitializer
{
    [ModuleInitializer]
    internal static void Init() =>
        RxAppBuilder.CreateReactiveUIBuilder().WithAvalonia().Build();
}
