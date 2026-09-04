# Caliburn.Micro Track Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Stand up a new `caliburn/` learning track — build scaffolding, a verified WPF/STA test harness, the 100-row catalog, and the first five exercises (stub + test + reference solution), each proven red against the stubs and green against the solutions.

**Architecture:** Three projects (`exercises/`, `solutions/`, `tests/`) in `FeWoLearning.Caliburn.slnx`. `tests/` references **exactly one** content project, chosen by the `UseSolutions` MSBuild property, so `dotnet test` is the red run and `dotnet test -p:UseSolutions=true` the green run. A shared harness in `tests/_harness/` re-establishes Caliburn's process-global configuration per test and, where a view is involved, hosts it in a real off-screen `Window`.

**Tech Stack:** .NET 10 (`net10.0-windows`, `UseWPF`), Caliburn.Micro 5.0.258, xunit.v3 3.2.2, Xunit.StaFact 3.0.13, Microsoft.NET.Test.Sdk 17.14.1.

**Spec:** [`docs/superpowers/specs/2026-09-04-caliburn-track-design.md`](../specs/2026-09-04-caliburn-track-design.md)

## Global Constraints

Every task's requirements implicitly include this section. Values are copied verbatim from the spec.

- **Package versions are exact, not floating.** `Caliburn.Micro` **5.0.258**, `Xunit.StaFact` **3.0.13**, `xunit.v3` **3.2.2**, `xunit.runner.visualstudio` **3.1.4**, `Microsoft.NET.Test.Sdk` **17.14.1**.
- **`Xunit.StaFact` 4.x is forbidden.** It requires `xunit.v3.extensibility.core` 4.0.0, which has dropped the VSTest bridge; on the .NET 10 SDK the build then fails with `Testing with VSTest target is no longer supported by Microsoft.Testing.Platform`. Spec §2.1.
- **Target framework is `net10.0-windows` with `<UseWPF>true</UseWPF>`** in all three projects.
- **Tier namespaces are pinned**, because `01-beginner` is not a valid C# identifier: `FeWoLearning.Caliburn.Exercises.Beginner` / `.Intermediate` / `.Advanced` / `.Expert`. Test namespaces mirror them: `FeWoLearning.Caliburn.Tests.Beginner`.
- **Both content projects share `RootNamespace`** `FeWoLearning.Caliburn.Exercises`; only `AssemblyName` differs (`…Exercises` / `…Solutions`).
- **Stubs throw `NotImplementedException("TODO: ExNNN – …")` at runtime and must compile.** A stub that fails to build is a bug — the learner would get a build error instead of a red test.
- **No test may pass against an untouched stub.** In particular, never write a standalone `[Fact]` that asserts a purely structural fact (a base type, a member's existence): it is green before the learner starts. Fold such an assertion into a test that also exercises the throwing behaviour. Spec §8.
- **Never assert `Assert.Throws<NotImplementedException>`.**
- **The whole test assembly runs serially.** Caliburn's `IoC`, `PlatformProvider`, `AssemblySource` and `ViewLocator` are process-global.
- **The track requires an interactive desktop session** (the harness opens a real, off-screen window). It will not run in a service or session-0 context. Spec §2.3.
- **Element names never collide with `FrameworkElement` members.** `x:Name="Name"` generates a field hiding `FrameworkElement.Name` (`CS0108`). Use `UserName`-style names. Spec §2.2.
- **Commits:** the scaffolding lands as its own commit `caliburn: track scaffolding`. The five exercises land as a **single** batch commit `caliburn: ex001-ex005`, per `CLAUDE.md`'s batch-of-five convention — this overrides the per-task commit granularity this plan otherwise uses. Always stage explicit paths; never `git add -A`. The working tree contains unrelated uncommitted `uno/` work that must not be swept in.

---

## File Structure

| Path | Responsibility |
|---|---|
| `caliburn/FeWoLearning.Caliburn.slnx` | Solution, three projects |
| `caliburn/Directory.Build.props` | Redirects the `UseSolutions` build to `artifacts-solutions/` |
| `caliburn/.gitignore` | `artifacts-solutions/`, `bin/`, `obj/` |
| `caliburn/exercises/FeWoLearning.Caliburn.Exercises.csproj` | Stub library |
| `caliburn/exercises/_support/TrackMarker.cs` | Marker type so the harness can name the content assembly without depending on any one exercise |
| `caliburn/exercises/01-beginner/ExNNN_*.cs` | The stubs |
| `caliburn/solutions/FeWoLearning.Caliburn.Solutions.csproj` | Reference library, same namespaces |
| `caliburn/solutions/_support/TrackMarker.cs` | Byte-identical twin of the exercises marker |
| `caliburn/solutions/01-beginner/ExNNN_*.cs` | The reference implementations |
| `caliburn/tests/FeWoLearning.Caliburn.Tests.csproj` | Test project; references exactly one content project |
| `caliburn/tests/AssemblyInfo.cs` | `DisableTestParallelization` |
| `caliburn/tests/_harness/CaliburnCoreContext.cs` | Per-test reset of `IoC`, `PlatformProvider`, `AssemblySource` |
| `caliburn/tests/_harness/CaliburnViewContext.cs` | Adds `XamlPlatformProvider`, `Show`, `Layout`, `Load`, `Pump` |
| `caliburn/tests/_harness/HarnessSmokeTests.cs` | Proves the harness itself; not an exercise, no catalog row |
| `caliburn/tests/01-beginner/ExNNN_*Tests.cs` | The tests |
| `caliburn/catalog.md` | The 100-row ledger and work queue |
| `caliburn/README.md` | Setup, commands, traps, harness, non-goals |

**Note on `_support/`:** the spec's layout section (§3) does not mention it. It is required and is a refinement, not a deviation: the harness must register the *content* assembly with `AssemblySource` so the `ViewLocator` can find views, and it cannot do that by referencing an individual exercise type. `blazor/` uses the same `_support/` convention for shared fixtures. Like blazor's, it is never a TODO and never gets a catalog row.

---

### Task 1: Scaffolding and the test harness

Setup, configuration and the harness are folded together because the harness's smoke test is the only thing that can prove the scaffolding actually works.

**Files:**
- Create: `caliburn/FeWoLearning.Caliburn.slnx`
- Create: `caliburn/Directory.Build.props`
- Create: `caliburn/.gitignore`
- Create: `caliburn/exercises/FeWoLearning.Caliburn.Exercises.csproj`
- Create: `caliburn/exercises/_support/TrackMarker.cs`
- Create: `caliburn/solutions/FeWoLearning.Caliburn.Solutions.csproj`
- Create: `caliburn/solutions/_support/TrackMarker.cs`
- Create: `caliburn/tests/FeWoLearning.Caliburn.Tests.csproj`
- Create: `caliburn/tests/AssemblyInfo.cs`
- Create: `caliburn/tests/_harness/CaliburnCoreContext.cs`
- Create: `caliburn/tests/_harness/CaliburnViewContext.cs`
- Create: `caliburn/tests/_harness/HarnessProbeViewModel.cs`
- Create: `caliburn/tests/_harness/HarnessProbeView.xaml`
- Create: `caliburn/tests/_harness/HarnessProbeView.xaml.cs`
- Test: `caliburn/tests/_harness/HarnessSmokeTests.cs`

**Interfaces:**
- Consumes: nothing.
- Produces:
  - `FeWoLearning.Caliburn.Exercises.TrackMarker` — empty marker class, present in both content assemblies.
  - `FeWoLearning.Caliburn.Tests.CaliburnCoreContext` — abstract base; `protected SimpleContainer Container { get; }`.
  - `FeWoLearning.Caliburn.Tests.CaliburnViewContext : CaliburnCoreContext, IDisposable` — abstract base; `protected Window Show(FrameworkElement view)`, `protected static void Layout(FrameworkElement e)`, `protected static void Load(FrameworkElement root)`, `protected static void Pump(DispatcherPriority priority = DispatcherPriority.Background)`.

- [ ] **Step 1: Create the solution, props and gitignore**

`caliburn/FeWoLearning.Caliburn.slnx`:

```xml
<Solution>
  <Folder Name="/exercises/">
    <Project Path="exercises/FeWoLearning.Caliburn.Exercises.csproj" />
  </Folder>
  <Folder Name="/solutions/">
    <Project Path="solutions/FeWoLearning.Caliburn.Solutions.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/FeWoLearning.Caliburn.Tests.csproj" />
  </Folder>
</Solution>
```

`caliburn/Directory.Build.props`:

```xml
<Project>

  <!-- Redirect the solutions build to its own output tree. This is required, not
       cosmetic: exercises/ and solutions/ compile the same type names into the same
       namespaces, so sharing an obj/ tree makes the build fail with CS0579
       duplicate-attribute errors on the generated assembly info. Setting these
       conditionally inside a .csproj body is read too late - before the SDK props
       import - so they have to live here. -->
  <PropertyGroup Condition="'$(UseSolutions)' == 'true'">
    <UseArtifactsOutput>true</UseArtifactsOutput>
    <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts-solutions</ArtifactsPath>
  </PropertyGroup>

</Project>
```

`caliburn/.gitignore`:

```
artifacts-solutions/
bin/
obj/
```

- [ ] **Step 2: Create the two content projects and their marker**

`caliburn/exercises/FeWoLearning.Caliburn.Exercises.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <RootNamespace>FeWoLearning.Caliburn.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.Caliburn.Exercises</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Caliburn.Micro" Version="5.0.258" />
  </ItemGroup>

</Project>
```

`caliburn/solutions/FeWoLearning.Caliburn.Solutions.csproj` — identical except `AssemblyName`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <RootNamespace>FeWoLearning.Caliburn.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.Caliburn.Solutions</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Caliburn.Micro" Version="5.0.258" />
  </ItemGroup>

</Project>
```

`caliburn/exercises/_support/TrackMarker.cs` and `caliburn/solutions/_support/TrackMarker.cs` — **byte-identical**:

```csharp
namespace FeWoLearning.Caliburn.Exercises;

/// <summary>
/// Names the content assembly for the test harness, which registers it with
/// <c>AssemblySource</c> so Caliburn's ViewLocator can find views. A marker rather than
/// any real exercise type, so the harness does not depend on one particular exercise.
/// Not an exercise; never gets a catalog.md row.
/// </summary>
public sealed class TrackMarker;
```

- [ ] **Step 3: Create the test project**

`caliburn/tests/FeWoLearning.Caliburn.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <RootNamespace>FeWoLearning.Caliburn.Tests</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <!-- Xunit.StaFact supplies [WpfFact]: an STA thread with a real WPF Dispatcher.
       Version 3.0.13, NOT 4.x - 4.x depends on xunit.v3 4.0.0, which has dropped the
       VSTest bridge, and `dotnet test` then fails on the .NET 10 SDK. 3.0.13 depends on
       xunit.v3.extensibility.core 3.0.0, so it sits on the same xunit.v3 3.2.2 that
       avalonia/ already runs. -->
  <ItemGroup>
    <PackageReference Include="Xunit.StaFact" Version="3.0.13" />
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <!-- Exactly one of the two content projects, never both: that is what keeps the
       identical namespaces and type names from colliding. `dotnet test` is the red run
       against the stubs, `dotnet test -p:UseSolutions=true` the green run against the
       reference implementations. Same mechanism as avalonia/, blazor/ and uno/. -->
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Caliburn.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Caliburn.Solutions.csproj" />
  </ItemGroup>

</Project>
```

`caliburn/tests/AssemblyInfo.cs`:

```csharp
// Caliburn's configuration is process-global (IoC, PlatformProvider, AssemblySource,
// ViewLocator) and every WPF test owns a Dispatcher. Parallel tests trample each other.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
```

- [ ] **Step 4: Write the harness**

`caliburn/tests/_harness/CaliburnCoreContext.cs`:

```csharp
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Base class for exercises with no view. Caliburn is configured through process-global
/// statics; a real app sets them once in a Bootstrapper, but a test run has to
/// re-establish them for EVERY test, because the previous test left its own behind.
/// </summary>
public abstract class CaliburnCoreContext
{
    protected SimpleContainer Container { get; } = new();

    protected CaliburnCoreContext()
    {
        // Reset to the inline provider. A previous [WpfFact] may have installed the XAML
        // one, whose captured Dispatcher belongs to an STA thread that no longer pumps --
        // NotifyOfPropertyChange would then block until the call is cancelled, surfacing
        // as a TaskCanceledException from deep inside PropertyChangedBase.
        PlatformProvider.Current = new DefaultPlatformProvider();

        // The ViewLocator searches these assemblies. TrackMarker names whichever content
        // assembly this run is built against; the test assembly carries the harness's own
        // probe view.
        AssemblySource.Instance.Clear();
        AssemblySource.Instance.Add(typeof(TrackMarker).Assembly);
        AssemblySource.Instance.Add(typeof(CaliburnCoreContext).Assembly);

        // Not optional even with no UI at all: Coroutine.BeginExecute calls IoC.BuildUp,
        // so an otherwise pure-core coroutine test throws "IoC is not initialized".
        IoC.GetInstance = (service, key) =>
            Container.GetInstance(service, key) ?? Activator.CreateInstance(service)!;
        IoC.GetAllInstances = service => Container.GetAllInstances(service, null);
        IoC.BuildUp = Container.BuildUp;
    }
}
```

`caliburn/tests/_harness/CaliburnViewContext.cs`:

```csharp
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Base class for exercises with a view. Valid ONLY under [WpfFact]/[WpfTheory]:
/// XamlPlatformProvider captures Dispatcher.CurrentDispatcher in its constructor, so it
/// has to be built on the STA test thread that will actually pump it.
/// </summary>
public abstract class CaliburnViewContext : CaliburnCoreContext, IDisposable
{
    readonly List<Window> _windows = [];

    protected CaliburnViewContext() => PlatformProvider.Current = new XamlPlatformProvider();

    /// <summary>Measure/arrange only. Enough for geometry and for guard evaluation.</summary>
    protected static void Layout(FrameworkElement e)
    {
        e.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        e.Arrange(new Rect(e.DesiredSize));
        e.UpdateLayout();
    }

    /// <summary>
    /// Raises Loaded across the tree. FrameworkElement.LoadedEvent is a *direct* routed
    /// event, so raising it on the root alone never reaches the root's children.
    /// Use this only when a Loaded callback is the subject; actions need <see cref="Show"/>.
    /// </summary>
    protected static void Load(FrameworkElement root)
    {
        Layout(root);
        foreach (var e in SelfAndDescendants(root))
            e.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, e));
    }

    static IEnumerable<FrameworkElement> SelfAndDescendants(DependencyObject root)
    {
        if (root is FrameworkElement fe) yield return fe;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            foreach (var child in SelfAndDescendants(VisualTreeHelper.GetChild(root, i)))
                yield return child;
    }

    /// <summary>
    /// Hosts the view in a real window, parked off-screen at zero opacity, closed on
    /// dispose. THIS IS THE ONLY WAY TO EXERCISE AN ACTION: Caliburn's actions ride on
    /// Microsoft.Xaml.Behaviors triggers, which refuse to resolve their source until the
    /// element has a PresentationSource. Measure/Arrange does not supply one, ApplyTemplate
    /// does not, and neither does raising Loaded by hand -- only a real window does.
    /// </summary>
    protected Window Show(FrameworkElement view)
    {
        var w = new Window
        {
            Content = view,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Opacity = 0,
            Width = 400,
            Height = 300,
            Left = -32000,
            Top = -32000,
        };
        w.Show();
        Pump(DispatcherPriority.Loaded);
        _windows.Add(w);
        return w;
    }

    /// <summary>Drains the dispatcher queue. Assert only after pumping.</summary>
    protected static void Pump(DispatcherPriority priority = DispatcherPriority.Background) =>
        Dispatcher.CurrentDispatcher.Invoke(() => { }, priority);

    public void Dispose()
    {
        foreach (var w in _windows) w.Close();
        GC.SuppressFinalize(this);
    }
}
```

- [ ] **Step 5: Write the harness's own fixture**

`caliburn/tests/_harness/HarnessProbeViewModel.cs`:

```csharp
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Fixture for the harness smoke test only. Never an exercise. The property is UserName,
/// not Name: an element named "Name" generates a field that hides FrameworkElement.Name
/// and the build warns CS0108.
/// </summary>
public class HarnessProbeViewModel : PropertyChangedBase
{
    string _userName = "Ada";

