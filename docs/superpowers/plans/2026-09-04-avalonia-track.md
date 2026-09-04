# Avalonia Track Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Stand up a tenth learning track, `avalonia/`, teaching Avalonia 12 desktop UI with ReactiveUI MVVM — scaffolding, a 100-row catalog, and the first ten exercises verified genuinely red and genuinely green.

**Architecture:** Four projects under `avalonia/`. `exercises/` holds stubs, `solutions/` holds reference implementations at mirrored paths using the *same* namespaces and type names, and `tests/` plus `gallery/` each reference **exactly one** of the two, selected by the MSBuild property `UseSolutions`. That is what keeps the identical type names from colliding, and it makes the green check one command instead of a scratchpad overlay. Tests run under `Avalonia.Headless.XUnit` on xunit.v3.

**Tech Stack:** .NET 10 (SDK 10.0.400), Avalonia 12.1.1, ReactiveUI 24.1.0 via `ReactiveUI.Avalonia` 12.1.1, xunit.v3 3.2.2, `Avalonia.Headless.XUnit` 12.1.1.

**Spec:** [`docs/superpowers/specs/2026-09-04-avalonia-track-design.md`](../specs/2026-09-04-avalonia-track-design.md) — read it before Task 1. Sections 2.1 and 7 are the ones that will otherwise cost you a day.

## Global Constraints

Every task's requirements implicitly include this section. All values are copied verbatim from the spec.

- **All commands run from inside `avalonia/`**, never the repo root.
- **Package versions are pinned and coherent at 12.1.1**: `Avalonia`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`, `Avalonia.Desktop`, `Avalonia.Headless.XUnit`, `ReactiveUI.Avalonia` — all `12.1.1`. Plus `xunit.v3` `3.2.2`, `xunit.runner.visualstudio` `3.1.4`, `Microsoft.NET.Test.Sdk` `17.14.1`. Do not bump anything to Avalonia 12.1.2 or `ReactiveUI.Avalonia` 14.7.1; 12.1.1 is the verified set.
- **Never reference `xunit` 2.x.** `Avalonia.Headless.XUnit` depends on `xunit.v3.extensibility.core`; adding `xunit` 2.9.3 puts `FactAttribute` in two assemblies and every test file fails with CS0433. The Avalonia track cannot copy `blazor/`'s xunit 2.9.3 setup.
- **The attribute is `[AvaloniaFact]` / `[AvaloniaTheory]`**, not `[AvaloniaTest]`.
- **`ReactiveUI.Primitives.RxVoid` replaces `System.Reactive.Unit`.** ReactiveUI 24 has no `Unit` type.
- **Rx operators come from `using ReactiveUI.Primitives;`** (`LinqExtensions`), not `System.Reactive.Linq`. There is no `System.Reactive` dependency in this track.
- **ReactiveUI must be initialized** with `RxAppBuilder.CreateReactiveUIBuilder().WithAvalonia().Build()`, once per process. Without it the first `WhenAnyValue` anywhere throws `TypeInitializationException` → `InvalidOperationException: ReactiveUI has not been initialized`, and *every* exercise goes red for the wrong reason.
- **Layout assertions require a `Window` that has been `Show()`n.** `Measure`/`Arrange` alone leaves every child at `0,0,0,0` — measured, not guessed — and `ApplyTemplate()` does not fix it. A headless window's client area equals its requested `Width`/`Height` exactly.
- **Tier namespaces are pinned**: `FeWoLearning.Avalonia.Exercises.Beginner`, `.Intermediate`, `.Advanced`, `.Expert`. They do not follow the `NN-tier` folder names, because a C# identifier cannot start with a digit. Every `.axaml` states its `x:Class` fully qualified.
- **Stubs must compile and fail at runtime**, throwing `NotImplementedException("TODO: ExNNN – …")`. A stub that breaks the build or the XAML compiler is a bug.
- **Never assert `Assert.Throws<NotImplementedException>`** in an exercise test, and never assert an error the signature alone produces. Either passes against the untouched stub.
- **Commit one batch of five at a time**, message `avalonia: exNNN-exNNN`, staging explicit paths. Never `git add -A` — it has swept up unrelated files in this repo before.

---

## File Structure

**Task 1 — scaffolding and harness**

| File | Responsibility |
|---|---|
| `avalonia/Directory.Build.props` | Redirect the solutions build to `artifacts-solutions/` when `UseSolutions=true` |
| `avalonia/FeWoLearning.Avalonia.slnx` | Solution, all four projects |
| `avalonia/exercises/FeWoLearning.Avalonia.Exercises.csproj` | Stub content library |
| `avalonia/solutions/FeWoLearning.Avalonia.Solutions.csproj` | Reference content library, own `AssemblyName` |
| `avalonia/tests/FeWoLearning.Avalonia.Tests.csproj` | Headless test project, conditional content reference |
| `avalonia/tests/_harness/TestAppHarness.cs` | `TestApp`, `TestAppBuilder`, ReactiveUI module initializer, `[assembly: AvaloniaTestApplication]` |
| `avalonia/tests/_harness/ViewHarness.cs` | The one verified `Show` helper every view test uses |
| `avalonia/tests/_harness/HarnessSmokeTests.cs` | Permanent regression test for the two harness prerequisites |
| `avalonia/gallery/FeWoLearning.Avalonia.Gallery.csproj` | Desktop gallery app |
| `avalonia/gallery/Program.cs` | Entry point + `AppBuilder` incl. ReactiveUI init |
| `avalonia/gallery/App.axaml` + `.axaml.cs` | FluentTheme, main window |
| `avalonia/gallery/MainWindow.axaml` + `.axaml.cs` | Navigation list + content host |
| `avalonia/gallery/GalleryEntry.cs` | `GalleryEntry` record |
| `avalonia/gallery/GalleryCatalog.cs` | The registry — one entry per visual exercise |
| `avalonia/.gitignore` | Keep `artifacts-solutions/`, `bin/`, `obj/` out of git |

**Task 2 — ledger and docs**: `avalonia/catalog.md`, `avalonia/README.md`.

**Tasks 3–4 — exercises.** Per exercise `NNN` with slug `S`, in tier folder `T`:

| File | Responsibility |
|---|---|
| `exercises/T/ExNNN_S.axaml` + `.axaml.cs` | Stub (view exercises) |
| `exercises/T/ExNNN_S.cs` | Stub (view-model-only exercises: ex008, ex009) |
| `tests/T/ExNNN_STests.cs` | The exercise's test — identical in both modes |
| `solutions/T/ExNNN_S.axaml` + `.axaml.cs` (or `.cs`) | Reference implementation |
| `gallery/Pages/Beginner/ExNNN.axaml` + `.axaml.cs` | Only where the result is visual |

**Task 5 — repo registration**: `CLAUDE.md`, `README.md`, `docs/exercise-format.md`.

---

## Task 1: Scaffolding and the red/green harness

**Files:**
- Create: `avalonia/.gitignore`
- Create: `avalonia/Directory.Build.props`
- Create: `avalonia/FeWoLearning.Avalonia.slnx`
- Create: `avalonia/exercises/FeWoLearning.Avalonia.Exercises.csproj`
- Create: `avalonia/solutions/FeWoLearning.Avalonia.Solutions.csproj`
- Create: `avalonia/tests/FeWoLearning.Avalonia.Tests.csproj`
- Create: `avalonia/tests/_harness/TestAppHarness.cs`
- Create: `avalonia/tests/_harness/ViewHarness.cs`
- Test: `avalonia/tests/_harness/HarnessSmokeTests.cs`
- Create: `avalonia/gallery/FeWoLearning.Avalonia.Gallery.csproj`
- Create: `avalonia/gallery/Program.cs`, `App.axaml`, `App.axaml.cs`, `MainWindow.axaml`, `MainWindow.axaml.cs`, `GalleryEntry.cs`, `GalleryCatalog.cs`

**Interfaces:**
- Consumes: nothing.
- Produces:
  - `FeWoLearning.Avalonia.Tests.ViewHarness.Show<TView>(TView view, double width = 400, double height = 300)` returning `TView` — **every** view exercise test calls this and nothing else to trigger layout.
  - `FeWoLearning.Avalonia.Tests.ViewHarness.SolutionsMode` — `bool`, true when the loaded content assembly is the solutions one.
  - `FeWoLearning.Avalonia.Gallery.GalleryEntry` — `record GalleryEntry(string Id, string Title, Func<Control> Create)`.
  - `FeWoLearning.Avalonia.Gallery.GalleryCatalog.Entries` — `IReadOnlyList<GalleryEntry>`.
  - Both content projects expose namespace `FeWoLearning.Avalonia.Exercises.Beginner` (and the other three tiers) regardless of which one is referenced.

- [ ] **Step 1: Create the folder skeleton and `.gitignore`**

```bash
cd avalonia 2>/dev/null || mkdir -p avalonia && cd avalonia
mkdir -p exercises/01-beginner exercises/02-intermediate exercises/03-advanced exercises/04-expert exercises/_support
mkdir -p solutions/01-beginner solutions/02-intermediate solutions/03-advanced solutions/04-expert solutions/_support
mkdir -p tests/01-beginner tests/_harness
mkdir -p gallery/Pages/Beginner
```

`avalonia/.gitignore`:

```gitignore
bin/
obj/
artifacts-solutions/
```

- [ ] **Step 2: Write `Directory.Build.props`**

This is required, not cosmetic. Setting `BaseOutputPath` inside a `.csproj` body is evaluated too late; the stale default `obj/` is then globbed alongside the new one and the build fails with CS0579 duplicate-attribute errors. Same wording as `blazor/Directory.Build.props`, which documents the identical failure.

`avalonia/Directory.Build.props`:

```xml
<Project>

  <!-- Redirect the solutions build to its own output tree. This is required, not
       cosmetic: setting BaseOutputPath/BaseIntermediateOutputPath inside a .csproj
       body is evaluated too late, the stale default obj/ is then globbed alongside
       the new one, and the build fails with CS0579 duplicate-attribute errors. -->
  <PropertyGroup Condition="'$(UseSolutions)' == 'true'">
    <UseArtifactsOutput>true</UseArtifactsOutput>
    <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts-solutions</ArtifactsPath>
  </PropertyGroup>

</Project>
```

- [ ] **Step 3: Write the two content projects**

They are identical except that solutions overrides `AssemblyName` while keeping the same `RootNamespace` — that is how the same type names live in two assemblies.

`avalonia/exercises/FeWoLearning.Avalonia.Exercises.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.Avalonia.Exercises</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.1.1" />
    <PackageReference Include="ReactiveUI.Avalonia" Version="12.1.1" />
  </ItemGroup>

</Project>
```

`avalonia/solutions/FeWoLearning.Avalonia.Solutions.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>FeWoLearning.Avalonia.Solutions</AssemblyName>
    <RootNamespace>FeWoLearning.Avalonia.Exercises</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.1.1" />
    <PackageReference Include="ReactiveUI.Avalonia" Version="12.1.1" />
  </ItemGroup>

</Project>
```

- [ ] **Step 4: Write the tests project**

`avalonia/tests/FeWoLearning.Avalonia.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.Avalonia.Tests</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <!-- xunit.v3 ONLY. Avalonia.Headless.XUnit depends on xunit.v3.extensibility.core;
       adding xunit 2.x puts FactAttribute in two assemblies and every test file
       fails with CS0433. -->
  <ItemGroup>
    <PackageReference Include="Avalonia.Themes.Fluent" Version="12.1.1" />
    <PackageReference Include="Avalonia.Headless.XUnit" Version="12.1.1" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <!-- Exactly one of the two content projects, never both: that is what keeps the
       identical namespaces and type names from colliding. `dotnet test` is the red
       run against the stubs, `dotnet test -p:UseSolutions=true` the green run against
       the reference implementations. Same mechanism as blazor/ and uno/. -->
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Avalonia.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Avalonia.Solutions.csproj" />
  </ItemGroup>

  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\gallery\FeWoLearning.Avalonia.Gallery.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\gallery\FeWoLearning.Avalonia.Gallery.csproj" />
  </ItemGroup>

