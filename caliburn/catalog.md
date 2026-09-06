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

**Status: 60 ✅ / 40 ⬜**

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
| 031 | IoCFacade | `IoC.Get`/`GetAll`/`BuildUp` forwarding to whatever delegates are installed; `Get` can be satisfied by whatever fallback the installed delegates carry, so `GetAll` (which has none here) is the safe way to probe "nothing registered"; `BuildUp` only ever injects interface-typed properties | ✅ |
| 032 | BootstrapperConfigure | `BootstrapperBase(useApplication: false)` headless; `Configure` does not run until `Initialize()`, which is idempotent and installs the override triad behind `IoC` | ✅ |
| 033 | ConductorSingleActive | `Conductor<T>`, activating and replacing an item - the replaced item is CLOSED (`OnDeactivateAsync(close: true)`), and a refusing `CanCloseAsync` blocks the replacement entirely | ✅ |
| 034 | ConductorOneActive | `Conductor<T>.Collection.OneActive`, `Items` (a `BindableCollection<T>`), `ActiveItem` - the outgoing item is only deactivated (`close: false`), not closed, and stays in `Items` until explicitly closed | ✅ |
| 035 | ConductorAllActive | `Conductor<T>.Collection.AllActive` - every item in `Items` active simultaneously; no `ActiveItem` property exists at all (measured by reflection) | ✅ |
| 036 | ParentChildRelationship | `IChild.Parent` (object-typed), set by a conductor's `ActivateItemAsync` regardless of the conductor's own activation state; the *interface* `IConductor` declares `ActivateItemAsync`, `DeactivateItemAsync` and the `ActivationProcessed` event - `CloseItemAsync` exists as a `ScreenExtensions` extension forwarding to `DeactivateItemAsync(item, close: true)`, honouring the item's own `CanCloseAsync` either way | ✅ |
| 037 | EventAggregatorBasics | `IEventAggregator`'s raw four-method instance surface - `Subscribe`/`PublishAsync` (not the `SubscribeOnXxx`/`PublishOnXxxAsync` extensions) - and `IHandle<T>.HandleAsync(T, CancellationToken)` | ✅ |
| 038 | EventAggregatorMultipleMessages | one `Subscribe` call covers every `IHandle<T>` a subscriber implements - no per-message-type registration step | ✅ |
| 039 | EventAggregatorUnsubscribe | explicit `Unsubscribe` in `OnDeactivateAsync` as the deterministic alternative to `EventAggregator`'s WEAK subscriber references, which silently stop delivering once a forgotten subscriber is garbage-collected | ✅ |
| 040 | EventAggregatorMarshalling | `PublishAsync`'s marshal delegate wraps the entire delivery and runs exactly once per publish, regardless of subscriber count - the caller controls it, not the aggregator | ✅ |
| 041 | CoroutineBasics | `IResult`'s two members - `Execute` and the `Completed` event; forgetting to raise `Completed` stalls the coroutine forever instead of failing | ✅ |
| 042 | CoroutineSequence | a `yield return` chain drives one `IResult` at a time via `Coroutine.ExecuteAsync(IEnumerator<IResult>, ...)` - each step starts only after the previous one's `Completed` has fired | ✅ |
| 043 | CoroutineResultValue | `IResult<T>.Result` (read-only on the interface, not `Result.Value`) - reaches you only through the instance inside a `Coroutine.ExecuteAsync` sequence (still a plain `Task`), but `TaskExtensions.ExecuteAsync<TResult>(this IResult<TResult>, ...)` returns `Task<TResult>` directly for a single step | ✅ |
| 044 | CoroutineFromTask | `TaskExtensions.AsResult()`/`AsResult<T>()` adapting a `Task`/`Task<T>` into the coroutine pipeline - the coroutine genuinely waits for it, and a faulted task surfaces as `AggregateException`, not the original exception | ✅ |
| 045 | CoroutineCancellation | a sequence stops early two ways - `WasCancelled` throws `TaskCanceledException`, `Error` throws that same original exception - and no later step ever runs either way | ✅ |
| 046 | CoroutineExecutionContext | `CoroutineExecutionContext.Source`/`View`/`Target` - all settable, all `null` on a directly-constructed context, the SAME instance handed to every step in a `Coroutine.ExecuteAsync` sequence, including one a middle step mutates | ✅ |
| 047 | WindowManagerDialog | showing a dialog through an INJECTED `IWindowManager.ShowDialogAsync` and awaiting its outcome - the modal frame is why the close has to be scheduled from inside it | ✅ |
| 048 | DialogResult | `TryCloseAsync(bool?)` flowing back to the caller - measured: `true`\|`false`\|`null` in, but only `true`/`false` out, `TryCloseAsync(null)` resolving `ShowDialogAsync` to `false`, not `null` | ✅ |
| 049 | WindowManagerSettings | the settings dictionary applied to the window by reflection - `Title`/`ShowInTaskbar` stick, but `Width`/`Left` do not: `EnsureWindow` sets `SizeToContent`/a centred `WindowStartupLocation` *before* the dictionary is applied, and never touches `Width`/`Left` itself - it is WPF's own layout at `Show()` that discards them to honour those two | ✅ |
| 050 | ViewLocatorForDialogs | `ViewLocator.LocateForModelType` (the type-based lookup `WindowManager` itself uses) - a `Window`-derived located view is used AS-IS as the dialog's own window, anything else gets WRAPPED in a bare `Window`; either way `GetView()` is `null` once closed | ✅ |
| 051 | ConductorActivationChain | `ActivateItemAsync` sets `ActiveItem` (and `Parent`) immediately even on an inactive conductor, but the child's `OnActivatedAsync` only runs once the conductor itself is activated through `IActivate`/`IDeactivate` (explicit interface members, unreachable without a cast); `DeactivateAsync(close: false)` cascades that same flag to the child without clearing `ActiveItem`, so reactivating reuses it | ✅ |
| 052 | ConductorCloseGuard | `Conductor<T>.Collection.AllActive.CanCloseAsync()` asks EVERY child's own `CanCloseAsync` (each exactly once, never short-circuited by an earlier refusal) and answers true only when all agree; with the DEFAULT close strategy one refusal makes `Children` come back empty so asking closes nothing, but that is a property of the default strategy, not of the guard itself - a strategy returning a willing subset alongside `CloseCanOccur == false` (ex053's flag, ex054's own strategy) makes this same call deactivate and remove those children | ✅ |
| 053 | DefaultCloseStrategy | `DefaultCloseStrategy<T>`'s constructor flag never changes `CloseCanOccur` - one refusal always makes the whole group refuse either way; it only changes whether `Children` comes back empty (default) or holding the willing subset (`true`), and even with the flag `true`, `Children` is empty when nobody is willing | ✅ |
| 054 | CustomCloseStrategy | `ICloseStrategy<T>` has exactly one member, `ExecuteAsync(IEnumerable<T>, CancellationToken) -> Task<ICloseResult<T>>` (`CloseCanOccur` + `Children`); `ConductorBase<T>.CloseStrategy` is a plain settable property, so a hand-written majority-vote policy plugs in and is honoured in place of Caliburn's own all-or-nothing default | ✅ |
| 055 | DataErrorInfoValidation | implementing `IDataErrorInfo` on a screen flips a real `Binding`'s `ValidatesOnDataErrors` to `true` by Caliburn's own naming convention (a plain `PropertyChangedBase`/`Screen` gets `false`) - `ValidatesOnNotifyDataErrors` is `true` for both and proves nothing about the convention by itself | ✅ |
| 056 | NotifyDataErrorInfoValidation | `INotifyDataErrorInfo` does NOT change the binding - `ValidatesOnDataErrors` stays `false` and `ValidatesOnNotifyDataErrors` stays `true` (WPF's own default) for both an `INotifyDataErrorInfo` screen and a plain one, unlike ex055's `IDataErrorInfo`; the lesson lives on `HasErrors`/`GetErrors`/`ErrorsChanged` instead, including an error that only exists once an async validation task completes | ✅ |
| 057 | ValidatingScreen | `CanCloseAsync` derived from the screen's own computed validation state rather than an externally-toggled flag - and, measured, a `Screen` with no `Parent` and no attached view never has `CanCloseAsync` invoked by `TryCloseAsync` at all, so the guard is asked directly and through a close-request method that must act on its answer | ✅ |
| 058 | ItemsConventionBinding | an `ItemsControl` named after a collection binds `ItemsSource` `Mode=OneWay` (get-only) with `DisplayMemberPath`/`ItemTemplate` left untouched for plain strings - a name matching NOTHING binds nothing at all, not even the `Visibility` fallback other elements get, since `ItemsControl`'s own convention IS `ItemsSource` | ✅ |
| 059 | ActiveItemSelectedItem | the selection convention derives `SelectedItem`'s name FROM the collection's own name and wires a `ListBox.SelectedItem` to the conductor's `ActiveItem` `Mode=TwoWay` with no XAML written for it, genuinely bidirectionally; a `ContentControl` named `ActiveItem` binds Caliburn's own `View.Model` attached property `TwoWay` instead of `Content`, which gets no binding at all | ✅ |
| 060 | ItemTemplateViewLocator | `ConventionManager.DefaultItemTemplate` is assigned for any reference-type item collection other than `string` (measured: even a plain non-Caliburn POCO, not just a view model) while a value-type or `string` collection gets `null`; the template's own loaded content is a `ContentControl` with `View.Model` bound directly to the item itself (`Binding.Path` is `null`, not an empty `PropertyPath`) - which is what runs the ViewLocator per row | ✅ |
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