    public string UserName
    {
        get => _userName;
        set
        {
            if (Set(ref _userName, value)) NotifyOfPropertyChange(nameof(CanSayHello));
        }
    }

    /// <summary>Caliburn's guard convention: gates the IsEnabled of the SayHello button.</summary>
    public bool CanSayHello => UserName.Length > 3;

    public int Greetings { get; private set; }

    public void SayHello() => Greetings++;
}
```

`caliburn/tests/_harness/HarnessProbeView.xaml`:

```xml
<UserControl x:Class="FeWoLearning.Caliburn.Tests.HarnessProbeView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <StackPanel>
    <TextBox x:Name="UserName" />
    <Button x:Name="SayHello" Content="Go" />
  </StackPanel>
</UserControl>
```

`caliburn/tests/_harness/HarnessProbeView.xaml.cs`:

```csharp
using System.Windows.Controls;

namespace FeWoLearning.Caliburn.Tests;

public partial class HarnessProbeView : UserControl
{
    public HarnessProbeView() => InitializeComponent();
}
```

- [ ] **Step 6: Write the harness smoke test**

`caliburn/tests/_harness/HarnessSmokeTests.cs`:

```csharp
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Proves the harness itself, so it is green in the real tree from the first commit
/// instead of first exercised at ex012. Not an exercise; no catalog.md row.
/// </summary>
public class HarnessCoreSmokeTests : CaliburnCoreContext
{
    [Fact]
    public void Core_context_initializes_IoC()
    {
        // Coroutine.BeginExecute calls IoC.BuildUp; without initialization this throws
        // "IoC is not initialized" and every coroutine exercise fails for the wrong reason.
        Assert.NotNull(IoC.BuildUp);
        Assert.NotNull(IoC.GetInstance);
    }
}