</Project>
```

Note the gallery reference is unconditional in effect (both branches reference it); it is written as two blocks only to sit beside the content blocks it mirrors. Simplify to one unconditional `ItemGroup` if you prefer — the gallery itself carries the `UseSolutions` condition, so it transitively brings whichever content project is active.

Actually do simplify it. Replace the last two `ItemGroup`s with:

```xml
  <ItemGroup>
    <ProjectReference Include="..\gallery\FeWoLearning.Avalonia.Gallery.csproj" />
  </ItemGroup>
```

- [ ] **Step 5: Write the test harness**

`avalonia/tests/_harness/TestAppHarness.cs`:

```csharp
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
```

`avalonia/tests/_harness/ViewHarness.cs`:

```csharp
using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Tests;

public static class ViewHarness
{
    /// <summary>
    /// Puts <paramref name="view"/> in a headless Window and shows it, which applies
    /// control templates and drives a full measure/arrange pass.
    ///
    /// Do NOT replace this with a bare Measure/Arrange call. A UserControl's XAML
    /// lives in its Content, hosted by a ContentPresenter from its control template;
    /// without an applied template the control reports its own arranged size while
    /// every child stays 0,0,0,0. ApplyTemplate() before Measure/Arrange does not
    /// fix it either. This was measured, not assumed.
    ///
    /// A headless Window's client area equals its requested Width/Height exactly,
    /// so geometry assertions against these sizes are deterministic.
    /// </summary>
    public static TView Show<TView>(TView view, double width = 400, double height = 300)
        where TView : Control
    {
        var window = new Window { Content = view, Width = width, Height = height };
        window.Show();
        return view;
    }

    /// <summary>
    /// True when the tests were built with -p:UseSolutions=true, detected from the
    /// content assembly that actually got loaded rather than from a compile symbol.
    /// </summary>
    public static bool SolutionsMode =>
        typeof(Ex001_HelloView).Assembly.GetName().Name == "FeWoLearning.Avalonia.Solutions";
}
```

`ViewHarness.SolutionsMode` references `Ex001_HelloView`, which Task 3 creates. Until then, temporarily anchor it on any type that exists in both content projects. Task 1 has no exercises yet, so for this task write the property as:

```csharp
    public static bool SolutionsMode =>
        typeof(FeWoLearning.Avalonia.Exercises.TrackMarker).Assembly.GetName().Name
            == "FeWoLearning.Avalonia.Solutions";
```

and create the marker in **both** content projects, at `exercises/_support/TrackMarker.cs` and `solutions/_support/TrackMarker.cs`, with identical content:

```csharp
namespace FeWoLearning.Avalonia.Exercises;

/// <summary>
/// Anchor for detecting which content assembly is loaded. Present in both the
/// exercises and the solutions project with identical source, so the only
/// difference is the assembly it lands in.
/// </summary>
public static class TrackMarker;
```

This keeps `SolutionsMode` independent of any single exercise, so it never breaks when exercises are added or renamed. Use the `TrackMarker` version, not the `Ex001_HelloView` version.

- [ ] **Step 6: Write the harness regression test**

This is a permanent test, not a throwaway, and it is named to match `uno/tests/_harness/HarnessSmokeTests.cs` — the sibling .NET UI track uses the same convention for the same reason. It guards the prerequisites from spec section 2.1 that would otherwise make every exercise fail for the wrong reason.

`avalonia/tests/_harness/HarnessSmokeTests.cs`:

```csharp
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Tests;

/// <summary>
/// Not an exercise: proves the harness itself still works. ReactiveUI initialization and
/// the layout pass are prerequisites for every exercise in the track, so when one of
/// them breaks these fail first and every exercise failure after them is noise.
/// </summary>
public class HarnessSmokeTests
{
    private sealed class Probe : ReactiveObject
    {
        private int _value;
        public int Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }
    }

    [Fact]
    public void ReactiveUI_Is_Initialized_So_WhenAnyValue_Works()
    {
        var probe = new Probe();
        var seen = new List<int>();
        using var sub = probe.WhenAnyValue(x => x.Value).Subscribe(seen.Add);

        probe.Value = 7;

        // WhenAnyValue emits the current value on subscribe, then each change.
        Assert.Equal(new[] { 0, 7 }, seen);
    }

    [Fact]
    public void RxVoid_Is_The_Unit_Type_And_Commands_Gate_On_CanExecute()
    {
        var probe = new Probe();
        ReactiveCommand<RxVoid, RxVoid> command = ReactiveCommand.Create(
            () => { probe.Value = 0; },
            probe.WhenAnyValue(x => x.Value).Select(v => v != 0));

        Assert.False(((System.Windows.Input.ICommand)command).CanExecute(null));
        probe.Value = 3;
        Assert.True(((System.Windows.Input.ICommand)command).CanExecute(null));
    }

    [AvaloniaFact]
    public void Show_Drives_A_Full_Layout_Pass_On_Children()
    {
        var inner = new Border { Name = "Inner", Height = 30 };
        var view = new UserControl { Content = new StackPanel { Children = { inner } } };

        ViewHarness.Show(view, 200, 100);

        Assert.Equal(200, view.Bounds.Width);
        Assert.Equal(100, view.Bounds.Height);
        Assert.Equal(30, inner.Bounds.Height);
        Assert.Equal(200, inner.Bounds.Width);
    }

    [AvaloniaFact]
    public void RunJobs_Drains_Dispatcher_Work_Queued_By_Bindings()
    {
        var probe = new Probe();
        var text = new TextBlock();
        text.Bind(TextBlock.TextProperty,
            probe.WhenAnyValue(x => x.Value).Select(v => v.ToString()));
        var view = new UserControl { Content = text };
        ViewHarness.Show(view, 200, 100);

        probe.Value = 42;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("42", text.Text);
    }
}
```

- [ ] **Step 7: Write the gallery**

`avalonia/gallery/FeWoLearning.Avalonia.Gallery.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.Avalonia.Gallery</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.1.1" />
    <PackageReference Include="Avalonia.Desktop" Version="12.1.1" />
    <PackageReference Include="Avalonia.Themes.Fluent" Version="12.1.1" />
    <PackageReference Include="Avalonia.Fonts.Inter" Version="12.1.1" />
    <PackageReference Include="ReactiveUI.Avalonia" Version="12.1.1" />
  </ItemGroup>

  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Avalonia.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Avalonia.Solutions.csproj" />
  </ItemGroup>

</Project>
```

`avalonia/gallery/GalleryEntry.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Gallery;

/// <param name="Id">Three-digit exercise number, e.g. "001".</param>
/// <param name="Title">Exercise slug, e.g. "HelloView".</param>
/// <param name="Create">Builds the demo page. In exercises mode this throws the
/// exercise's NotImplementedException, which is the correct behaviour.</param>
public sealed record GalleryEntry(string Id, string Title, Func<Control> Create);
```

`avalonia/gallery/GalleryCatalog.cs` — starts empty; Tasks 3 and 4 add entries:

```csharp
namespace FeWoLearning.Avalonia.Gallery;

public static class GalleryCatalog
{
    /// <summary>
    /// One entry per exercise whose result is visual. View-model-only exercises
    /// (ex008, ex009) deliberately have no page.
    /// </summary>
    public static IReadOnlyList<GalleryEntry> Entries { get; } = [];
}
```

`avalonia/gallery/App.axaml`:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Gallery.App">
  <Application.Styles>
    <FluentTheme />
  </Application.Styles>
</Application>
```

`avalonia/gallery/App.axaml.cs`:

```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace FeWoLearning.Avalonia.Gallery;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}
```

`avalonia/gallery/Program.cs`:

```csharp
using Avalonia;
using ReactiveUI.Avalonia;
using ReactiveUI.Builder;

namespace FeWoLearning.Avalonia.Gallery;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Same mandatory ReactiveUI initialization the test harness performs.
        RxAppBuilder.CreateReactiveUIBuilder().WithAvalonia().Build();
        BuildAvaloniaApp().StartWithClassicDesktopStyleApplicationLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
```

`avalonia/gallery/MainWindow.axaml`:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="FeWoLearning.Avalonia.Gallery.MainWindow"
        Width="900" Height="600"
        Title="FeWoLearning — Avalonia Gallery">
  <Grid ColumnDefinitions="240,*">
    <ListBox Name="EntryList" Grid.Column="0" Margin="8">
      <ListBox.ItemTemplate>
        <DataTemplate>
          <TextBlock Text="{Binding Id, StringFormat='{}{0}'}" />
        </DataTemplate>
      </ListBox.ItemTemplate>
    </ListBox>
    <Border Grid.Column="1" Margin="8" Padding="12" BorderThickness="1"
            BorderBrush="{DynamicResource SystemControlForegroundBaseLowBrush}">
      <ContentControl Name="Host" />
    </Border>
  </Grid>
</Window>
```

The `ItemTemplate` above shows only the id, which is not useful. Use this template body instead:

```xml
        <DataTemplate>
          <StackPanel Orientation="Horizontal" Spacing="8">
            <TextBlock Text="{Binding Id}" FontFamily="monospace" />
            <TextBlock Text="{Binding Title}" />
          </StackPanel>
        </DataTemplate>
```

`avalonia/gallery/MainWindow.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Gallery;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var list = this.FindControl<ListBox>("EntryList")!;
        var host = this.FindControl<ContentControl>("Host")!;

        list.ItemsSource = GalleryCatalog.Entries;
        list.SelectionChanged += (_, _) =>
        {
            if (list.SelectedItem is not GalleryEntry entry)
                return;

            // In exercises mode Create() throws the exercise's NotImplementedException.
            // Surface it in the pane instead of killing the app, so the gallery stays
            // usable as a browser of unfinished work.
            try
            {
                host.Content = entry.Create();
            }
            catch (Exception ex)
            {
                host.Content = new TextBlock
                {
                    Text = ex.Message,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                };
            }
        };
    }
}
```

- [ ] **Step 8: Write the solution file**

`avalonia/FeWoLearning.Avalonia.slnx`:

```xml
<Solution>
  <Folder Name="/exercises/">
    <Project Path="exercises/FeWoLearning.Avalonia.Exercises.csproj" />
  </Folder>
  <Folder Name="/solutions/">
    <Project Path="solutions/FeWoLearning.Avalonia.Solutions.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/FeWoLearning.Avalonia.Tests.csproj" />
  </Folder>
  <Folder Name="/gallery/">
    <Project Path="gallery/FeWoLearning.Avalonia.Gallery.csproj" />
  </Folder>
</Solution>
```

- [ ] **Step 9: Verify the harness is green in exercises mode**

Run: `dotnet test` (from `avalonia/`)
Expected: PASS — 4 tests, 0 failed. If `ReactiveUI_Is_Initialized_So_WhenAnyValue_Works` fails with `InvalidOperationException: ReactiveUI has not been initialized`, the `[ModuleInitializer]` is not running — check it is `internal static` in a non-generic static class.

- [ ] **Step 10: Verify the harness is green in solutions mode and the redirect happened**

Run: `dotnet test -p:UseSolutions=true`
Expected: PASS — the same 4 tests.

Run: `ls artifacts-solutions/bin`
Expected: directories for the solutions, tests and gallery projects. If `artifacts-solutions/` is missing, `Directory.Build.props` is not being picked up.

- [ ] **Step 11: Verify the gallery builds in both modes**

Run: `dotnet build gallery` then `dotnet build gallery -p:UseSolutions=true`
Expected: both succeed with 0 errors.

- [ ] **Step 12: Commit**

```bash
git add avalonia/.gitignore avalonia/Directory.Build.props avalonia/FeWoLearning.Avalonia.slnx \
        avalonia/exercises/FeWoLearning.Avalonia.Exercises.csproj avalonia/exercises/_support/TrackMarker.cs \
        avalonia/solutions/FeWoLearning.Avalonia.Solutions.csproj avalonia/solutions/_support/TrackMarker.cs \
        avalonia/tests/FeWoLearning.Avalonia.Tests.csproj avalonia/tests/_harness/ \
        avalonia/gallery/
