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

Exercises ex001–ex011 need no view and derive from `CaliburnCoreContext`. Viewless
exercises throughout the rest of the catalog also use `CaliburnCoreContext`;
exercises **with a view** derive from `CaliburnViewContext` and must be hosted with
`Show(...)` before any action can fire — the first of these is ex012. See `README.md`.

**Status: 35 ✅ / 65 ⬜**

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | NotifyByHand | `INotifyPropertyChanged` by hand, `[CallerMemberName]`, suppress on unchanged value | ✅ |
| 002 | PropertyChangedBaseBasics | `PropertyChangedBase`, `Set`, `Refresh` raises an empty property name | ✅ |
| 003 | NotifyOfPropertyChange | announcing without a backing field, why `Set` cannot help | ✅ |
| 004 | DependentProperties | one setter announcing a chain of computed properties — and not the wrong ones | ✅ |
| 005 | BindableCollectionBasics | `BindableCollection<T>`, `IsNotifying` suspension, `Refresh` as one `Reset` | ✅ |
| 006 | BindableCollectionRange | `AddRange`/`RemoveRange` raise a single `Reset`, not one event per item - even for an empty or no-op batch | ✅ |
| 007 | ScreenDisplayName | `Screen.DisplayName` defaults to the type's full name and announces on every assignment - no suppression on an unchanged value | ✅ |
| 008 | ScreenInitialize | `OnInitializedAsync` runs once, `IsInitialized` | ✅ |
| 009 | ScreenActivate | `OnActivatedAsync`/`OnDeactivateAsync`, `IsActive`, the `Activated` async event | ✅ |
| 010 | ScreenGuardClose | `CanCloseAsync` refusing a close | ✅ |
| 011 | ScreenTryClose | `TryCloseAsync` is a silent no-op with no `Parent`; deactivation with `close: true` under an active conductor | ✅ |
| 012 | ViewAwareCallbacks | `IViewAware`, `OnViewAttached`, `OnViewLoaded` | ✅ |
| 013 | ViewLocatorConvention | `FooViewModel` → `FooView`, `AssemblySource`, missing view yields a placeholder `TextBlock` | ✅ |
| 014 | ViewLocatorContext | context convention is namespace-based, not suffix-based | ✅ |
| 015 | NameTransformerRule | custom `NameTransformer` mapping rule | ✅ |
| 016 | ViewModelLocator | view-first resolution via `ViewModelLocator.LocateTypeForViewType`/`LocateForView`, constructed through `IoC`; its own `NameTransformer`, a different object from `ViewLocator`'s | ✅ |
| 017 | ViewModelBinderNames | `ViewModelBinder.Bind` matches an element's `x:Name` to a same-named view-model property; an unmatched name gets no `Binding` at all, not even a fallback one | ✅ |
| 018 | BindingConventionTwoWay | `Mode` is `TwoWay` when the view-model property has a public setter and `OneWay` otherwise - the element has no say (`ConventionManager.ApplyBindingMode` never sees it); `UpdateSourceTrigger.PropertyChanged` always - not WPF's own `LostFocus` default for `TextBox.Text` | ✅ |
| 019 | ElementConventionLookup | `ConventionManager.GetElementConvention` walks the type hierarchy (`CheckBox`→`ToggleButton`, `ComboBox`/`ListBox`→`Selector`) and never returns null for a `FrameworkElement` - an unregistered type falls back to the `Visibility` convention | ✅ |
| 020 | CustomElementConvention | `ConventionManager.AddElementConvention<T>` registers a convention scoped to one owned type, turning a nonsense `Visibility` binding into the intended one | ✅ |
| 021 | ConventionValueConverter | `ConventionManager.ApplyValueConverter` inserts WPF's `BooleanToVisibilityConverter` only where the types need bridging (`bool` onto `Visibility`) - an `int` onto `TextBlock.Text` gets no converter and no `StringFormat` | ✅ |
| 022 | ActionConventionButton | naming a `Button` after a public method wires a real `Microsoft.Xaml.Behaviors.EventTrigger`/`Caliburn.Micro.ActionMessage` at bind time, but the click only invokes the method once the view is hosted in a real window (`Show`) | ✅ |
| 023 | ActionGuardProperty | a `CanXxx` property gates `IsEnabled` from the moment the view is **loaded** (not merely bound - `Bind` alone leaves it ungated) - and by direct assignment, not a WPF `Binding` (`BindingOperations.GetBinding` on `IsEnabledProperty` returns `null` even though the gating works) | ✅ |
| 024 | ActionGuardRefresh | a guard is only as fresh as its last announcement - silently mutating the guarded state leaves `IsEnabled` stale until `NotifyOfPropertyChange` fires; a `CanXxx` **method** guard is evaluated once on load and never re-evaluated, not even by a full `Refresh()` | ✅ |
| 025 | MessageAttachExplicit | `cal:Message.Attach="Method('literal')"` (`xmlns:cal="clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro.Platform"`) wires the action explicitly, decoupling `x:Name` from the method and passing a literal string parameter | ✅ |
| 026 | ActionParameters | a bare identifier in an attach expression names an ELEMENT, not a view-model property, and `MessageBinder` coerces its convention parameter (a `TextBox`'s `Text`) to the target parameter's declared CLR type, read live at click time | ✅ |
| 027 | ActionSpecialValues | `$eventArgs`, `$dataContext`, `$source`, `$view`, plus `$this` (not one of the five `SpecialValues` keys, yet it resolves); `$view` is the view the action's target was bound to - `ViewModelBinder.Bind` (ex017) has been setting it via `Action.SetTarget` all along, which is why `$view` normally resolves to the bound root; it collapses onto `$source` only when nothing up the tree ever had a target set, as with ex025/ex026's bare `DataContext` assignment | ✅ |
| 028 | ActionTarget | `Action.SetTarget` also sets the element's `DataContext`; `Action.SetTargetWithoutContext` leaves it untouched; both make the action invocable identically | ✅ |
| 029 | SimpleContainerBasics | `RegisterSingleton` vs `RegisterPerRequest`, `GetInstance` returning `null` (not throwing) for an unregistered service, and constructor injection - which only applies once the CONSUMER type is itself registered | ✅ |
| 030 | SimpleContainerInstances | `RegisterInstance` returns the exact registered object; `RegisterHandler`'s factory runs fresh on every resolution; `GetAllInstances` counts every registration for a service, including duplicates and mixed registration kinds | ✅ |
| 031 | IoCFacade | `IoC.Get`/`GetAll`/`BuildUp` forwarding to whatever delegates are installed; `GetAll` (not `Get`) is the safe way to probe "nothing registered" - `Get` inherits the harness's `Activator.CreateInstance` fallback | ✅ |
| 032 | BootstrapperConfigure | `BootstrapperBase(useApplication: false)` headless; `Configure` does not run until `Initialize()`, which is idempotent and installs the override triad behind `IoC` | ✅ |
| 033 | ConductorSingleActive | `Conductor<T>`, activating and replacing an item - the replaced item is CLOSED (`OnDeactivateAsync(close: true)`), and a refusing `CanCloseAsync` blocks the replacement entirely | ✅ |
| 034 | ConductorOneActive | `Conductor<T>.Collection.OneActive`, `Items` (a `BindableCollection<T>`), `ActiveItem` - the outgoing item is only deactivated (`close: false`), not closed, and stays in `Items` until explicitly closed | ✅ |
| 035 | ConductorAllActive | `Conductor<T>.Collection.AllActive` - every item in `Items` active simultaneously; no `ActiveItem` property exists at all (measured by reflection) | ✅ |
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
| 089 | CollectionReconciliation | diffing an incoming list against the bound collection into minimal `Add`/`Remove` events instead of a `Reset`, to preserve selection and scroll position | ⬜ |
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