/// <summary>
/// The view half. Every test here is [WpfFact]: CaliburnViewContext installs
/// XamlPlatformProvider, which captures the current thread's Dispatcher, so a plain
/// [Fact] in this class would bind it to a thread that never pumps.
/// </summary>
public class HarnessSmokeTests : CaliburnViewContext
{
    [WpfFact]
    public void Default_control_templates_resolve_without_an_application()
    {
        var button = new Button { Content = "Hello" };
        button.ApplyTemplate();
        Layout(button);

        Assert.True(button.DesiredSize.Width > 0, $"width was {button.DesiredSize.Width}");
        Assert.True(button.DesiredSize.Height > 0, $"height was {button.DesiredSize.Height}");
    }

    [WpfFact]
    public void ViewLocator_finds_the_view_by_convention()
    {
        var view = ViewLocator.LocateForModel(new HarnessProbeViewModel(), null, null);

        Assert.IsType<HarnessProbeView>(view);
    }

    [WpfFact]
    public void ViewModelBinder_binds_by_name_in_both_directions()
    {
        var vm = new HarnessProbeViewModel();
        var view = new HarnessProbeView { DataContext = vm };
        ViewModelBinder.Bind(vm, view, null);
        Show(view);

        var box = (TextBox)view.FindName("UserName")!;
        Assert.Equal("Ada", box.Text);

        vm.UserName = "Grace";
        Pump();
        Assert.Equal("Grace", box.Text);
    }