git commit -m "avalonia: scaffold track (Avalonia 12 + ReactiveUI 24 + headless xunit.v3)"
```

---

## Task 2: The 100-row catalog and the README

**Files:**
- Create: `avalonia/catalog.md`
- Create: `avalonia/README.md`

**Interfaces:**
- Consumes: the commands and layout established in Task 1.
- Produces: the slug and concept vocabulary that Tasks 3 and 4 implement. The Slug column is the spec for each exercise's type name; the Concepts column is the spec for its `Drills:` header line.

- [ ] **Step 1: Write `avalonia/catalog.md`**

Follow the `blazor/catalog.md` shape: a preamble, a `**Status:**` line, and four tier tables. `**Status: 10 ✅ / 90 ⬜**` — Task 2 writes all 100 rows as ⬜ and Tasks 3 and 4 flip the first ten.

Preamble:

```markdown
# Avalonia — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned.

"Beginner" means **Avalonia** beginner, not C# beginner: ex001 is a `UserControl`
with a bound `TextBlock`, not a `FizzBuzz`. Plain C# language drills belong to the
`dotnet/` track; Blazor's component model belongs to `blazor/`.

The MVVM base is **ReactiveUI throughout**. The beginner tier uses it only
declaratively (`ReactiveObject`, `RaiseAndSetIfChanged`, `ReactiveCommand.Create`);
observable *composition* (`WhenAnyValue` at higher arity, `ToProperty`, `Throttle`,
sequencers) starts at ex036, so the Rx curve does not collide with the Avalonia curve.

Stubs live in `exercises/<tier>/ExNNN_<Slug>.axaml` (+ `.axaml.cs`) or
`ExNNN_<Slug>.cs` for view-model-only exercises, their tests in
`tests/<tier>/ExNNN_<Slug>Tests.cs`, reference solutions at the mirrored path under
`solutions/<tier>/`, and a demo page in `gallery/Pages/<Tier>/ExNNN.axaml` where the
result is visual. Tier namespaces are pinned
(`FeWoLearning.Avalonia.Exercises.Beginner` and friends), because `01-beginner` is
not a valid C# identifier.

**Status: 0 ✅ / 100 ⬜**
```

- [ ] **Step 2: Write the Beginner table (001–035)**

```markdown
## Beginner (001–035) — Avalonia fundamentals, ReactiveUI declarative

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | HelloView | `UserControl`, `x:DataType`, one-way binding into named `TextBlock`s | ⬜ |
| 002 | LayoutStackPanel | `StackPanel` orientation and `Spacing`, stacked `Bounds` | ⬜ |
| 003 | LayoutGrid | `RowDefinitions`/`ColumnDefinitions`, `Auto` vs `*`, `Grid.Row`/`Grid.Column` | ⬜ |
| 004 | LayoutGridSpan | `Grid.ColumnSpan`, proportional star sizing | ⬜ |
| 005 | LayoutDockPanel | `DockPanel.Dock`, `LastChildFill`, dock order | ⬜ |
| 006 | AlignmentAndMargin | `HorizontalAlignment`/`VerticalAlignment`, `Margin` vs `Padding` | ⬜ |
| 007 | LayoutWrapPanel | `WrapPanel` wrapping at a constrained width | ⬜ |
| 008 | ObservableViewModel | `INotifyPropertyChanged` by hand, change-only notification | ⬜ |
| 009 | ReactiveObjectBasics | `ReactiveObject`, `RaiseAndSetIfChanged`, `PropertyChanging` ordering | ⬜ |
| 010 | CompiledBinding | explicit `{CompiledBinding}`, nested path re-resolution | ⬜ |
| 011 | BindingModes | `OneWay`, `TwoWay`, `OneWayToSource` | ⬜ |
| 012 | TextBoxTwoWay | `TextBox.Text` two-way round-trip | ⬜ |
| 013 | BindingStringFormat | `StringFormat`, invariant culture | ⬜ |
| 014 | BindingFallback | `FallbackValue`, `TargetNullValue` | ⬜ |
| 015 | ValueConverter | `IValueConverter` both directions | ⬜ |
| 016 | ReactiveCommandBasics | `ReactiveCommand.Create`, `RxVoid`, invocation | ⬜ |
| 017 | CommandCanExecute | `WhenAnyValue` feeding `canExecute`, button enablement | ⬜ |
| 018 | CommandParameter | `ReactiveCommand<TParam, RxVoid>`, `CommandParameter` | ⬜ |
| 019 | ButtonClickEvent | `Click` event handler versus a bound command | ⬜ |
| 020 | CheckBoxBinding | `IsChecked` as `bool?`, three-state | ⬜ |
| 021 | RadioGroupBinding | `RadioButton` `GroupName`, enum-backed selection | ⬜ |
| 022 | SliderBinding | `Slider` `Value`/`Minimum`/`Maximum`, clamping | ⬜ |
| 023 | ComboBoxSelection | `ItemsSource` plus `SelectedItem` | ⬜ |
| 024 | ListBoxSelection | `SelectedIndex`, `SelectedItems`, selection mode | ⬜ |
| 025 | ItemsControlTemplate | `ItemsControl` with a `DataTemplate` | ⬜ |
| 026 | ObservableCollectionUpdates | add and remove reflected in the visual tree | ⬜ |
| 027 | EmptyStateFallback | `IsVisible` driven by an empty collection | ⬜ |
| 028 | StyleSelectors | `Style` `Selector` by type and by descendant | ⬜ |
| 029 | StyleClasses | `Classes`, toggling a class at runtime | ⬜ |
| 030 | PseudoClasses | `:pointerover`, `:disabled` selectors | ⬜ |
| 031 | StaticAndDynamicResource | `ResourceDictionary`, `StaticResource` vs `DynamicResource` | ⬜ |
| 032 | UserControlComposition | nesting a `UserControl`, exposing a CLR property | ⬜ |
| 033 | StyledPropertyBasics | `StyledProperty<T>` registration, default value, styling | ⬜ |
| 034 | AttachedPropertyUsage | consuming an attached property (`ToolTip.Tip`) | ⬜ |
| 035 | ScrollViewerAndSizing | `ScrollViewer`, `MinWidth`/`MaxHeight` interaction | ⬜ |
```

- [ ] **Step 3: Write the Intermediate table (036–070)**

```markdown
## Intermediate (036–070) — ReactiveUI composition, Avalonia data and templating

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | WhenAnyValueMultiArity | `WhenAnyValue` over several source properties | ⬜ |
| 037 | OutputProperty | `ToProperty`, `ObservableAsPropertyHelper` | ⬜ |
| 038 | OaphInitialValue | OAPH initial value, deferred subscription | ⬜ |
| 039 | CommandFromTask | `ReactiveCommand.CreateFromTask`, awaiting a result | ⬜ |
| 040 | CommandIsExecuting | `IsExecuting` gating concurrent invocation | ⬜ |
| 041 | CommandThrownExceptions | `ThrownExceptions`, no unobserved crash | ⬜ |
| 042 | CommandCancellation | `CancellationToken` in `CreateFromTask` | ⬜ |
| 043 | ThrottledSearch | `Throttle` plus `DistinctUntilChanged` | ⬜ |
| 044 | SequencerScheduling | `ISequencer`, virtual time in tests | ⬜ |
| 045 | MainThreadMarshalling | `RxApp.MainThreadScheduler`, `Dispatcher.UIThread` | ⬜ |
| 046 | InteractionDialog | `Interaction<TIn, TOut>` for a dialog result | ⬜ |
| 047 | ValidationNotifyDataErrorInfo | `INotifyDataErrorInfo`, per-property errors | ⬜ |
| 048 | ViewModelActivation | `IActivatableViewModel`, `WhenActivated` disposal | ⬜ |
| 049 | ViewForBinding | `IViewFor<T>`, `ReactiveUserControl` | ⬜ |
| 050 | ViewModelViewHost | resolving a view from a view model | ⬜ |
| 051 | RoutingStateNavigation | `RoutingState` navigate and navigate-back | ⬜ |
| 052 | RoutedViewHostShell | a shell hosting a router | ⬜ |
| 053 | ViewLocatorConvention | the default view-locator naming convention | ⬜ |
| 054 | DataTemplateSelector | choosing a template by item type | ⬜ |
| 055 | HierarchicalTemplate | `TreeView` with `TreeDataTemplate` | ⬜ |
| 056 | DataGridColumns | `DataGrid` columns and sorting | ⬜ |
| 057 | ItemsRepeaterLayout | `ItemsRepeater` with `UniformGridLayout` | ⬜ |
| 058 | SelectionModel | `SelectionModel` multi-selection | ⬜ |
| 059 | TemplatedControlBasics | `TemplatedControl` with a `ControlTheme` | ⬜ |
| 060 | TemplatePartLookup | `OnApplyTemplate`, finding a named part | ⬜ |
| 061 | ControlTemplateBinding | `TemplateBinding` inside a control template | ⬜ |
| 062 | AttachedPropertyAuthoring | registering your own attached property | ⬜ |
| 063 | StyleSetterAndTransition | `Transitions` on a styled property | ⬜ |
| 064 | KeyFrameAnimation | `Animation` with `KeyFrame`s | ⬜ |
| 065 | RenderTransformAnimation | animating a `RenderTransform` | ⬜ |
| 066 | MultiValueConverter | `IMultiValueConverter` over several bindings | ⬜ |
| 067 | MarkupExtensionBasics | a custom `MarkupExtension` | ⬜ |
| 068 | AsyncImageLoading | async load with placeholder and cancellation | ⬜ |
| 069 | DispatcherPriority | posting work at differing priorities | ⬜ |
| 070 | ObservableCollectionSync | diffing a source list into a bound collection | ⬜ |
```

- [ ] **Step 4: Write the Advanced table (071–090)**

```markdown
## Advanced (071–090) — custom controls, rendering, input, collections

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | CustomControlRender | `Control.Render(DrawingContext)` | ⬜ |
| 072 | MeasureArrangeOverride | `MeasureOverride`, `ArrangeOverride` | ⬜ |
| 073 | CustomLayoutPanel | a `Panel` subclass laying out children | ⬜ |
| 074 | GeometryAndPen | `StreamGeometry`, `Pen`, fill rules | ⬜ |
| 075 | CustomBrushGradient | gradient brushes, opacity masks | ⬜ |
| 076 | InvalidateVisualLifecycle | when `Render` re-runs, `InvalidateVisual` | ⬜ |
| 077 | PointerInputHandling | `PointerPressed`/`Moved`/`Released` | ⬜ |
| 078 | GestureRecognition | gesture recognizers, scroll gestures | ⬜ |
| 079 | KeyBindingsAndAccelerators | `KeyBinding`, `KeyGesture` | ⬜ |
| 080 | FocusManagement | focus traversal, `TabIndex`, `IsTabStop` | ⬜ |
| 081 | DragAndDropPayload | `DataObject`, `DragDrop` handlers | ⬜ |
| 082 | ClipboardRoundTrip | clipboard read and write | ⬜ |
| 083 | ChangeSetFilterPipeline | ReactiveUI change sets, reactive filtering | ⬜ |
| 084 | ChangeSetSortAndCount | change-set sorting and count projection | ⬜ |
| 085 | VirtualizationBudget | realized item count under virtualization | ⬜ |
| 086 | ControlThemeOverride | overriding a FluentTheme `ControlTheme` | ⬜ |
| 087 | ResourceDictionaryMerging | merged dictionaries, resource lookup order | ⬜ |
| 088 | ThemeVariantSwitching | `ThemeVariant` light and dark | ⬜ |
| 089 | LocalizationResources | culture-driven strings | ⬜ |
| 090 | FlowDirectionMirroring | right-to-left layout mirroring | ⬜ |
```

- [ ] **Step 5: Write the Expert table (091–100)**

```markdown
## Expert (091–100) — architecture, performance, harness

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | AppShellArchitecture | routing, DI and activation composed together | ⬜ |
| 092 | CustomViewLocator | an `IViewLocator` implementation | ⬜ |
| 093 | DependencyInjectionWiring | a DI container behind the ReactiveUI resolver | ⬜ |
| 094 | CompiledBindingPerformance | compiled versus reflection binding cost | ⬜ |
| 095 | TrimmingFriendlyBindings | AOT- and trim-safe binding patterns | ⬜ |
| 096 | MultiWindowLifetime | `IClassicDesktopStyleApplicationLifetime`, extra windows | ⬜ |
| 097 | PluginLoadedViews | views from a dynamically loaded assembly | ⬜ |
| 098 | RenderedFrameCapture | `CaptureRenderedFrame` pixel assertions | ⬜ |
| 099 | CustomHeadlessTestHarness | a bespoke `AppBuilder` for tests | ⬜ |
| 100 | EndToEndMvvmFeature | routing plus validation plus async in one feature | ⬜ |
```

- [ ] **Step 6: Write `avalonia/README.md`**

It must document setup, all six commands, the seven constraints, the `solutions/` deviation, and the non-goals. Write these sections:

1. **What this track is** — 100 Avalonia 12 exercises, ReactiveUI MVVM, stub-red/solution-green.
2. **Setup** — nothing to install beyond .NET 10; first `dotnet test` restores.
3. **Commands** — the table from spec section 4.1, verbatim:

```markdown
| Purpose | Command |
|---|---|
| Red — stubs | `dotnet test` |
| Green — reference solutions | `dotnet test -p:UseSolutions=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |
| One exercise, green | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |
| Look at it | `dotnet run --project gallery` |
| Look at the answers | `dotnet run --project gallery -p:UseSolutions=true` |
```

