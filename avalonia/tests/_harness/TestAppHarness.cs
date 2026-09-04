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
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
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