    [WpfFact]
    public void Show_makes_guards_gate_and_actions_fire()
    {
        var vm = new HarnessProbeViewModel();
        var view = new HarnessProbeView { DataContext = vm };
        ViewModelBinder.Bind(vm, view, null);
        Show(view);

        var button = (Button)view.FindName("SayHello")!;

        // "Ada".Length > 3 is false, so the guard must have disabled it.
        Assert.False(button.IsEnabled);

        vm.UserName = "Grace";
        Pump();
        Assert.True(button.IsEnabled);

        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
        Pump();
        Assert.Equal(1, vm.Greetings);
    }
}
```

- [ ] **Step 7: Run the harness smoke test against the stubs**

Run from inside `caliburn/`:

```
dotnet test --filter FullyQualifiedName~Harness
```

Expected: **5 passed, 0 failed.** If `Show_makes_guards_gate_and_actions_fire` fails on `Greetings`, the window is not supplying a `PresentationSource` — do not "fix" it by asserting less; re-check that `Show` is called and that `w.Show()` actually ran.

- [ ] **Step 8: Run the harness smoke test against the solutions**

```
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Harness
```

Expected: **5 passed, 0 failed**, and a separate `artifacts-solutions/` output tree appears. If the build fails with `CS0579`, `Directory.Build.props` is not being picked up.

- [ ] **Step 9: Verify both full builds are clean**

```
dotnet build
dotnet build -p:UseSolutions=true
```

Expected: both succeed with **0 errors and 0 warnings**. A `CS0108` warning means an element was named after a `FrameworkElement` member.

- [ ] **Step 10: Commit**

```bash
git add caliburn/.gitignore caliburn/Directory.Build.props caliburn/FeWoLearning.Caliburn.slnx caliburn/exercises caliburn/solutions caliburn/tests
git commit -m "caliburn: track scaffolding"
```

---

### Task 2: Catalog and README

**Files:**
- Create: `caliburn/catalog.md`
- Create: `caliburn/README.md`

**Interfaces:**
- Consumes: the project layout from Task 1.
- Produces: the work queue every later batch reads. Slugs defined here are binding — Task 3–7 file names must match them exactly.

- [ ] **Step 1: Write `caliburn/catalog.md`**

Header, then a single 100-row table. Rows 001–005 stay ⬜ for now; Task 8 flips them.

````markdown
# Caliburn.Micro — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned.

"Beginner" means **Caliburn** beginner, not C# or WPF beginner: ex001 writes
`INotifyPropertyChanged` by hand so ex002 can show what `PropertyChangedBase`
replaces. Plain C# language drills belong to the `dotnet/` track.

**Caliburn.Micro is the subject; WPF is the carrier.** There are no exercises on
`ControlTemplate` authoring, animations, custom-drawn controls or virtualization —
none of those teach Caliburn. See `README.md` for the full non-goals list.

Stubs live in `exercises/<tier>/ExNNN_<Slug>.cs` (plus `.xaml` + `.xaml.cs` when the
exercise is about a view), their xunit tests in `tests/<tier>/ExNNN_<Slug>Tests.cs`,
reference solutions in `solutions/<tier>/`. Tier namespaces are
`FeWoLearning.Caliburn.Exercises.Beginner/.Intermediate/.Advanced/.Expert`, because
`01-beginner` is not a valid C# identifier.

Exercises ex001–ex011 need no view and derive from `CaliburnCoreContext`. From ex012
on they derive from `CaliburnViewContext` and must be hosted with `Show(...)` before
any action can fire — see `README.md`.

**Status: 0 ✅ / 100 ⬜**

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | NotifyByHand | `INotifyPropertyChanged` by hand, `[CallerMemberName]`, suppress on unchanged value | ⬜ |
| 002 | PropertyChangedBaseBasics | `PropertyChangedBase`, `Set`, `Refresh` raises an empty property name | ⬜ |
| 003 | NotifyOfPropertyChange | announcing without a backing field, why `Set` cannot help | ⬜ |
| 004 | DependentProperties | one setter announcing a chain of computed properties — and not the wrong ones | ⬜ |
| 005 | BindableCollectionBasics | `BindableCollection<T>`, `IsNotifying` suspension, `Refresh` as one `Reset` | ⬜ |
| 006 | BindableCollectionRange | `AddRange`/`RemoveRange` raise a single `Reset`, not one event per item | ⬜ |
| 007 | ScreenDisplayName | `Screen.DisplayName`, announced like any other property | ⬜ |
| 008 | ScreenInitialize | `OnInitializeAsync` runs once, `IsInitialized` | ⬜ |
| 009 | ScreenActivate | `OnActivateAsync`/`OnDeactivateAsync`, `IsActive`, the `Activated` async event | ⬜ |
| 010 | ScreenGuardClose | `CanCloseAsync` refusing a close | ⬜ |
| 011 | ScreenTryClose | `TryCloseAsync`, deactivation with `close: true` | ⬜ |
| 012 | ViewAwareCallbacks | `IViewAware`, `OnViewAttached`, `OnViewLoaded` | ⬜ |
| 013 | ViewLocatorConvention | `FooViewModel` → `FooView`, `AssemblySource` | ⬜ |
| 014 | ViewLocatorContext | context-specific view variants | ⬜ |
| 015 | NameTransformerRule | custom `NameTransformer` mapping rule | ⬜ |
| 016 | ViewModelLocator | view-first resolution | ⬜ |
| 017 | ViewModelBinderNames | element named after a property binds to it | ⬜ |
| 018 | BindingConventionTwoWay | convention-chosen binding mode and update trigger | ⬜ |
| 019 | ElementConventionLookup | what `ConventionManager` knows out of the box | ⬜ |
| 020 | CustomElementConvention | registering an `ElementConvention` for a new control | ⬜ |
| 021 | ConventionValueConverter | automatic converter application | ⬜ |
| 022 | ActionConventionButton | button named after a method invokes it | ⬜ |
| 023 | ActionGuardProperty | `CanXxx` gating `IsEnabled` | ⬜ |
| 024 | ActionGuardRefresh | re-announcing a guard so the button re-evaluates | ⬜ |
| 025 | MessageAttachExplicit | `cal:Message.Attach` instead of the naming convention | ⬜ |
| 026 | ActionParameters | passing parameters to an action | ⬜ |
| 027 | ActionSpecialValues | `$eventArgs`, `$dataContext`, `$source` | ⬜ |
| 028 | ActionTarget | `Action.Target` vs `Action.TargetWithoutContext` | ⬜ |
| 029 | SimpleContainerBasics | `Singleton`, `PerRequest`, resolution | ⬜ |
| 030 | SimpleContainerInstances | instance and handler registration | ⬜ |
| 031 | IoCFacade | `IoC.Get`, `GetAll`, `BuildUp` | ⬜ |
| 032 | BootstrapperConfigure | `BootstrapperBase`, `Configure`, container wiring | ⬜ |
| 033 | ConductorSingleActive | `Conductor<T>`, activating and replacing an item | ⬜ |
| 034 | ConductorOneActive | `Conductor<T>.Collection.OneActive`, `Items`, `ActiveItem` | ⬜ |
| 035 | ConductorAllActive | `Conductor<T>.Collection.AllActive` | ⬜ |
| 036 | ParentChildRelationship | `IChild`, `Parent`, set by the conductor | ⬜ |
| 037 | EventAggregatorBasics | `Subscribe`, `PublishAsync`, `IHandle<T>` | ⬜ |
| 038 | EventAggregatorMultipleMessages | one subscriber handling several message types | ⬜ |
| 039 | EventAggregatorUnsubscribe | unsubscribing on deactivation | ⬜ |
| 040 | EventAggregatorMarshalling | the publish marshaller delegate | ⬜ |
| 041 | CoroutineBasics | `IResult`, the `Completed` event | ⬜ |
| 042 | CoroutineSequence | `yield return` chains and their order | ⬜ |
| 043 | CoroutineResultValue | `IResult<T>` and `Result.Value` | ⬜ |
| 044 | CoroutineFromTask | adapting a `Task` into an `IResult` | ⬜ |
| 045 | CoroutineCancellation | stopping a sequence on failure | ⬜ |
| 046 | CoroutineExecutionContext | `Target` and `View` on the context | ⬜ |
| 047 | WindowManagerDialog | `ShowDialogAsync` | ⬜ |
| 048 | DialogResult | `TryCloseAsync(bool?)` flowing back to the caller | ⬜ |
| 049 | WindowManagerSettings | the settings dictionary applied to the window | ⬜ |
| 050 | ViewLocatorForDialogs | locating a window-shaped view | ⬜ |
| 051 | ConductorActivationChain | activating a conductor activates its active child | ⬜ |
| 052 | ConductorCloseGuard | `CanCloseAsync` cascading through children | ⬜ |
| 053 | DefaultCloseStrategy | how the built-in strategy decides | ⬜ |
| 054 | CustomCloseStrategy | writing an `ICloseStrategy` | ⬜ |
| 055 | DataErrorInfoValidation | `IDataErrorInfo` on a screen | ⬜ |
| 056 | NotifyDataErrorInfoValidation | `INotifyDataErrorInfo`, asynchronous errors | ⬜ |
| 057 | ValidatingScreen | validation gating `CanClose` | ⬜ |
| 058 | ItemsConventionBinding | `ItemsControl` named after a collection | ⬜ |
| 059 | ActiveItemSelectedItem | `ActiveItem` ↔ `SelectedItem` convention | ⬜ |
| 060 | ItemTemplateViewLocator | the ViewLocator inside a `DataTemplate` | ⬜ |
| 061 | AsyncGuardRefresh | async work flipping a guard | ⬜ |
| 062 | ExecuteOnUIThread | `Execute`, `PlatformProvider`, marshalling | ⬜ |
| 063 | LogManagerCustomLogger | plugging a logger into `LogManager` | ⬜ |
| 064 | DesignTimeDetection | `Execute.InDesignMode` and design-time data | ⬜ |
| 065 | CustomIoCDelegates | replacing `SimpleContainer` through the `IoC` delegates | ⬜ |
| 066 | MicrosoftDIBootstrapper | Caliburn on `Microsoft.Extensions.DependencyInjection` | ⬜ |
| 067 | BootstrapperLifecycle | `Configure` and `OnStartup` ordering | ⬜ |
| 068 | ActionMessageCustomization | the `ActionMessage.InvokeAction` hook | ⬜ |
| 069 | CustomSpecialValues | registering a new `$value` | ⬜ |
| 070 | ActionFilters | preconditions wrapped around an action | ⬜ |
| 071 | CustomViewLocatorStrategy | a namespace/folder-driven locator | ⬜ |
| 072 | CustomViewModelBinderConvention | extending `ViewModelBinder` | ⬜ |
| 073 | BindingScopeInTemplates | `BindingScope` finding named elements in templates | ⬜ |
| 074 | ConventionsInsideDataTemplate | conventions applied to templated items | ⬜ |
| 075 | CustomConductor | a conductor written from scratch | ⬜ |
| 076 | ConductorBaseWithActiveItem | extending the built-in base | ⬜ |
| 077 | NavigationOverConductor | a navigation service on top of a conductor | ⬜ |
| 078 | MessageRoutingToParent | routing a message up the parent chain | ⬜ |
| 079 | EventAggregatorLeaks | why a forgotten subscriber leaks, and the fix | ⬜ |
| 080 | BackgroundWorkMarshalling | background work marshalled back to the UI thread | ⬜ |
| 081 | TestingScreensWithoutViews | testing a screen with no view at all | ⬜ |
| 082 | TestingCloseStrategies | testing a close cascade deterministically | ⬜ |
| 083 | CustomResultLibrary | a reusable `IResult` set | ⬜ |
| 084 | ScreenStatePersistence | saving and restoring screen state | ⬜ |
| 085 | MultiShellComposition | more than one shell in one process | ⬜ |
| 086 | AsyncInitializationOrdering | `OnInitializeAsync` vs `OnActivateAsync` ordering | ⬜ |
| 087 | CustomAttachedConventions | conventions driven by an attached property | ⬜ |
| 088 | NestedViewModelGuards | a guard depending on a nested view model | ⬜ |
| 089 | CollectionSuspension | `IsNotifying` under load, and `Refresh` | ⬜ |
| 090 | ConventionPerformance | the cost of convention lookup, and caching it | ⬜ |
| 091 | ModularShellAssemblySource | a modular shell over `AssemblySource` | ⬜ |
| 092 | DynamicPluginLoading | loading view/view-model assemblies at runtime | ⬜ |
| 093 | ConventionBasedDiscovery | a bootstrapper discovering by convention | ⬜ |
| 094 | GenericHostIntegration | Caliburn on the .NET generic host | ⬜ |
| 095 | CustomConventionEngine | a complete convention set of your own | ⬜ |
| 096 | ActionInterception | interception around `ActionMessage` | ⬜ |
| 097 | UndoRedoOverPropertyChangedBase | undo/redo built on property notifications | ⬜ |
| 098 | AsyncValidationPipeline | composite asynchronous validation | ⬜ |
| 099 | CapstoneMultiScreenApp | conductor + event aggregator + coroutines + dialogs | ⬜ |
| 100 | ConventionsVsSourceGenerators | Caliburn's conventions against source-generator MVVM | ⬜ |
````

- [ ] **Step 2: Write `caliburn/README.md`**

Write it with these sections, in this order. The substance for each is in the spec at
the section named — reproduce it in the README rather than cross-referencing a document
that lives outside the track folder, because the README is what a learner opens.

1. **Title and one-paragraph intro** — Caliburn.Micro 5 on WPF, .NET 10, no templates or
   IDE plugin needed. State plainly that **Caliburn.Micro is the subject and WPF is only
   the carrier** (spec §1).
2. **Requirements** — .NET 10 SDK, Windows, and **an interactive desktop session**: the
   harness opens a real (off-screen, invisible) window, so the track cannot run as a
   service or in session 0 (spec §2.3).
3. **Commands** — the table below (spec §4.1).
4. **Layout** — the `exercises/` / `solutions/` / `tests/` tree and the `UseSolutions`
   switch; note that `tests/_harness/` and `_support/` are not exercises and get no
   catalog row (spec §3, §4).
5. **How the harness works** — `CaliburnCoreContext` vs `CaliburnViewContext`, and the
   four helpers `Show` / `Layout` / `Load` / `Pump`. Lead with the rule that matters most:
   **an action only fires when the view is hosted with `Show`** (spec §5).
6. **What the harness cannot do** — the non-goals: real window management and DPI, OS-level
   input, `ControlTemplate` authoring, animations, custom-drawn controls, virtualization,
   theming, localization, Blend design-time tooling beyond `Execute.InDesignMode`
   (spec §7). Say explicitly that a green test is not proof of desktop behaviour.
7. **Traps** — a table of the six probe constraints (spec §2.2) plus the `Xunit.StaFact`
   4.x version trap and why the pin is 3.0.13 (spec §2.1).
8. **Why `solutions/` is in the build** — the deliberate deviation from the repo-wide
   convention, and the instruction not to "fix" it back (spec §9).

The commands table:

| Purpose | Command |
|---|---|
| Red — stubs | `dotnet test` |
| Green — reference solutions | `dotnet test -p:UseSolutions=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |
| One exercise, green | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |

- [ ] **Step 3: Commit**

```bash
git add caliburn/catalog.md caliburn/README.md
git commit -m "caliburn: catalog and readme"
```

---

### Task 3: ex001 NotifyByHand

**Files:**
- Create: `caliburn/exercises/01-beginner/Ex001_NotifyByHand.cs`
- Create: `caliburn/solutions/01-beginner/Ex001_NotifyByHand.cs`
- Test: `caliburn/tests/01-beginner/Ex001_NotifyByHandTests.cs`

**Interfaces:**
- Consumes: `CaliburnCoreContext` from Task 1.
- Produces: `FeWoLearning.Caliburn.Exercises.Beginner.Ex001_NotifyByHand` — `string FirstName { get; set; }`, `string LastName { get; set; }`, `string FullName { get; }`, `event PropertyChangedEventHandler? PropertyChanged`.

- [ ] **Step 1: Write the stub**

```csharp
// Exercise 001 - Notify By Hand (beginner).
// Goal:   Write INotifyPropertyChanged once, by hand, so ex002 can show you what
//         Caliburn's PropertyChangedBase replaces.
// Drills: INotifyPropertyChanged, [CallerMemberName], suppressing the event when the
//         value did not change, announcing a computed property whose inputs moved.
// Passes: dotnet test --filter FullyQualifiedName~Ex001_
//
// Deliberately NOT derived from PropertyChangedBase. This is the only exercise in the
// track that writes the plumbing by hand.

using System.ComponentModel;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex001_NotifyByHand : INotifyPropertyChanged
{
    private string _firstName = "";
    private string _lastName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string FirstName
    {
        get => _firstName;
        set => throw new NotImplementedException("TODO: Ex001 - store _firstName and announce it");
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set => throw new NotImplementedException("TODO: Ex001 - store _lastName and announce it");
    }

    /// <summary>
    /// Computed, so it has no setter of its own to announce from. A binding to FullName
    /// goes stale unless the two setters above announce it as well.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    // TODO: add one helper both setters use. Take the field by `ref`, compare with
    // EqualityComparer<T>.Default, return whether the value actually moved, and let the
    // property name arrive through [CallerMemberName] so no setter passes a string literal.
}
```

- [ ] **Step 2: Write the test**

```csharp
using System.ComponentModel;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex001_NotifyByHandTests : CaliburnCoreContext
{
    private static List<string?> Record(Ex001_NotifyByHand vm)
    {
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [Fact]
    public void Setting_A_Property_Stores_The_Value()
    {
        var vm = new Ex001_NotifyByHand { FirstName = "Ada", LastName = "Lovelace" };

        Assert.Equal("Ada", vm.FirstName);
        Assert.Equal("Lovelace", vm.LastName);
        Assert.Equal("Ada Lovelace", vm.FullName);
    }

    [Fact]
    public void Setting_A_Property_Announces_It_By_Name()
    {
        // Asserted here rather than in a [Fact] of its own: a standalone structural
        // assertion would be green against the untouched stub, which the track forbids.
        Assert.False(
            typeof(Ex001_NotifyByHand).IsSubclassOf(typeof(PropertyChangedBase)),
            "ex001 is the hand-written version on purpose - ex002 is the PropertyChangedBase one.");

        var vm = new Ex001_NotifyByHand();
        var names = Record(vm);

        vm.FirstName = "Ada";

        Assert.Contains("FirstName", names);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var vm = new Ex001_NotifyByHand { FirstName = "Ada", LastName = "Lovelace" };
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        // Every redundant notification re-evaluates each binding on the property and
        // re-runs its converters, for nothing.
        Assert.Empty(names);
    }

    [Fact]
    public void Computed_FullName_Is_Announced_When_Its_Inputs_Move()
    {
        var vm = new Ex001_NotifyByHand();
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        Assert.Equal(2, names.Count(n => n == nameof(Ex001_NotifyByHand.FullName)));
    }

    [Fact]
    public void Announced_Names_Exist_On_The_Type()
    {
        var vm = new Ex001_NotifyByHand();
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        // A typo in a hand-written string literal fails silently at runtime; the compiler
        // never sees it. [CallerMemberName] is how you stop writing them.
        Assert.NotEmpty(names);
        Assert.All(names, name =>
        {
            Assert.False(string.IsNullOrEmpty(name));
            Assert.NotNull(typeof(Ex001_NotifyByHand).GetProperty(name!));
        });
    }
}
```

- [ ] **Step 3: Write the reference solution**

Same header comment as the stub, minus the two `TODO` blocks.