4. **This is not the ReactiveUI you have read about** — reproduce the seven-row table from spec section 2.1 verbatim. This is the single most useful section in the file.
5. **Why `solutions/` is in the build here** — spec section 8, including the note that this deviates from `CLAUDE.md` deliberately and must not be "fixed" back.
6. **Writing tests for this track** — the layout-pass rule (`ViewHarness.Show`, never bare `Measure`/`Arrange`), `Dispatcher.UIThread.RunJobs()` before asserting on scheduled work, assert through the visual tree not the view model, and the "would a wrong implementation also pass?" check.
7. **Non-goals** — spec section 6: no real window management or DPI, no GPU paths, no native dialogs, no OS clipboard or drag-and-drop hand-off, no platform handles. A green test proves Avalonia behaviour, never desktop behaviour.

- [ ] **Step 7: Verify the catalog is well-formed**

Run: `grep -c '^| [0-9]' avalonia/catalog.md`
Expected: `100`

Run: `grep -c '⬜' avalonia/catalog.md`
Expected: `101` — the 100 rows plus the legend line.

- [ ] **Step 8: Commit**

```bash
git add avalonia/catalog.md avalonia/README.md
git commit -m "avalonia: 100-row catalog and track README"
```

---

## Task 3: ex001–ex005 — binding basics and layout

**Files:**
- Create: `avalonia/exercises/01-beginner/Ex001_HelloView.axaml` + `.axaml.cs`
- Create: `avalonia/exercises/01-beginner/Ex002_LayoutStackPanel.axaml` + `.axaml.cs`
- Create: `avalonia/exercises/01-beginner/Ex003_LayoutGrid.axaml` + `.axaml.cs`
- Create: `avalonia/exercises/01-beginner/Ex004_LayoutGridSpan.axaml` + `.axaml.cs`
- Create: `avalonia/exercises/01-beginner/Ex005_LayoutDockPanel.axaml` + `.axaml.cs`
- Create: the five mirrored files under `avalonia/solutions/01-beginner/`
- Create: `avalonia/gallery/Pages/Beginner/Ex001.axaml` … `Ex005.axaml` (+ `.axaml.cs`)
- Modify: `avalonia/gallery/GalleryCatalog.cs`
- Modify: `avalonia/catalog.md` (five rows plus the status line)
- Test: `avalonia/tests/01-beginner/Ex001_HelloViewTests.cs` … `Ex005_LayoutDockPanelTests.cs`
- Test: `avalonia/tests/_harness/GallerySmokeTests.cs`

**Interfaces:**
- Consumes: `ViewHarness.Show<TView>(TView, double, double)` and `ViewHarness.SolutionsMode` from Task 1; `GalleryEntry(string Id, string Title, Func<Control> Create)` and `GalleryCatalog.Entries` from Task 1.
- Produces: in namespace `FeWoLearning.Avalonia.Exercises.Beginner` —
  `Ex001_HelloView : UserControl` with named children `TitleText`, `SubtitleText`, and `Ex001_HelloViewModel : ReactiveObject { string Title; string Subtitle; }`;
  `Ex002_LayoutStackPanel : UserControl` with named children `Row1`, `Row2`, `Row3`;
  `Ex003_LayoutGrid : UserControl` with named children `HeaderLeft`, `HeaderRight`, `BodyLeft`, `BodyRight`;
  `Ex004_LayoutGridSpan : UserControl` with named children `Banner`, `Left`, `Middle`, `Right`;
  `Ex005_LayoutDockPanel : UserControl` with named children `TopBar`, `BottomBar`, `SideBar`, `Body`.
  Task 4 adds `Ex006`–`Ex010` in the same namespace and appends to the same `GalleryCatalog.Entries`.

### ex001 — HelloView

- [ ] **Step 1: Write the ex001 stub**

`avalonia/exercises/01-beginner/Ex001_HelloView.axaml`:

```xml
<!-- Exercise 001 - HelloView (beginner).
     Goal:   Render two view-model properties into two named TextBlocks.
     Drills: UserControl, x:DataType, one-way binding into named TextBlocks.
     Passes: dotnet test - -filter FullyQualifiedName~Ex001_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex001_HelloView"
             x:DataType="b:Ex001_HelloViewModel">
  <!-- TODO: replace this placeholder with a vertical StackPanel holding
       two TextBlocks named "TitleText" and "SubtitleText", bound one-way
       to the view model's Title and Subtitle. -->
  <TextBlock Text="TODO" />
</UserControl>
```

Write the `Passes:` line with a real double dash (`--filter`); it is spelled apart here only to survive this document's own markdown.

`avalonia/exercises/01-beginner/Ex001_HelloView.axaml.cs`:

```csharp
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex001_HelloView : UserControl
{
    public Ex001_HelloView()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex001 - bind Title and Subtitle into TitleText and SubtitleText");
    }
}

/// <summary>Given. Do not change: the exercise is the XAML, not this class.</summary>
public class Ex001_HelloViewModel : ReactiveObject
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private string _subtitle = "";
    public string Subtitle
    {
        get => _subtitle;
        set => this.RaiseAndSetIfChanged(ref _subtitle, value);
    }
}
```

- [ ] **Step 2: Write the ex001 test**

`avalonia/tests/01-beginner/Ex001_HelloViewTests.cs`:

```csharp
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex001_HelloViewTests
{
    private static (Ex001_HelloView View, Ex001_HelloViewModel Vm) Arrange()
    {
        var vm = new Ex001_HelloViewModel { Title = "Avalonia", Subtitle = "desktop UI" };
        var view = ViewHarness.Show(new Ex001_HelloView { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Renders_Both_ViewModel_Properties_Into_Named_TextBlocks()
    {
        var (view, _) = Arrange();

        Assert.Equal("Avalonia", view.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("desktop UI", view.FindControl<TextBlock>("SubtitleText")!.Text);
    }

    [AvaloniaFact]
    public void Both_TextBlocks_Are_Laid_Out_And_Stacked_Vertically()
    {
        var (view, _) = Arrange();

        var title = view.FindControl<TextBlock>("TitleText")!;
        var subtitle = view.FindControl<TextBlock>("SubtitleText")!;

        Assert.True(title.Bounds.Height > 0, "TitleText was never laid out");
        Assert.True(subtitle.Bounds.Y >= title.Bounds.Bottom,
            "SubtitleText must sit below TitleText, so the panel must stack vertically");
    }

    // The anti-literal check: a view that hard-codes the strings passes the first
    // test but not this one, because only a real binding re-renders.
    [AvaloniaFact]
    public void Text_Follows_Later_ViewModel_Changes()
    {
        var (view, vm) = Arrange();

        vm.Title = "Changed";
        vm.Subtitle = "also changed";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Changed", view.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("also changed", view.FindControl<TextBlock>("SubtitleText")!.Text);
    }
}
```

- [ ] **Step 3: Write the ex001 solution**

`avalonia/solutions/01-beginner/Ex001_HelloView.axaml` — same header comment as the stub, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex001_HelloView"
             x:DataType="b:Ex001_HelloViewModel">
  <StackPanel Orientation="Vertical" Spacing="4">
    <TextBlock Name="TitleText" Text="{Binding Title}" FontSize="20" />
    <TextBlock Name="SubtitleText" Text="{Binding Subtitle}" Opacity="0.7" />
  </StackPanel>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex001_HelloView.axaml.cs` — identical to the stub's code-behind minus the `throw`:

```csharp
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex001_HelloView : UserControl
{
    public Ex001_HelloView() => InitializeComponent();
}

public class Ex001_HelloViewModel : ReactiveObject
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private string _subtitle = "";
    public string Subtitle
    {
        get => _subtitle;
        set => this.RaiseAndSetIfChanged(ref _subtitle, value);
    }
}
```

`AvaloniaUseCompiledBindingsByDefault` is `true` in both content projects, so `{Binding Title}` compiles to a compiled binding against `x:DataType`. A typo in the path is a build error, not a silent runtime miss.

### ex002 — LayoutStackPanel

- [ ] **Step 4: Write the ex002 stub**

`avalonia/exercises/01-beginner/Ex002_LayoutStackPanel.axaml`:

```xml
<!-- Exercise 002 - LayoutStackPanel (beginner).
     Goal:   Stack three fixed-height bars with uniform gaps.
     Drills: StackPanel orientation and Spacing, stacked Bounds.
     Passes: dotnet test - -filter FullyQualifiedName~Ex002_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex002_LayoutStackPanel">
  <!-- TODO: replace this placeholder with a vertical StackPanel whose Spacing is 8,
       holding three Borders named "Row1", "Row2" and "Row3", each 20 device pixels
       tall. Leave their widths to the panel. -->
  <TextBlock Text="TODO" />
</UserControl>
```

`avalonia/exercises/01-beginner/Ex002_LayoutStackPanel.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex002_LayoutStackPanel : UserControl
{
    public Ex002_LayoutStackPanel()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex002 - stack Row1/Row2/Row3 vertically with Spacing 8");
    }
}
```

- [ ] **Step 5: Write the ex002 test**

`avalonia/tests/01-beginner/Ex002_LayoutStackPanelTests.cs`:

```csharp
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex002_LayoutStackPanelTests
{
    private static (Border R1, Border R2, Border R3) Rows()
    {
        var view = ViewHarness.Show(new Ex002_LayoutStackPanel(), 200, 200);
        return (view.FindControl<Border>("Row1")!,
                view.FindControl<Border>("Row2")!,
                view.FindControl<Border>("Row3")!);
    }

    [AvaloniaFact]
    public void Each_Row_Is_Twenty_Tall_And_Fills_The_Width()
    {
        var (r1, r2, r3) = Rows();

        foreach (var row in new[] { r1, r2, r3 })
        {
            Assert.Equal(20, row.Bounds.Height);
            Assert.Equal(200, row.Bounds.Width);
        }
    }

    // The discriminator: any vertical arrangement puts the rows in order, but only
    // Spacing="8" produces exactly these offsets. A StackPanel with no Spacing
    // yields 0/20/40 and fails here.
    [AvaloniaFact]
    public void Rows_Are_Stacked_Top_Down_With_An_Eight_Pixel_Gap()
    {
        var (r1, r2, r3) = Rows();

        Assert.Equal(0, r1.Bounds.Y);
        Assert.Equal(28, r2.Bounds.Y);
        Assert.Equal(56, r3.Bounds.Y);
    }
}
```

- [ ] **Step 6: Write the ex002 solution**

`avalonia/solutions/01-beginner/Ex002_LayoutStackPanel.axaml` — same header, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex002_LayoutStackPanel">
  <StackPanel Orientation="Vertical" Spacing="8">
    <Border Name="Row1" Height="20" Background="#4C8BF5" />
    <Border Name="Row2" Height="20" Background="#34A853" />
    <Border Name="Row3" Height="20" Background="#FBBC05" />
  </StackPanel>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex002_LayoutStackPanel.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex002_LayoutStackPanel : UserControl
{
    public Ex002_LayoutStackPanel() => InitializeComponent();
}
```

### ex003 — LayoutGrid

- [ ] **Step 7: Write the ex003 stub**

`avalonia/exercises/01-beginner/Ex003_LayoutGrid.axaml`:

```xml
<!-- Exercise 003 - LayoutGrid (beginner).
     Goal:   Place four cells in a two-by-two Grid mixing Auto and star sizing.
     Drills: RowDefinitions/ColumnDefinitions, Auto vs *, Grid.Row/Grid.Column.
     Passes: dotnet test - -filter FullyQualifiedName~Ex003_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex003_LayoutGrid">
  <!-- TODO: replace this placeholder with a Grid whose rows are "Auto,*" and whose
       columns are "80,*", holding four Borders:
         HeaderLeft  at row 0 col 0, Height 24
         HeaderRight at row 0 col 1, Height 24
         BodyLeft    at row 1 col 0
         BodyRight   at row 1 col 1
       The Auto row must take its height from the 24-tall header cells. -->
  <TextBlock Text="TODO" />
</UserControl>
```

`avalonia/exercises/01-beginner/Ex003_LayoutGrid.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex003_LayoutGrid : UserControl
{
    public Ex003_LayoutGrid()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex003 - lay out HeaderLeft/HeaderRight/BodyLeft/BodyRight in a Grid");
    }
}
```

- [ ] **Step 8: Write the ex003 test**

`avalonia/tests/01-beginner/Ex003_LayoutGridTests.cs`:

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex003_LayoutGridTests
{
    // 200 x 200: column 0 is a fixed 80, column 1 takes the remaining 120.
    // Row 0 is Auto and the header cells are 24 tall, so row 1 gets 176.
    private static Ex003_LayoutGrid Show() =>
        ViewHarness.Show(new Ex003_LayoutGrid(), 200, 200);

    [AvaloniaFact]
    public void Fixed_Column_Is_Eighty_And_The_Star_Column_Takes_The_Rest()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 80, 24), view.FindControl<Border>("HeaderLeft")!.Bounds);
        Assert.Equal(new Rect(80, 0, 120, 24), view.FindControl<Border>("HeaderRight")!.Bounds);
    }

    [AvaloniaFact]
    public void Auto_Row_Takes_Its_Height_From_The_Header_And_The_Star_Row_Takes_The_Rest()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 24, 80, 176), view.FindControl<Border>("BodyLeft")!.Bounds);
        Assert.Equal(new Rect(80, 24, 120, 176), view.FindControl<Border>("BodyRight")!.Bounds);
    }
}
```

- [ ] **Step 9: Write the ex003 solution**

`avalonia/solutions/01-beginner/Ex003_LayoutGrid.axaml` — same header, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex003_LayoutGrid">
  <Grid RowDefinitions="Auto,*" ColumnDefinitions="80,*">
    <Border Name="HeaderLeft"  Grid.Row="0" Grid.Column="0" Height="24" Background="#22000000" />
    <Border Name="HeaderRight" Grid.Row="0" Grid.Column="1" Height="24" Background="#11000000" />
    <Border Name="BodyLeft"    Grid.Row="1" Grid.Column="0" Background="#4C8BF5" />
    <Border Name="BodyRight"   Grid.Row="1" Grid.Column="1" Background="#34A853" />
  </Grid>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex003_LayoutGrid.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex003_LayoutGrid : UserControl
{
    public Ex003_LayoutGrid() => InitializeComponent();
}
```

### ex004 — LayoutGridSpan

- [ ] **Step 10: Write the ex004 stub**

`avalonia/exercises/01-beginner/Ex004_LayoutGridSpan.axaml`:

```xml
<!-- Exercise 004 - LayoutGridSpan (beginner).
     Goal:   Span a banner across three proportionally sized columns.
     Drills: Grid.ColumnSpan, proportional star sizing.
     Passes: dotnet test - -filter FullyQualifiedName~Ex004_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex004_LayoutGridSpan">
  <!-- TODO: replace this placeholder with a Grid whose columns are "1*,2*,1*" and
       whose rows are "Auto,Auto", holding:
         Banner at row 0, spanning all three columns, Height 16
         Left   at row 1 col 0, Height 30
         Middle at row 1 col 1, Height 30
         Right  at row 1 col 2, Height 30
       The middle column must end up exactly twice as wide as each outer one. -->
  <TextBlock Text="TODO" />
</UserControl>
```

`avalonia/exercises/01-beginner/Ex004_LayoutGridSpan.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex004_LayoutGridSpan : UserControl
{
    public Ex004_LayoutGridSpan()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex004 - span Banner across three 1*/2*/1* columns");
    }
}
```

- [ ] **Step 11: Write the ex004 test**

`avalonia/tests/01-beginner/Ex004_LayoutGridSpanTests.cs`:

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex004_LayoutGridSpanTests
{
    // 400 wide over 1*/2*/1* gives 100 / 200 / 100.
    private static Ex004_LayoutGridSpan Show() =>
        ViewHarness.Show(new Ex004_LayoutGridSpan(), 400, 200);

    [AvaloniaFact]
    public void Banner_Spans_The_Full_Width_Of_All_Three_Columns()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 400, 16), view.FindControl<Border>("Banner")!.Bounds);
    }

    // The discriminator: three equal columns would give 133.33 each and fail.
    [AvaloniaFact]
    public void Middle_Column_Is_Exactly_Twice_Each_Outer_Column()
    {
        var view = Show();

        var left = view.FindControl<Border>("Left")!;
        var middle = view.FindControl<Border>("Middle")!;
        var right = view.FindControl<Border>("Right")!;

        Assert.Equal(new Rect(0, 16, 100, 30), left.Bounds);
        Assert.Equal(new Rect(100, 16, 200, 30), middle.Bounds);
        Assert.Equal(new Rect(300, 16, 100, 30), right.Bounds);
        Assert.Equal(2 * left.Bounds.Width, middle.Bounds.Width);
    }
}
```

- [ ] **Step 12: Write the ex004 solution**

`avalonia/solutions/01-beginner/Ex004_LayoutGridSpan.axaml` — same header, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex004_LayoutGridSpan">
  <Grid RowDefinitions="Auto,Auto" ColumnDefinitions="1*,2*,1*">
    <Border Name="Banner" Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="3"
            Height="16" Background="#22000000" />
    <Border Name="Left"   Grid.Row="1" Grid.Column="0" Height="30" Background="#4C8BF5" />
    <Border Name="Middle" Grid.Row="1" Grid.Column="1" Height="30" Background="#34A853" />
    <Border Name="Right"  Grid.Row="1" Grid.Column="2" Height="30" Background="#FBBC05" />
  </Grid>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex004_LayoutGridSpan.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex004_LayoutGridSpan : UserControl
{
    public Ex004_LayoutGridSpan() => InitializeComponent();
}
```

### ex005 — LayoutDockPanel

- [ ] **Step 13: Write the ex005 stub**

`avalonia/exercises/01-beginner/Ex005_LayoutDockPanel.axaml`:

```xml
<!-- Exercise 005 - LayoutDockPanel (beginner).
     Goal:   Build a classic shell: top bar, bottom bar, side bar, filled body.
     Drills: DockPanel.Dock, LastChildFill, dock order.
     Passes: dotnet test - -filter FullyQualifiedName~Ex005_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex005_LayoutDockPanel">
  <!-- TODO: replace this placeholder with a DockPanel that fills with its last
       child, holding in this order:
         TopBar    docked Top,    Height 30
         BottomBar docked Bottom, Height 20
         SideBar   docked Left,   Width 60
         Body      the last child, taking whatever is left
       Dock order matters: TopBar must span the full width, and SideBar must only
       occupy what is left between the two bars. -->
  <TextBlock Text="TODO" />
</UserControl>
```

`avalonia/exercises/01-beginner/Ex005_LayoutDockPanel.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex005_LayoutDockPanel : UserControl
{
    public Ex005_LayoutDockPanel()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex005 - dock TopBar/BottomBar/SideBar and fill with Body");
    }
}
```

- [ ] **Step 14: Write the ex005 test**

These four rectangles, like every other geometry assertion in Tasks 3 and 4, were measured against a real Avalonia 12.1.1 headless run rather than derived on paper.

`avalonia/tests/01-beginner/Ex005_LayoutDockPanelTests.cs`:

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex005_LayoutDockPanelTests
{
    private static Ex005_LayoutDockPanel Show() =>
        ViewHarness.Show(new Ex005_LayoutDockPanel(), 300, 200);

    [AvaloniaFact]
    public void Top_And_Bottom_Bars_Span_The_Full_Width()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 300, 30), view.FindControl<Border>("TopBar")!.Bounds);
        Assert.Equal(new Rect(0, 180, 300, 20), view.FindControl<Border>("BottomBar")!.Bounds);
    }

    // The discriminator for dock ORDER: if SideBar were docked before TopBar it would
    // run the full 200 height instead of the 150 left between the bars.
    [AvaloniaFact]
    public void SideBar_Only_Occupies_What_Is_Left_Between_The_Bars()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 30, 60, 150), view.FindControl<Border>("SideBar")!.Bounds);
    }

    // The discriminator for LastChildFill: without it Body collapses to zero width.
    [AvaloniaFact]
    public void Body_Fills_The_Remaining_Space()
    {
        var view = Show();

        Assert.Equal(new Rect(60, 30, 240, 150), view.FindControl<Border>("Body")!.Bounds);
    }
}
```

- [ ] **Step 15: Write the ex005 solution**

`avalonia/solutions/01-beginner/Ex005_LayoutDockPanel.axaml` — same header, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex005_LayoutDockPanel">
  <DockPanel LastChildFill="True">
    <Border Name="TopBar"    DockPanel.Dock="Top"    Height="30" Background="#22000000" />
    <Border Name="BottomBar" DockPanel.Dock="Bottom" Height="20" Background="#22000000" />
    <Border Name="SideBar"   DockPanel.Dock="Left"   Width="60"  Background="#11000000" />
    <Border Name="Body"      Background="#4C8BF5" />
  </DockPanel>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex005_LayoutDockPanel.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex005_LayoutDockPanel : UserControl
{
    public Ex005_LayoutDockPanel() => InitializeComponent();
}
```

### Gallery pages and the red check

- [ ] **Step 16: Write the five gallery pages**

Each page is a thin wrapper that supplies a sample `DataContext` where the exercise needs one. `avalonia/gallery/Pages/Beginner/Ex001.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner;assembly=FeWoLearning.Avalonia.Exercises"
             x:Class="FeWoLearning.Avalonia.Gallery.Pages.Beginner.Ex001">
  <StackPanel Spacing="12">
    <TextBlock Text="Exercise 001 — HelloView" FontSize="18" FontWeight="Bold" />
    <b:Ex001_HelloView Name="Subject" />
  </StackPanel>
</UserControl>
```

The `assembly=` in that `clr-namespace` is wrong for solutions mode, where the assembly is named `FeWoLearning.Avalonia.Solutions`. **Omit the `assembly=` part entirely** — Avalonia then resolves the namespace across referenced assemblies, and exactly one of the two is ever referenced:

```xml
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner"
```

`avalonia/gallery/Pages/Beginner/Ex001.axaml.cs`:

```csharp
using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex001 : UserControl
{
    public Ex001()
    {
        InitializeComponent();
        this.FindControl<Ex001_HelloView>("Subject")!.DataContext =
            new Ex001_HelloViewModel { Title = "Avalonia", Subtitle = "desktop UI" };
    }
}
```

`Ex002` through `Ex005` follow the same shape with their own title text and no `DataContext` line, since those exercises are pure layout. For example `avalonia/gallery/Pages/Beginner/Ex005.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner"
             x:Class="FeWoLearning.Avalonia.Gallery.Pages.Beginner.Ex005">
  <StackPanel Spacing="12">
    <TextBlock Text="Exercise 005 — LayoutDockPanel" FontSize="18" FontWeight="Bold" />
    <Border Height="200" Width="300" HorizontalAlignment="Left">
      <b:Ex005_LayoutDockPanel />
    </Border>
  </StackPanel>
</UserControl>
```