```csharp
// Exercise 001 - Notify By Hand (beginner).
// Goal:   Write INotifyPropertyChanged once, by hand, so ex002 can show you what
//         Caliburn's PropertyChangedBase replaces.
// Drills: INotifyPropertyChanged, [CallerMemberName], suppressing the event when the
//         value did not change, announcing a computed property whose inputs moved.
// Passes: dotnet test --filter FullyQualifiedName~Ex001_

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex001_NotifyByHand : INotifyPropertyChanged
{
    private string _firstName = "";
    private string _lastName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (Set(ref _firstName, value)) Raise(nameof(FullName));
        }
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set
        {
            if (Set(ref _lastName, value)) Raise(nameof(FullName));
        }
    }

    /// <summary>
    /// Computed, so it has no setter of its own to announce from. A binding to FullName
    /// goes stale unless the two setters above announce it as well.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Returns whether the value actually moved, so callers can chain the dependent
    /// notifications without repeating the comparison.
    /// </summary>
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;

        field = value;
        Raise(propertyName);
        return true;
    }

    private void Raise(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

- [ ] **Step 4: Red check**

```
dotnet test --filter FullyQualifiedName~Ex001_
```

Expected: **5 failed, 0 passed.** Every failure must be the exercise's own `NotImplementedException`. `Setting_A_Property_Stores_The_Value` fails in the object initializer; the rest fail on the first assignment.

- [ ] **Step 5: Green check**

```
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_
```

Expected: **5 passed, 0 failed.**

---

### Task 4: ex002 PropertyChangedBaseBasics

**Files:**
- Create: `caliburn/exercises/01-beginner/Ex002_PropertyChangedBaseBasics.cs`
- Create: `caliburn/solutions/01-beginner/Ex002_PropertyChangedBaseBasics.cs`
- Test: `caliburn/tests/01-beginner/Ex002_PropertyChangedBaseBasicsTests.cs`

**Interfaces:**
- Consumes: `CaliburnCoreContext`.
- Produces: `FeWoLearning.Caliburn.Exercises.Beginner.Ex002_PropertyChangedBaseBasics : PropertyChangedBase` — `string FirstName { get; set; }`, `string LastName { get; set; }`, `string FullName { get; }`, `void RefreshAll()`.

**Measured behaviour this task depends on** (verified 2026-09-04 against Caliburn.Micro 5.0.258): `PropertyChangedBase.Set` suppresses when the value is unchanged; `Refresh()` raises exactly one `PropertyChanged` whose `PropertyName` is the **empty string**, not `null`.

- [ ] **Step 1: Write the stub**

```csharp
// Exercise 002 - PropertyChangedBase Basics (beginner).
// Goal:   The same view model as ex001, on Caliburn's base class instead of by hand.
// Drills: PropertyChangedBase, the protected Set helper, NotifyOfPropertyChange for a
//         computed property, and Refresh() as "everything changed".
// Passes: dotnet test --filter FullyQualifiedName~Ex002_
//
// Compare this file with ex001 when you are done. That is the point of the pair.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex002_PropertyChangedBaseBasics : PropertyChangedBase
{
    private string _firstName = "";
    private string _lastName = "";

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string FirstName
    {
        get => _firstName;
        // TODO: use the inherited Set(ref field, value) helper. It already compares, already
        // announces, and returns whether the value moved - use that to announce FullName too.
        set => throw new NotImplementedException("TODO: Ex002 - set _firstName via Set(...)");
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set => throw new NotImplementedException("TODO: Ex002 - set _lastName via Set(...)");
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Re-announces every property at once, for the case where a view model changed
    /// underneath the bindings and naming each property individually is not worth it.
    /// </summary>
    public void RefreshAll() =>
        throw new NotImplementedException("TODO: Ex002 - announce all properties in one event");
}
```

- [ ] **Step 2: Write the test**

```csharp
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex002_PropertyChangedBaseBasicsTests : CaliburnCoreContext
{
    private static List<string?> Record(Ex002_PropertyChangedBaseBasics vm)
    {
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [Fact]
    public void Uses_The_Caliburn_Base_Class()
    {
        // Folded together with a behavioural assertion: on its own, the base-type check
        // would be green against the untouched stub.
        Assert.True(typeof(Ex002_PropertyChangedBaseBasics).IsSubclassOf(typeof(PropertyChangedBase)));

        var vm = new Ex002_PropertyChangedBaseBasics { FirstName = "Ada" };

        Assert.Equal("Ada", vm.FirstName);
    }

    [Fact]
    public void Setting_A_Property_Announces_It_By_Name()
    {
        var vm = new Ex002_PropertyChangedBaseBasics();
        var names = Record(vm);

        vm.FirstName = "Ada";

        Assert.Contains("FirstName", names);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var vm = new Ex002_PropertyChangedBaseBasics { FirstName = "Ada", LastName = "Lovelace" };
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        // Set already does this comparison for you - that is half of what it is for.
        Assert.Empty(names);
    }

    [Fact]
    public void Computed_FullName_Is_Announced_When_Its_Inputs_Move()
    {
        var vm = new Ex002_PropertyChangedBaseBasics();
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        Assert.Equal(2, names.Count(n => n == nameof(Ex002_PropertyChangedBaseBasics.FullName)));
        Assert.Equal("Ada Lovelace", vm.FullName);
    }

    [Fact]
    public void RefreshAll_Announces_Everything_In_One_Event()
    {
        var vm = new Ex002_PropertyChangedBaseBasics();
        var names = Record(vm);

        vm.RefreshAll();

        // An empty property name is the INotifyPropertyChanged convention for "all of
        // them"; Caliburn's Refresh() raises exactly one such event. Note: empty, not null.
        Assert.Single(names);
        Assert.Equal(string.Empty, names[0]);
    }
}
```

- [ ] **Step 3: Write the reference solution**

```csharp
// Exercise 002 - PropertyChangedBase Basics (beginner).
// Goal:   The same view model as ex001, on Caliburn's base class instead of by hand.
// Drills: PropertyChangedBase, the protected Set helper, NotifyOfPropertyChange for a
//         computed property, and Refresh() as "everything changed".
// Passes: dotnet test --filter FullyQualifiedName~Ex002_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex002_PropertyChangedBaseBasics : PropertyChangedBase
{
    private string _firstName = "";
    private string _lastName = "";

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (Set(ref _firstName, value)) NotifyOfPropertyChange(nameof(FullName));
        }
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set
        {
            if (Set(ref _lastName, value)) NotifyOfPropertyChange(nameof(FullName));
        }
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Re-announces every property at once, for the case where a view model changed
    /// underneath the bindings and naming each property individually is not worth it.
    /// </summary>
    public void RefreshAll() => Refresh();
}
```

- [ ] **Step 4: Red check**

```
dotnet test --filter FullyQualifiedName~Ex002_
```

Expected: **5 failed, 0 passed**, each from this exercise's `NotImplementedException`.

- [ ] **Step 5: Green check**

```
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex002_
```

Expected: **5 passed, 0 failed.**

---

### Task 5: ex003 NotifyOfPropertyChange

**Files:**
- Create: `caliburn/exercises/01-beginner/Ex003_NotifyOfPropertyChange.cs`
- Create: `caliburn/solutions/01-beginner/Ex003_NotifyOfPropertyChange.cs`
- Test: `caliburn/tests/01-beginner/Ex003_NotifyOfPropertyChangeTests.cs`

**Interfaces:**
- Consumes: `CaliburnCoreContext`.
- Produces: `FeWoLearning.Caliburn.Exercises.Beginner.Ex003_NotifyOfPropertyChange : PropertyChangedBase` — constructor `Ex003_NotifyOfPropertyChange(IDictionary<string, string> store)`, `string Theme { get; set; }`, `bool IsDark { get; }`.

The lesson: `Set(ref …)` needs a backing **field**. When the value lives somewhere else — a settings store, a service, a parent object — you announce by hand, and you own the equality check too.

- [ ] **Step 1: Write the stub**

```csharp
// Exercise 003 - NotifyOfPropertyChange (beginner).
// Goal:   Announce a property whose value does not live in a field of this class.
// Drills: NotifyOfPropertyChange, why Set(ref ...) cannot help here, doing the equality
//         check yourself, announcing a dependent computed property.
// Passes: dotnet test --filter FullyQualifiedName~Ex003_
//
// Set(ref field, value) needs a backing FIELD to take by reference. This view model keeps
// its value in an injected store instead - which is the normal case as soon as a setting
// is shared, persisted, or owned by a service.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex003_NotifyOfPropertyChange(IDictionary<string, string> store) : PropertyChangedBase
{
    private const string Key = "Theme";

    /// <summary>Reads through to the store; "light" when the store has nothing yet.</summary>
    public string Theme
    {
        get => store.TryGetValue(Key, out var value) ? value : "light";

        // TODO: write the value into the store under Key, then announce Theme and IsDark.
        // Announce nothing at all when the incoming value equals the current one - there is
        // no Set(...) here to do that comparison for you.
        set => throw new NotImplementedException("TODO: Ex003 - store the theme and announce it");
    }

    /// <summary>Computed from <see cref="Theme"/>, so only the Theme setter can announce it.</summary>
    public bool IsDark => Theme == "dark";
}
```

- [ ] **Step 2: Write the test**

```csharp
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex003_NotifyOfPropertyChangeTests : CaliburnCoreContext
{
    private static (Ex003_NotifyOfPropertyChange Vm, Dictionary<string, string> Store, List<string?> Names) Make()
    {
        var store = new Dictionary<string, string>();
        var vm = new Ex003_NotifyOfPropertyChange(store);
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return (vm, store, names);
    }

    [Fact]
    public void Setting_Writes_Through_To_The_Store()
    {
        var (vm, store, _) = Make();

        // Nothing in the store yet, so the getter's fallback is what a view would show.
        Assert.Equal("light", vm.Theme);
        Assert.False(vm.IsDark);

        vm.Theme = "dark";

        // The store is the single source of truth - there is no field shadowing it.
        Assert.Equal("dark", store["Theme"]);
        Assert.Equal("dark", vm.Theme);
    }

    [Fact]
    public void Reads_Through_To_The_Store_Rather_Than_A_Field()
    {
        var store = new Dictionary<string, string> { ["Theme"] = "dark" };
        var vm = new Ex003_NotifyOfPropertyChange(store);

        // Constructed over a populated store, the getter already reflects it. Then prove
        // the setter writes back into that same store rather than into a field.
        Assert.Equal("dark", vm.Theme);
        Assert.True(vm.IsDark);

        vm.Theme = "light";

        Assert.Equal("light", store["Theme"]);
    }

    [Fact]
    public void Setting_Announces_Theme_And_IsDark()
    {
        var (vm, _, names) = Make();

        vm.Theme = "dark";

        Assert.Contains(nameof(Ex003_NotifyOfPropertyChange.Theme), names);
        Assert.Contains(nameof(Ex003_NotifyOfPropertyChange.IsDark), names);
        Assert.True(vm.IsDark);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var (vm, _, names) = Make();
        vm.Theme = "dark";
        names.Clear();

        vm.Theme = "dark";

        // No Set(...) to lean on: this comparison is yours to write.
        Assert.Empty(names);
    }

    [Fact]
    public void Writing_The_Default_Over_An_Empty_Store_Announces_Nothing()
    {
        var (vm, _, names) = Make();

        vm.Theme = "light";

        // Theme already reads "light", so nothing moved - even though the store was empty.
        Assert.Empty(names);
    }
}
```

- [ ] **Step 3: Write the reference solution**

```csharp
// Exercise 003 - NotifyOfPropertyChange (beginner).
// Goal:   Announce a property whose value does not live in a field of this class.
// Drills: NotifyOfPropertyChange, why Set(ref ...) cannot help here, doing the equality
//         check yourself, announcing a dependent computed property.
// Passes: dotnet test --filter FullyQualifiedName~Ex003_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex003_NotifyOfPropertyChange(IDictionary<string, string> store) : PropertyChangedBase
{
    private const string Key = "Theme";

    /// <summary>Reads through to the store; "light" when the store has nothing yet.</summary>
    public string Theme
    {
        get => store.TryGetValue(Key, out var value) ? value : "light";
        set
        {
            // Compare against what the getter would return, not against the raw store
            // entry: writing "light" into an empty store changes nothing observable.
            if (Theme == value) return;

            store[Key] = value;
            NotifyOfPropertyChange(nameof(Theme));
            NotifyOfPropertyChange(nameof(IsDark));
        }
    }

    /// <summary>Computed from <see cref="Theme"/>, so only the Theme setter can announce it.</summary>
    public bool IsDark => Theme == "dark";
}
```

- [ ] **Step 4: Red check**

```
dotnet test --filter FullyQualifiedName~Ex003_
```

Expected: **5 failed, 0 passed.** Every test drives the setter, so the stub's
`NotImplementedException` reaches all five - including the two that also assert on the
getter. A test that read only the getter would be **green against the untouched stub**,
which the track forbids; that is why those assertions are folded in rather than standing
alone.

- [ ] **Step 5: Green check**

```
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex003_
```

Expected: **5 passed, 0 failed.**

---

### Task 6: ex004 DependentProperties

**Files:**
- Create: `caliburn/exercises/01-beginner/Ex004_DependentProperties.cs`
- Create: `caliburn/solutions/01-beginner/Ex004_DependentProperties.cs`
- Test: `caliburn/tests/01-beginner/Ex004_DependentPropertiesTests.cs`

**Interfaces:**
- Consumes: `CaliburnCoreContext`.
- Produces: `FeWoLearning.Caliburn.Exercises.Beginner.Ex004_DependentProperties : PropertyChangedBase` — `int Quantity { get; set; }`, `decimal UnitPrice { get; set; }`, `decimal DiscountPercent { get; set; }`, `decimal Subtotal { get; }`, `decimal Discount { get; }`, `decimal Total { get; }`.

The lesson is as much about what you must **not** announce: `DiscountPercent` moves `Discount` and `Total`, but `Subtotal` is untouched, and announcing it anyway is a real (if invisible) cost.

- [ ] **Step 1: Write the stub**

```csharp
// Exercise 004 - Dependent Properties (beginner).
// Goal:   Announce exactly the computed properties a setter actually moved - no more.
// Drills: chains of computed properties, announcing dependents from a setter, and not
//         announcing the ones that did not change.
// Passes: dotnet test --filter FullyQualifiedName~Ex004_
//
// Subtotal <- Quantity, UnitPrice
// Discount <- Subtotal, DiscountPercent
// Total    <- Subtotal, Discount
//
// Over-announcing is not free: every announcement re-evaluates each binding on that
// property and re-runs its converters. Announce what moved, and only what moved.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex004_DependentProperties : PropertyChangedBase
{
    private int _quantity = 1;
    private decimal _unitPrice;
    private decimal _discountPercent;

    public int Quantity
    {
        get => _quantity;
        // TODO: set via Set(...), and when it moved announce Subtotal, Discount and Total.
        set => throw new NotImplementedException("TODO: Ex004 - set _quantity and announce its dependents");
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set => throw new NotImplementedException("TODO: Ex004 - set _unitPrice and announce its dependents");
    }

    public decimal DiscountPercent
    {
        get => _discountPercent;
        // TODO: this one does NOT move Subtotal. Announce only what it really changed.
        set => throw new NotImplementedException("TODO: Ex004 - set _discountPercent and announce its dependents");
    }

    public decimal Subtotal => Quantity * UnitPrice;

    public decimal Discount => Subtotal * DiscountPercent / 100m;

    public decimal Total => Subtotal - Discount;
}
```

- [ ] **Step 2: Write the test**

```csharp
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex004_DependentPropertiesTests : CaliburnCoreContext
{
    private static List<string?> Record(Ex004_DependentProperties vm)
    {
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [Fact]
    public void Computes_The_Whole_Chain()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, UnitPrice = 25m, DiscountPercent = 10m };

        Assert.Equal(100m, vm.Subtotal);
        Assert.Equal(10m, vm.Discount);
        Assert.Equal(90m, vm.Total);
    }

    [Fact]
    public void Quantity_Announces_Itself_And_Every_Dependent()
    {
        var vm = new Ex004_DependentProperties { UnitPrice = 25m, DiscountPercent = 10m };
        var names = Record(vm);

        vm.Quantity = 4;

        Assert.Contains(nameof(Ex004_DependentProperties.Quantity), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Subtotal), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Discount), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Total), names);
    }

    [Fact]
    public void UnitPrice_Announces_Itself_And_Every_Dependent()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, DiscountPercent = 10m };
        var names = Record(vm);

        vm.UnitPrice = 25m;

        Assert.Contains(nameof(Ex004_DependentProperties.UnitPrice), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Subtotal), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Discount), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Total), names);
    }

    [Fact]
    public void DiscountPercent_Does_Not_Announce_Subtotal()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, UnitPrice = 25m };
        var names = Record(vm);

        vm.DiscountPercent = 10m;

        Assert.Contains(nameof(Ex004_DependentProperties.DiscountPercent), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Discount), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Total), names);

        // Subtotal is Quantity * UnitPrice. A discount cannot move it, and announcing it
        // anyway would re-evaluate every binding on Subtotal for nothing.
        Assert.DoesNotContain(nameof(Ex004_DependentProperties.Subtotal), names);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, UnitPrice = 25m, DiscountPercent = 10m };
        var names = Record(vm);

        vm.Quantity = 4;
        vm.UnitPrice = 25m;
        vm.DiscountPercent = 10m;

        // Set(...) suppresses the property itself; the dependents must be suppressed too,
        // which is why the announcements belong inside the `if (Set(...))`.
        Assert.Empty(names);
    }
}
```

- [ ] **Step 3: Write the reference solution**

```csharp
// Exercise 004 - Dependent Properties (beginner).
// Goal:   Announce exactly the computed properties a setter actually moved - no more.
// Drills: chains of computed properties, announcing dependents from a setter, and not
//         announcing the ones that did not change.
// Passes: dotnet test --filter FullyQualifiedName~Ex004_
//
// Subtotal <- Quantity, UnitPrice
// Discount <- Subtotal, DiscountPercent
// Total    <- Subtotal, Discount

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex004_DependentProperties : PropertyChangedBase
{
    private int _quantity = 1;
    private decimal _unitPrice;
    private decimal _discountPercent;

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (Set(ref _quantity, value)) NotifySubtotalChanged();
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (Set(ref _unitPrice, value)) NotifySubtotalChanged();
        }
    }

    public decimal DiscountPercent
    {
        get => _discountPercent;
        set
        {
            // Deliberately not NotifySubtotalChanged: a discount does not move Subtotal.
            if (Set(ref _discountPercent, value))
            {
                NotifyOfPropertyChange(nameof(Discount));
                NotifyOfPropertyChange(nameof(Total));
            }
        }
    }

    public decimal Subtotal => Quantity * UnitPrice;

    public decimal Discount => Subtotal * DiscountPercent / 100m;

    public decimal Total => Subtotal - Discount;

    /// <summary>Subtotal moved, so everything downstream of it moved with it.</summary>
    private void NotifySubtotalChanged()
    {
        NotifyOfPropertyChange(nameof(Subtotal));
        NotifyOfPropertyChange(nameof(Discount));
        NotifyOfPropertyChange(nameof(Total));
    }
}
```

- [ ] **Step 4: Red check**

```
dotnet test --filter FullyQualifiedName~Ex004_
```

Expected: **5 failed, 0 passed.**

- [ ] **Step 5: Green check**

```
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex004_
```

Expected: **5 passed, 0 failed.**

---

### Task 7: ex005 BindableCollectionBasics

**Files:**
- Create: `caliburn/exercises/01-beginner/Ex005_BindableCollectionBasics.cs`
- Create: `caliburn/solutions/01-beginner/Ex005_BindableCollectionBasics.cs`
- Test: `caliburn/tests/01-beginner/Ex005_BindableCollectionBasicsTests.cs`

**Interfaces:**
- Consumes: `CaliburnCoreContext`.
- Produces: `FeWoLearning.Caliburn.Exercises.Beginner.Ex005_BindableCollectionBasics : PropertyChangedBase` — `BindableCollection<string> Items { get; }`, `void AddItem(string item)`, `void ReplaceAll(IEnumerable<string> items)`.

**Measured behaviour this task depends on** (verified 2026-09-04 against Caliburn.Micro 5.0.258): a plain `Add` raises `CollectionChanged` with action `Add`; setting `IsNotifying = false` suppresses `CollectionChanged` entirely while still mutating the collection; `Refresh()` raises exactly one `CollectionChanged` with action `Reset`.

- [ ] **Step 1: Write the stub**

```csharp
// Exercise 005 - BindableCollection Basics (beginner).
// Goal:   Rebuild a whole list without making the UI redraw once per item.
// Drills: BindableCollection<T>, IsNotifying as a notification suspension switch, and
//         Refresh() as the single Reset that tells the view "start over".
// Passes: dotnet test --filter FullyQualifiedName~Ex005_
//
// A bound ItemsControl reacts to every CollectionChanged event. Clearing a list of 500 and
// re-adding 500 naively is 501 CollectionChanged events and 501 rounds of container
// generation. Suspending notification and raising one Reset at the end is one.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex005_BindableCollectionBasics : PropertyChangedBase
{
    /// <summary>The bound collection. Created once - never reassign it, or bindings break.</summary>
    public BindableCollection<string> Items { get; } = new();

    /// <summary>Appends one item, announcing it the ordinary way.</summary>
    public void AddItem(string item) =>
        throw new NotImplementedException("TODO: Ex005 - append the item to Items");

    /// <summary>
    /// Replaces the entire contents, costing the view exactly ONE notification no matter
    /// how many items are involved.
    /// </summary>
    public void ReplaceAll(IEnumerable<string> items) =>
        throw new NotImplementedException("TODO: Ex005 - swap the contents in a single notification");

    // TODO for ReplaceAll: switch Items.IsNotifying off, clear, add the new items one at a
    // time, switch it back on, then call Items.Refresh() to raise the single Reset.
    // Leave IsNotifying true again afterwards even though nothing here throws - the next
    // caller depends on it.
}
```

- [ ] **Step 2: Write the test**

```csharp
using System.Collections.Specialized;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex005_BindableCollectionBasicsTests : CaliburnCoreContext
{
    private static List<NotifyCollectionChangedAction> Record(Ex005_BindableCollectionBasics vm)
    {
        var actions = new List<NotifyCollectionChangedAction>();
        vm.Items.CollectionChanged += (_, e) => actions.Add(e.Action);
        return actions;
    }

    [Fact]
    public void AddItem_Appends_And_Raises_One_Add()
    {
        var vm = new Ex005_BindableCollectionBasics();
        var actions = Record(vm);

        vm.AddItem("milk");

        Assert.Equal(new[] { "milk" }, vm.Items);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Add }, actions);
    }

    [Fact]
    public void ReplaceAll_Puts_The_New_Contents_In_Place()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.AddItem("milk");

        vm.ReplaceAll(new[] { "bread", "butter", "jam" });

        Assert.Equal(new[] { "bread", "butter", "jam" }, vm.Items);
    }

    [Fact]
    public void ReplaceAll_Costs_The_View_Exactly_One_Notification()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.AddItem("milk");
        var actions = Record(vm);

        vm.ReplaceAll(new[] { "bread", "butter", "jam" });

        // A naive Clear-then-Add-each would be four events here, and four rounds of
        // container generation in a bound ItemsControl. Reset means "re-read everything", once.
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }

    [Fact]
    public void ReplaceAll_Leaves_Notification_Switched_Back_On()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.ReplaceAll(new[] { "bread" });
        var actions = Record(vm);

        vm.AddItem("jam");

        // Suspension is a switch, not a scope: forgetting to flip it back leaves the
        // collection permanently silent and the bug surfaces far from here.
        Assert.True(vm.Items.IsNotifying);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Add }, actions);
    }

    [Fact]
    public void ReplaceAll_With_Nothing_Empties_The_List()
    {
        var vm = new Ex005_BindableCollectionBasics();
        vm.AddItem("milk");
        var actions = Record(vm);

        vm.ReplaceAll(Array.Empty<string>());

        Assert.Empty(vm.Items);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }
}
```

- [ ] **Step 3: Write the reference solution**

```csharp
// Exercise 005 - BindableCollection Basics (beginner).
// Goal:   Rebuild a whole list without making the UI redraw once per item.
// Drills: BindableCollection<T>, IsNotifying as a notification suspension switch, and
//         Refresh() as the single Reset that tells the view "start over".
// Passes: dotnet test --filter FullyQualifiedName~Ex005_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex005_BindableCollectionBasics : PropertyChangedBase
{
    /// <summary>The bound collection. Created once - never reassign it, or bindings break.</summary>
    public BindableCollection<string> Items { get; } = new();

    /// <summary>Appends one item, announcing it the ordinary way.</summary>
    public void AddItem(string item) => Items.Add(item);

    /// <summary>
    /// Replaces the entire contents, costing the view exactly ONE notification no matter
    /// how many items are involved.
    /// </summary>
    public void ReplaceAll(IEnumerable<string> items)
    {
        Items.IsNotifying = false;
        try
        {
            Items.Clear();
            foreach (var item in items) Items.Add(item);
        }
        finally
        {
            // try/finally because a half-suspended collection is silent forever, and the
            // symptom shows up in whatever binds to it rather than here.
            Items.IsNotifying = true;
        }

        Items.Refresh();
    }
}
```

- [ ] **Step 4: Red check**

```
dotnet test --filter FullyQualifiedName~Ex005_
```

Expected: **5 failed, 0 passed.**

- [ ] **Step 5: Green check**

```
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex005_
```

Expected: **5 passed, 0 failed.**

---

### Task 8: Batch verification, catalog flip, repo documentation

**Files:**
- Modify: `caliburn/catalog.md` (rows 001–005, status line)
- Modify: `CLAUDE.md`
- Modify: `README.md`
- Modify: `docs/exercise-format.md`

**Interfaces:**
- Consumes: everything from Tasks 1–7.
- Produces: the committed batch.

- [ ] **Step 1: Full red run**

```
dotnet test
```

Expected: **25 failed, 5 passed** — all 25 exercise tests (5 exercises x 5 tests) red,
and only the 5 harness smoke tests green. No exercise test may pass here.

Then confirm each failure individually: it must come from that exercise's own
`NotImplementedException`. A message mentioning `IoC is not initialized` or
`TaskCanceledException` is a **harness fault**, not a learner TODO, and invalidates the
red run - fix the harness before going further.

- [ ] **Step 2: Full green run**

```
dotnet test -p:UseSolutions=true
```

Expected: **30 passed, 0 failed.**

- [ ] **Step 3: Flip the catalog rows**

Change rows 001–005 from `| ⬜ |` to `| ✅ |` and the status line to:

```
**Status: 5 ✅ / 95 ⬜**
```

- [ ] **Step 4: Update the repo-level documentation**

In `CLAUDE.md`:
- per-track command table: add `| `caliburn/` | — (restore on first `dotnet test`) | `dotnet test` | `dotnet test --filter FullyQualifiedName~Ex001_` |`
- the note about `-p:UseSolutions=true` currently naming `blazor/` and `uno/`: add `caliburn/`
- track table under "Current state": add `| `caliburn/` | 5 / 100 (verified) | 95 |`
- toolchain status: record Caliburn.Micro 5.0.258 + Xunit.StaFact 3.0.13 + xunit.v3 3.2.2 on .NET 10.0.400, verified 2026-09-04, and the interactive-desktop-session requirement
- track-specific gotchas: a `**Caliburn**` bullet carrying the StaFact 4.x trap, the six probe constraints, `Show`-or-no-actions, and the `solutions/`-in-build deviation

In `README.md`, add a track row:

```
| `caliburn/` | Caliburn.Micro 5 MVVM on WPF (C#) | xUnit v3 + StaFact | **5 / 100** | ✅ .NET 10 |
```

In `docs/exercise-format.md`, add a naming row:

```
| `caliburn/`| one file per exercise, tier-wide namespace, test in a separate `tests/` project | `exercises/01-beginner/Ex001_NotifyByHand.cs` |
```

**Out of scope, do not touch:** `php/` is missing from every `CLAUDE.md` table and from root `README.md`, and `uno/` is missing from root `README.md`. That drift predates this work.

- [ ] **Step 5: Commit the batch**

Stage explicit paths only. The working tree contains unrelated uncommitted `uno/` work.

```bash
git add caliburn/exercises/01-beginner caliburn/solutions/01-beginner caliburn/tests/01-beginner caliburn/catalog.md CLAUDE.md README.md docs/exercise-format.md
git commit -m "caliburn: ex001-ex005"
```

- [ ] **Step 6: Confirm nothing unrelated was committed**

```bash
git show --stat HEAD
git status --short
```

Expected: the commit touches only `caliburn/`, `CLAUDE.md`, `README.md` and `docs/exercise-format.md`; `git status` still shows the untracked `uno/` files, untouched.

---

## Notes for whoever runs the next batch

- `catalog.md` is the work queue. The next assignment is ex006–ex010; do not re-inventory the disk.
- ex012 is the first exercise needing a view. It derives from `CaliburnViewContext`, must use `[WpfFact]` rather than `[Fact]`, and must call `Show(view)` before asserting anything about an action.
- Read one finished exercise from the tier as a style template — once per tier, not once per batch.