and `avalonia/gallery/Pages/Beginner/Ex005.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex005 : UserControl
{
    public Ex005() => InitializeComponent();
}
```

- [ ] **Step 17: Register the five pages in the gallery catalog**

Replace the body of `avalonia/gallery/GalleryCatalog.cs`:

```csharp
using FeWoLearning.Avalonia.Gallery.Pages.Beginner;

namespace FeWoLearning.Avalonia.Gallery;

public static class GalleryCatalog
{
    /// <summary>
    /// One entry per exercise whose result is visual. View-model-only exercises
    /// (ex008, ex009) deliberately have no page.
    /// </summary>
    public static IReadOnlyList<GalleryEntry> Entries { get; } =
    [
        new("001", "HelloView", () => new Ex001()),
        new("002", "LayoutStackPanel", () => new Ex002()),
        new("003", "LayoutGrid", () => new Ex003()),
        new("004", "LayoutGridSpan", () => new Ex004()),
        new("005", "LayoutDockPanel", () => new Ex005()),
    ];
}
```

- [ ] **Step 18: Write the gallery smoke test**

This test also proves the red/green mechanism itself, which nothing else does.

`avalonia/tests/_harness/GallerySmokeTests.cs`:

```csharp
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Gallery;

namespace FeWoLearning.Avalonia.Tests;

public class GallerySmokeTests
{
    [Fact]
    public void Every_Registered_Entry_Has_A_Three_Digit_Id_And_A_Title()
    {
        Assert.NotEmpty(GalleryCatalog.Entries);

        foreach (var entry in GalleryCatalog.Entries)
        {
            Assert.Matches(@"^\d{3}$", entry.Id);
            Assert.False(string.IsNullOrWhiteSpace(entry.Title));
        }
    }

    [Fact]
    public void Ids_Are_Unique_And_Ascending()
    {
        var ids = GalleryCatalog.Entries.Select(e => e.Id).ToList();

        Assert.Equal(ids.Distinct().Count(), ids.Count);
        Assert.Equal(ids.OrderBy(id => id).ToList(), ids);
    }

    // Constructing a page reaches straight into the exercise's constructor, so this
    // asserts the red/green mechanism as much as the gallery: in exercises mode every
    // page must surface the stub's NotImplementedException, and in solutions mode
    // every page must build. A page that succeeds in exercises mode means its stub
    // forgot to throw.
    [AvaloniaFact]
    public void Every_Page_Builds_In_Solutions_Mode_And_Throws_In_Exercises_Mode()
    {
        foreach (var entry in GalleryCatalog.Entries)
        {
            if (ViewHarness.SolutionsMode)
            {
                Assert.NotNull(entry.Create());
                continue;
            }

            var error = Record.Exception(() => entry.Create());
            Assert.NotNull(error);
            Assert.Contains($"TODO: Ex{entry.Id}", Flatten(error!));
        }
    }

    private static string Flatten(Exception ex) =>
        ex.InnerException is null ? ex.Message : $"{ex.Message} {Flatten(ex.InnerException)}";
}
```

The `Flatten` helper exists because a throwing constructor invoked from XAML surfaces wrapped in an Avalonia XAML load exception, so the TODO text can be one or two levels down.

- [ ] **Step 19: Red check — the five exercises must fail for the right reason**

Run: `dotnet test --filter "FullyQualifiedName~Ex001_|FullyQualifiedName~Ex002_|FullyQualifiedName~Ex003_|FullyQualifiedName~Ex004_|FullyQualifiedName~Ex005_"`

Expected: every test FAILS, and **no test passes**. For each failure confirm the message contains `TODO: ExNNN`. If any failure is a compile error, a XAML load error unrelated to the throw, or `ReactiveUI has not been initialized`, that is a bug in the stub or the harness — fix it before continuing. If any test *passes* against a stub, the test is defective.

- [ ] **Step 20: Green check — the same five must pass against the solutions**

Run: `dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex001_|FullyQualifiedName~Ex002_|FullyQualifiedName~Ex003_|FullyQualifiedName~Ex004_|FullyQualifiedName~Ex005_"`

Expected: PASS, 0 failed.

- [ ] **Step 21: Verify the harness and gallery tests in both modes**

Run: `dotnet test --filter "FullyQualifiedName~SmokeTests"`
Expected: PASS — `HarnessSmokeTests` plus `GallerySmokeTests`, with the exercises-mode branch asserting the TODOs. **Check the reported test count is non-zero**: a `--filter` that matches nothing exits successfully and reads like a pass, which is the cheapest way to fake a green run in this whole plan.

Run: `dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~SmokeTests"`
Expected: PASS — same tests, non-zero count, solutions-mode branch.

- [ ] **Step 22: Flip the five catalog rows**

In `avalonia/catalog.md` change rows 001–005 from `⬜` to `✅` and set the status line to `**Status: 5 ✅ / 95 ⬜**`. Check whether the status cell is padded in this file before doing a blind replace — some catalogs in this repo pad `⬜     |` and others do not.

- [ ] **Step 23: Commit**

```bash
git add avalonia/exercises/01-beginner avalonia/solutions/01-beginner avalonia/tests/01-beginner \
        avalonia/tests/_harness/GallerySmokeTests.cs avalonia/gallery/Pages avalonia/gallery/GalleryCatalog.cs \
        avalonia/catalog.md
git commit -m "avalonia: ex001-ex005"
```

---

## Task 4: ex006–ex010 — alignment, wrapping, notification, compiled binding

**Files:**
- Create: `avalonia/exercises/01-beginner/Ex006_AlignmentAndMargin.axaml` + `.axaml.cs`
- Create: `avalonia/exercises/01-beginner/Ex007_LayoutWrapPanel.axaml` + `.axaml.cs`
- Create: `avalonia/exercises/01-beginner/Ex008_ObservableViewModel.cs`
- Create: `avalonia/exercises/01-beginner/Ex009_ReactiveObjectBasics.cs`
- Create: `avalonia/exercises/01-beginner/Ex010_CompiledBinding.axaml` + `.axaml.cs`
- Create: the five mirrored files under `avalonia/solutions/01-beginner/`
- Create: `avalonia/gallery/Pages/Beginner/Ex006.axaml`, `Ex007.axaml`, `Ex010.axaml` (+ `.axaml.cs`)
- Modify: `avalonia/gallery/GalleryCatalog.cs`
- Modify: `avalonia/catalog.md`
- Test: `avalonia/tests/01-beginner/Ex006_AlignmentAndMarginTests.cs` … `Ex010_CompiledBindingTests.cs`

**Interfaces:**
- Consumes: `ViewHarness.Show`, `ViewHarness.SolutionsMode`, `GalleryEntry`, `GalleryCatalog.Entries` — all from Task 1; the five entries added in Task 3.
- Produces: in namespace `FeWoLearning.Avalonia.Exercises.Beginner` —
  `Ex006_AlignmentAndMargin : UserControl` with named children `Frame`, `Box`;
  `Ex007_LayoutWrapPanel : UserControl` with named children `Item1`–`Item4`;
  `Ex008_ObservableViewModel : INotifyPropertyChanged` with `int Count`;
  `Ex009_ReactiveObjectBasics : ReactiveObject` with `int Count`;
  `Ex010_CompiledBinding : UserControl` with named children `TitleText`, `AuthorText`, plus `Ex010_BookViewModel : ReactiveObject { string Title; Ex010_AuthorViewModel Author; }` and `Ex010_AuthorViewModel : ReactiveObject { string Name; }`.
  ex008 and ex009 get **no** gallery page.

### ex006 — AlignmentAndMargin

- [ ] **Step 1: Write the ex006 stub**

`avalonia/exercises/01-beginner/Ex006_AlignmentAndMargin.axaml`:

```xml
<!-- Exercise 006 - AlignmentAndMargin (beginner).
     Goal:   Position a small box inside a padded frame using alignment and margin.
     Drills: HorizontalAlignment/VerticalAlignment, Margin vs Padding.
     Passes: dotnet test - -filter FullyQualifiedName~Ex006_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex006_AlignmentAndMargin">
  <!-- TODO: replace this placeholder with a Border named "Frame":
         Width 200, Height 100, Padding 10,
         pinned to the top-left of the control (not centred),
       containing a Border named "Box":
         Width 40, Height 20,
         horizontally centred inside the frame's content area,
         aligned to the bottom of it, with a 5-pixel gap below.
       Padding belongs to the Frame; the gap below the Box is the Box's Margin. -->
  <TextBlock Text="TODO" />
</UserControl>
```

`avalonia/exercises/01-beginner/Ex006_AlignmentAndMargin.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex006_AlignmentAndMargin : UserControl
{
    public Ex006_AlignmentAndMargin()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex006 - place Box inside Frame with padding, alignment and margin");
    }
}
```

- [ ] **Step 2: Write the ex006 test**

`avalonia/tests/01-beginner/Ex006_AlignmentAndMarginTests.cs`:

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex006_AlignmentAndMarginTests
{
    private static Ex006_AlignmentAndMargin Show() =>
        ViewHarness.Show(new Ex006_AlignmentAndMargin(), 300, 200);

    // Frame is explicitly sized and must be pinned top-left, not centred in the
    // 300x200 control. Stretch alignment with an explicit Width centres it, which
    // is the mistake this asserts against.
    [AvaloniaFact]
    public void Frame_Is_Two_Hundred_By_One_Hundred_At_The_Top_Left()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 200, 100), view.FindControl<Border>("Frame")!.Bounds);
    }

    // Content area is inset 10 on every side: x 10..190 (180 wide), y 10..90 (80 tall).
    // Centred horizontally: 10 + (180 - 40) / 2 = 80.
    // Bottom aligned with a 5 margin below: 10 + 80 - 20 - 5 = 65.
    [AvaloniaFact]
    public void Box_Is_Centred_Horizontally_And_Sits_Five_Above_The_Padded_Bottom()
    {
        var view = Show();

        Assert.Equal(new Rect(80, 65, 40, 20), view.FindControl<Border>("Box")!.Bounds);
    }
}
```

- [ ] **Step 3: Write the ex006 solution**

`avalonia/solutions/01-beginner/Ex006_AlignmentAndMargin.axaml` — same header, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex006_AlignmentAndMargin">
  <Border Name="Frame" Width="200" Height="100" Padding="10"
          HorizontalAlignment="Left" VerticalAlignment="Top"
          Background="#11000000">
    <Border Name="Box" Width="40" Height="20"
            HorizontalAlignment="Center" VerticalAlignment="Bottom"
            Margin="0,0,0,5"
            Background="#4C8BF5" />
  </Border>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex006_AlignmentAndMargin.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex006_AlignmentAndMargin : UserControl
{
    public Ex006_AlignmentAndMargin() => InitializeComponent();
}
```

### ex007 — LayoutWrapPanel

- [ ] **Step 4: Write the ex007 stub**

`avalonia/exercises/01-beginner/Ex007_LayoutWrapPanel.axaml`:

```xml
<!-- Exercise 007 - LayoutWrapPanel (beginner).
     Goal:   Let four fixed-width tiles wrap onto two rows.
     Drills: WrapPanel wrapping at a constrained width.
     Passes: dotnet test - -filter FullyQualifiedName~Ex007_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex007_LayoutWrapPanel">
  <!-- TODO: replace this placeholder with a horizontal WrapPanel:
         Width 200, pinned to the top-left,
         holding four Borders named "Item1".."Item4", each 80 wide and 20 tall.
       Two tiles fit per row at 200 wide (160 fits, 240 does not), so Item3 and
       Item4 must wrap onto a second row. -->
  <TextBlock Text="TODO" />
</UserControl>
```

`avalonia/exercises/01-beginner/Ex007_LayoutWrapPanel.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex007_LayoutWrapPanel : UserControl
{
    public Ex007_LayoutWrapPanel()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex007 - wrap Item1..Item4 two per row in a 200-wide WrapPanel");
    }
}
```

- [ ] **Step 5: Write the ex007 test**

`avalonia/tests/01-beginner/Ex007_LayoutWrapPanelTests.cs`:

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex007_LayoutWrapPanelTests
{
    private static Ex007_LayoutWrapPanel Show() =>
        ViewHarness.Show(new Ex007_LayoutWrapPanel(), 400, 200);

    [AvaloniaFact]
    public void First_Two_Tiles_Share_The_First_Row()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 80, 20), view.FindControl<Border>("Item1")!.Bounds);
        Assert.Equal(new Rect(80, 0, 80, 20), view.FindControl<Border>("Item2")!.Bounds);
    }

    // The discriminator: a StackPanel or an unconstrained WrapPanel keeps all four on
    // one row, so Item3 would land at x=160 y=0 and fail here.
    [AvaloniaFact]
    public void Last_Two_Tiles_Wrap_Onto_A_Second_Row()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 20, 80, 20), view.FindControl<Border>("Item3")!.Bounds);
        Assert.Equal(new Rect(80, 20, 80, 20), view.FindControl<Border>("Item4")!.Bounds);
    }
}
```

- [ ] **Step 6: Write the ex007 solution**

`avalonia/solutions/01-beginner/Ex007_LayoutWrapPanel.axaml` — same header, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex007_LayoutWrapPanel">
  <WrapPanel Orientation="Horizontal" Width="200"
             HorizontalAlignment="Left" VerticalAlignment="Top">
    <Border Name="Item1" Width="80" Height="20" Background="#4C8BF5" />
    <Border Name="Item2" Width="80" Height="20" Background="#34A853" />
    <Border Name="Item3" Width="80" Height="20" Background="#FBBC05" />
    <Border Name="Item4" Width="80" Height="20" Background="#EA4335" />
  </WrapPanel>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex007_LayoutWrapPanel.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex007_LayoutWrapPanel : UserControl
{
    public Ex007_LayoutWrapPanel() => InitializeComponent();
}
```

### ex008 — ObservableViewModel

- [ ] **Step 7: Write the ex008 stub**

No XAML: this is the one exercise where the learner writes `INotifyPropertyChanged` by hand, so that ex009 onward can use `ReactiveObject` knowing what it does.

`avalonia/exercises/01-beginner/Ex008_ObservableViewModel.cs`:

```csharp
using System.ComponentModel;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 008 - ObservableViewModel (beginner).
/// Goal:   Implement INotifyPropertyChanged by hand, exactly once in this track.
/// Drills: INotifyPropertyChanged, change-only notification.
/// Passes: dotnet test --filter FullyQualifiedName~Ex008_
public class Ex008_ObservableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private int _count;

    /// <summary>
    /// TODO: return the stored value, and on set store it and raise PropertyChanged
    /// with the property's name - but ONLY when the incoming value actually differs
    /// from the current one. Assigning the same value again must raise nothing.
    /// </summary>
    public int Count
    {
        get => throw new NotImplementedException(
            "TODO: Ex008 - return the backing field");
        set => throw new NotImplementedException(
            "TODO: Ex008 - store and raise PropertyChanged only on a real change");
    }
}
```

The `_count` field is assigned by nothing in the stub, which produces a CS0414-style "assigned but never used" warning at most, never an error. Leave it — it is the hint.

- [ ] **Step 8: Write the ex008 test**

`avalonia/tests/01-beginner/Ex008_ObservableViewModelTests.cs`:

```csharp
using System.ComponentModel;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex008_ObservableViewModelTests
{
    private static (Ex008_ObservableViewModel Vm, List<string?> Raised) Arrange()
    {
        var vm = new Ex008_ObservableViewModel();
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);
        return (vm, raised);
    }

    [Fact]
    public void Starts_At_Zero_And_Round_Trips_The_Value()
    {
        var (vm, _) = Arrange();

        Assert.Equal(0, vm.Count);

        vm.Count = 5;

        Assert.Equal(5, vm.Count);
    }

    [Fact]
    public void Raises_PropertyChanged_With_The_Property_Name()
    {
        var (vm, raised) = Arrange();

        vm.Count = 5;

        Assert.Equal(new[] { nameof(Ex008_ObservableViewModel.Count) }, raised);
    }

    // The discriminator: a setter that raises unconditionally passes the test above
    // but fails here.
    [Fact]
    public void Assigning_The_Same_Value_Raises_Nothing()
    {
        var (vm, raised) = Arrange();

        vm.Count = 5;
        vm.Count = 5;
        vm.Count = 5;

        Assert.Single(raised);
    }

    [Fact]
    public void Each_Real_Change_Raises_Once()
    {
        var (vm, raised) = Arrange();

        vm.Count = 1;
        vm.Count = 2;
        vm.Count = 2;
        vm.Count = 3;

        Assert.Equal(3, raised.Count);
        Assert.All(raised, name => Assert.Equal("Count", name));
    }
}
```

- [ ] **Step 9: Write the ex008 solution**

`avalonia/solutions/01-beginner/Ex008_ObservableViewModel.cs`:

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 008 - ObservableViewModel (beginner).
/// Goal:   Implement INotifyPropertyChanged by hand, exactly once in this track.
/// Drills: INotifyPropertyChanged, change-only notification.
/// Passes: dotnet test --filter FullyQualifiedName~Ex008_
public class Ex008_ObservableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private int _count;

    public int Count
    {
        get => _count;
        set
        {
            if (_count == value)
                return;

            _count = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

### ex009 — ReactiveObjectBasics

- [ ] **Step 10: Write the ex009 stub**

`avalonia/exercises/01-beginner/Ex009_ReactiveObjectBasics.cs`:

```csharp
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 009 - ReactiveObjectBasics (beginner).
/// Goal:   Get the whole of Ex008 for one line, and get PropertyChanging for free.
/// Drills: ReactiveObject, RaiseAndSetIfChanged, PropertyChanging ordering.
/// Passes: dotnet test --filter FullyQualifiedName~Ex009_
public class Ex009_ReactiveObjectBasics : ReactiveObject
{
    private int _count;

    /// <summary>
    /// TODO: implement this the ReactiveUI way, with a single call in the setter.
    /// Besides the change-only PropertyChanged of Ex008, the tests also require
    /// PropertyChanging to be raised BEFORE the backing field is updated - which
    /// you get for free from the right helper, and cannot get from a hand-written
    /// OnPropertyChanged.
    /// </summary>
    public int Count
    {
        get => throw new NotImplementedException(
            "TODO: Ex009 - return the backing field");
        set => throw new NotImplementedException(
            "TODO: Ex009 - use the ReactiveObject helper that raises both events");
    }
}
```

- [ ] **Step 11: Write the ex009 test**

`avalonia/tests/01-beginner/Ex009_ReactiveObjectBasicsTests.cs`:

```csharp
using System.ComponentModel;
using FeWoLearning.Avalonia.Exercises.Beginner;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex009_ReactiveObjectBasicsTests
{
    [Fact]
    public void Round_Trips_The_Value_And_Raises_Change_Only()
    {
        var vm = new Ex009_ReactiveObjectBasics();
        var changed = new List<string?>();
        ((INotifyPropertyChanged)vm).PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Count = 5;
        vm.Count = 5;

        Assert.Equal(5, vm.Count);
        Assert.Equal(new[] { nameof(Ex009_ReactiveObjectBasics.Count) }, changed);
    }

    // The discriminator against re-hand-rolling Ex008 inside a ReactiveObject:
    // PropertyChanging must fire, and must fire while the old value is still in
    // place. Only RaiseAndSetIfChanged (or an explicit RaisePropertyChanging before
    // the assignment) satisfies both.
    [Fact]
    public void Raises_PropertyChanging_Before_The_Value_Is_Updated()
    {
        var vm = new Ex009_ReactiveObjectBasics { Count = 1 };
        var valueSeenWhileChanging = new List<int>();
        var changingNames = new List<string?>();

        ((INotifyPropertyChanging)vm).PropertyChanging += (_, e) =>
        {
            changingNames.Add(e.PropertyName);
            valueSeenWhileChanging.Add(vm.Count);
        };

        vm.Count = 2;

        Assert.Equal(new[] { nameof(Ex009_ReactiveObjectBasics.Count) }, changingNames);
        Assert.Equal(new[] { 1 }, valueSeenWhileChanging);
        Assert.Equal(2, vm.Count);
    }

    [Fact]
    public void Assigning_The_Same_Value_Raises_Neither_Event()
    {
        var vm = new Ex009_ReactiveObjectBasics { Count = 7 };
        var events = 0;
        ((INotifyPropertyChanged)vm).PropertyChanged += (_, _) => events++;
        ((INotifyPropertyChanging)vm).PropertyChanging += (_, _) => events++;

        vm.Count = 7;

        Assert.Equal(0, events);
    }

    // Proves the property participates in ReactiveUI's observable pipeline, which is
    // what the rest of the track builds on.
    [Fact]
    public void Property_Is_Observable_Through_WhenAnyValue()
    {
        var vm = new Ex009_ReactiveObjectBasics { Count = 1 };
        var seen = new List<int>();
        using var sub = vm.WhenAnyValue(x => x.Count).Subscribe(seen.Add);

        vm.Count = 2;
        vm.Count = 3;

        Assert.Equal(new[] { 1, 2, 3 }, seen);
    }
}
```

- [ ] **Step 12: Write the ex009 solution**

`avalonia/solutions/01-beginner/Ex009_ReactiveObjectBasics.cs`:

```csharp
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 009 - ReactiveObjectBasics (beginner).
/// Goal:   Get the whole of Ex008 for one line, and get PropertyChanging for free.
/// Drills: ReactiveObject, RaiseAndSetIfChanged, PropertyChanging ordering.
/// Passes: dotnet test --filter FullyQualifiedName~Ex009_
public class Ex009_ReactiveObjectBasics : ReactiveObject
{
    private int _count;

    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }
}
```

### ex010 — CompiledBinding

- [ ] **Step 13: Write the ex010 stub**

`avalonia/exercises/01-beginner/Ex010_CompiledBinding.axaml`:

```xml
<!-- Exercise 010 - CompiledBinding (beginner).
     Goal:   Bind a direct and a nested path with explicit compiled bindings.
     Drills: explicit {CompiledBinding}, nested path re-resolution.
     Passes: dotnet test - -filter FullyQualifiedName~Ex010_ -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex010_CompiledBinding"
             x:DataType="b:Ex010_BookViewModel">
  <!-- TODO: replace this placeholder with a vertical StackPanel holding
       two TextBlocks, using the explicit {CompiledBinding ...} markup extension
       rather than {Binding ...}:
         TitleText  bound to Title
         AuthorText bound to the NESTED path Author.Name
       Because x:DataType is declared above, a typo in either path is a build
       error rather than a silent blank at runtime - try it once on purpose. -->
  <TextBlock Text="TODO" />
</UserControl>
```

`avalonia/exercises/01-beginner/Ex010_CompiledBinding.axaml.cs`:

```csharp
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex010_CompiledBinding : UserControl
{
    public Ex010_CompiledBinding()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex010 - bind Title and Author.Name with {CompiledBinding}");
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex010_BookViewModel : ReactiveObject
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private Ex010_AuthorViewModel _author = new();
    public Ex010_AuthorViewModel Author
    {
        get => _author;
        set => this.RaiseAndSetIfChanged(ref _author, value);
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex010_AuthorViewModel : ReactiveObject
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
```

- [ ] **Step 14: Write the ex010 test**

`avalonia/tests/01-beginner/Ex010_CompiledBindingTests.cs`:

```csharp
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex010_CompiledBindingTests
{
    private static (Ex010_CompiledBinding View, Ex010_BookViewModel Vm) Arrange()
    {
        var vm = new Ex010_BookViewModel
        {
            Title = "Design Patterns",
            Author = new Ex010_AuthorViewModel { Name = "Erich Gamma" },
        };
        var view = ViewHarness.Show(new Ex010_CompiledBinding { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Renders_The_Direct_And_The_Nested_Path()
    {
        var (view, _) = Arrange();

        Assert.Equal("Design Patterns", view.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("Erich Gamma", view.FindControl<TextBlock>("AuthorText")!.Text);
    }

    [AvaloniaFact]
    public void Direct_Path_Follows_A_Later_Change()
    {
        var (view, vm) = Arrange();

        vm.Title = "Refactoring";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Refactoring", view.FindControl<TextBlock>("TitleText")!.Text);
    }

    // The nested-path discriminator: a binding to Author.Name must re-resolve when the
    // intermediate Author object itself is swapped, not only when its Name changes.
    [AvaloniaFact]
    public void Nested_Path_Re_Resolves_When_The_Intermediate_Object_Is_Replaced()
    {
        var (view, vm) = Arrange();
        var author = view.FindControl<TextBlock>("AuthorText")!;

        vm.Author.Name = "Richard Helm";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Richard Helm", author.Text);

        vm.Author = new Ex010_AuthorViewModel { Name = "Ralph Johnson" };
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Ralph Johnson", author.Text);
    }
}
```

- [ ] **Step 15: Write the ex010 solution**

`avalonia/solutions/01-beginner/Ex010_CompiledBinding.axaml` — same header, then:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner"
             x:Class="FeWoLearning.Avalonia.Exercises.Beginner.Ex010_CompiledBinding"
             x:DataType="b:Ex010_BookViewModel">
  <StackPanel Orientation="Vertical" Spacing="4">
    <TextBlock Name="TitleText" Text="{CompiledBinding Title}" FontSize="18" />
    <TextBlock Name="AuthorText" Text="{CompiledBinding Author.Name}" Opacity="0.7" />
  </StackPanel>
</UserControl>
```

`avalonia/solutions/01-beginner/Ex010_CompiledBinding.axaml.cs` — the stub's file with the `throw` removed and the two given view models unchanged:

```csharp
using Avalonia.Controls;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

public partial class Ex010_CompiledBinding : UserControl
{
    public Ex010_CompiledBinding() => InitializeComponent();
}

public class Ex010_BookViewModel : ReactiveObject
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private Ex010_AuthorViewModel _author = new();
    public Ex010_AuthorViewModel Author
    {
        get => _author;
        set => this.RaiseAndSetIfChanged(ref _author, value);
    }
}

public class Ex010_AuthorViewModel : ReactiveObject
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
```

### Gallery pages and the checks

- [ ] **Step 16: Write the three gallery pages**

ex006, ex007 and ex010 get pages; ex008 and ex009 have no view and get none. Follow the Task 3 Step 16 shape. `avalonia/gallery/Pages/Beginner/Ex010.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:b="clr-namespace:FeWoLearning.Avalonia.Exercises.Beginner"
             x:Class="FeWoLearning.Avalonia.Gallery.Pages.Beginner.Ex010">
  <StackPanel Spacing="12">
    <TextBlock Text="Exercise 010 — CompiledBinding" FontSize="18" FontWeight="Bold" />
    <b:Ex010_CompiledBinding Name="Subject" />
  </StackPanel>
</UserControl>
```

`avalonia/gallery/Pages/Beginner/Ex010.axaml.cs`:

```csharp
using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex010 : UserControl
{
    public Ex010()
    {
        InitializeComponent();
        this.FindControl<Ex010_CompiledBinding>("Subject")!.DataContext = new Ex010_BookViewModel
        {
            Title = "Design Patterns",
            Author = new Ex010_AuthorViewModel { Name = "Erich Gamma" },
        };
    }
}
```

`Ex006.axaml` and `Ex007.axaml` need no `DataContext`, so their code-behind is just `InitializeComponent()`, same as Task 3's `Ex005`.

- [ ] **Step 17: Extend the gallery catalog**

Append to the collection expression in `avalonia/gallery/GalleryCatalog.cs`, keeping ids ascending — `GallerySmokeTests.Ids_Are_Unique_And_Ascending` enforces that:

```csharp
        new("006", "AlignmentAndMargin", () => new Ex006()),
        new("007", "LayoutWrapPanel", () => new Ex007()),
        new("010", "CompiledBinding", () => new Ex010()),
```

- [ ] **Step 18: Red check**

Run: `dotnet test --filter "FullyQualifiedName~Ex006_|FullyQualifiedName~Ex007_|FullyQualifiedName~Ex008_|FullyQualifiedName~Ex009_|FullyQualifiedName~Ex010_"`

Expected: every test FAILS, none passes, and each message contains `TODO: ExNNN`. Pay particular attention to ex008 and ex009: they have no XAML, so a failure that is not the `NotImplementedException` means the test itself is wrong. Also confirm no failure reads `ReactiveUI has not been initialized`.

- [ ] **Step 19: Green check**

Run: `dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex006_|FullyQualifiedName~Ex007_|FullyQualifiedName~Ex008_|FullyQualifiedName~Ex009_|FullyQualifiedName~Ex010_"`

Expected: PASS, 0 failed.

- [ ] **Step 20: Full-suite check in both modes**

This is the once-per-completed-batch full run. Ten exercises plus the harness and gallery tests.

Run: `dotnet test`
Expected: every exercise test red, the two `SmokeTests` classes green, 0 build errors.

Run: `dotnet test -p:UseSolutions=true`
Expected: **everything** green, 0 failed.

Record both counts in the commit body — that is the evidence the catalog's ✅ marks stand on.

- [ ] **Step 21: Attempt a real gallery run and report honestly**

Run: `dotnet run --project gallery -p:UseSolutions=true`

If a display is available, click through entries 001–010 and confirm each renders. If the process exits with a platform or display error, **say so plainly** — the per-exercise proof is the test suite, and a headless machine is a fine reason for this step to be inconclusive. Do not claim visual verification that did not happen.

- [ ] **Step 22: Flip the catalog rows**

Rows 006–010 from `⬜` to `✅`, status line to `**Status: 10 ✅ / 90 ⬜**`.

- [ ] **Step 23: Commit**

```bash
git add avalonia/exercises/01-beginner avalonia/solutions/01-beginner avalonia/tests/01-beginner \
        avalonia/gallery/Pages avalonia/gallery/GalleryCatalog.cs avalonia/catalog.md
git commit -m "avalonia: ex006-ex010"
```

---

## Task 5: Register the track in the repo documentation

**Files:**
- Modify: `CLAUDE.md`
- Modify: `README.md`
- Modify: `docs/exercise-format.md`

**Interfaces:**
- Consumes: the verified command set and gotchas from Tasks 1–4.
- Produces: nothing code depends on.

- [ ] **Step 1: Add the track row to root `README.md`**

In the `## Languages & tracks` table, after the `rust/` row:

```markdown
| `avalonia/`| Avalonia 12 (ReactiveUI MVVM, C#)| xUnit v3 + Avalonia.Headless | **10 / 100** | ✅ .NET 10 |
```

`blazor/`, `php/` and `uno/` are also missing from this table. That is pre-existing drift — leave it alone and do not stage those lines. Note `uno/` landed in this repo while this plan was being written, so re-check the table before editing: if someone has since added those rows, just slot `avalonia/` in beside them.

- [ ] **Step 2: Add the naming row to `docs/exercise-format.md`**

In the Naming table:

```markdown
| `avalonia/`| one folder per tier, `.axaml` + code-behind + sibling test | `exercises/01-beginner/Ex001_HelloView.axaml` |
```

- [ ] **Step 3: Add `avalonia/` to the `CLAUDE.md` track table**

In the `## Current state` table:

```markdown
| `avalonia/`| 10 / 100 (verified) | 90 |
```

- [ ] **Step 4: Add the `CLAUDE.md` per-track command row**

In the `## Per-track commands` table:

```markdown
| `avalonia/`| — (restore on first `dotnet test`)     | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_HelloView` |
```

- [ ] **Step 5: Add the `CLAUDE.md` track-specific gotchas**

Under `## Track-specific gotchas`, add an `**Avalonia**` bullet covering, in this order: the `.slnx` solution and four-project layout; `solutions/` deliberately being in the build with the `UseSolutions` switch and why (mirroring the `blazor/` entry); the tier-pinned namespaces and fully qualified `x:Class`; that stubs throw `NotImplementedException` after `InitializeComponent()`; and then the seven-row constraint table from spec section 2.1 condensed to prose — `ReactiveUI.Avalonia` not `Avalonia.ReactiveUI`, `[AvaloniaFact]` not `[AvaloniaTest]`, xunit.v3 mandatory and xunit 2.x a CS0433 error, `RxVoid` not `Unit`, `using ReactiveUI.Primitives` not `System.Reactive.Linq`, `ISequencer` not `IScheduler`, and `RxAppBuilder…Build()` mandatory or every exercise is red for the wrong reason.

Then the two recurring bug classes, phrased as the other tracks' gotchas are:

> A recurring bug class here: a headless test that never shows its control in a
> `Window` asserts on children that are all still `0,0,0,0` — `Measure`/`Arrange`
> alone does not apply the control template, and neither does `ApplyTemplate()`.
> Use `ViewHarness.Show`. The second: a test that asserts only rendered text can be
> satisfied by a hard-coded literal in the XAML, so every binding exercise must also
> change the view model afterwards, call `Dispatcher.UIThread.RunJobs()`, and assert
> the text followed.

- [ ] **Step 6: Add `avalonia/` to the `CLAUDE.md` toolchain status**

Add to the ✅-verified line that Avalonia 12.1.1 with ReactiveUI 24.1.0 on .NET 10 is verified end to end, and note the pinned-version constraint: the set is coherent at 12.1.1 and must not be bumped piecemeal, because `ReactiveUI.Avalonia`'s 12.x line stops at 12.1.1 while Avalonia itself has 12.1.2.

- [ ] **Step 7: Verify the docs claims match reality**

Every number written in Step 1 and Step 3 must match the catalog. Run: `grep -c '✅' avalonia/catalog.md`
Expected: `11` — ten rows plus the legend line. If it is not 11, fix the table rows rather than the documentation.

- [ ] **Step 8: Commit**

```bash
git add CLAUDE.md README.md docs/exercise-format.md
git commit -m "avalonia: register track in repo documentation"
```

---

## Self-Review

**Spec coverage.** Every spec section maps to a task: §2 toolchain and §2.1 constraints → Task 1 Steps 3–6 plus the Global Constraints block; §3 project structure → Task 1 Steps 1–3, 7–8; §4 red/green mechanism and §4.1 commands → Task 1 Step 4 and Task 2 Step 6; §5 stub failure mode → the stub in every exercise step; §6 tier themes and beginner slugs → Task 2 Steps 2–5; §6 non-goals → Task 2 Step 6 item 7; §7 test-quality rules → `ViewHarness` in Task 1 Step 5, the named discriminator test in every exercise, and the red checks in Task 3 Step 19 and Task 4 Step 18; §8 deviation → Task 2 Step 6 item 5 and Task 5 Step 5; §9 definition of done → Task 4 Steps 18–21 and Task 5 Step 7.

**Two corrections made while reviewing.** The tests project first carried the gallery reference in two mirrored conditional `ItemGroup`s, which is pointless since both branches are identical — Task 1 Step 4 now says to collapse it to one unconditional group. And `ViewHarness.SolutionsMode` was first anchored on `Ex001_HelloView`, a type Task 1 does not yet create; it is now anchored on a `TrackMarker` present in both content projects, which also keeps it stable as exercises are added or renamed.

**Type consistency.** `ViewHarness.Show<TView>(TView, double, double)` and `ViewHarness.SolutionsMode` are defined in Task 1 Step 5 and used under those exact names in Tasks 3 and 4. `GalleryEntry(string Id, string Title, Func<Control> Create)` is defined in Task 1 Step 7 and constructed positionally in Task 3 Step 17 and Task 4 Step 17. The control names each test looks up (`TitleText`, `Row1`–`Row3`, `HeaderLeft`/`HeaderRight`/`BodyLeft`/`BodyRight`, `Banner`/`Left`/`Middle`/`Right`, `TopBar`/`BottomBar`/`SideBar`/`Body`, `Frame`/`Box`, `Item1`–`Item4`, `AuthorText`) each appear with identical spelling in the matching stub comment, solution XAML, and test.

**Geometry is measured, not derived.** Every rectangle asserted in ex003, ex004, ex005, ex006 and ex007 was produced by building that exact layout and reading `Bounds` back from a real Avalonia 12.1.1 headless run, then copied into this plan. They are not paper arithmetic. If one still disagrees during the green check, the layout in the solution differs from the one that was measured — fix the XAML, not the number.
