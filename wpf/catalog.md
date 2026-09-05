# WPF — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned.

"Beginner" means **WPF** beginner, not C# beginner: ex001 registers a
`DependencyProperty`, not a `FizzBuzz`. Plain C# language drills belong to the
`dotnet/` track; WinUI's property system belongs to `uno/`.

The selection is weighted toward **practice and migration** rather than API
coverage: what actually hurts in a grown WPF solution. So DI and the generic host
instead of `App.xaml` singletons, async over the dispatcher, leaks through bindings
and event handlers, performance switches, custom controls, and Win32 interop.

Rows 001–005 deliberately cut across the beginner blocks rather than staying inside
one: dependency property, coercion, `INotifyPropertyChanged`, binding, command. After
five exercises the learner has a working hand-written mini-MVVM, which is the shape
every later tier builds on.

Stubs live in `exercises/<tier>/ExNNN_<Slug>.cs` (plus `.xaml` + `.xaml.cs` when the
exercise is about markup), their xunit tests in `tests/<tier>/ExNNN_<Slug>Tests.cs`,
reference solutions in `solutions/<tier>/`. Tier namespaces are
`FeWoLearning.Wpf.Exercises.Beginner/.Intermediate/.Advanced/.Expert`, because
`01-beginner` is not a valid C# identifier.

Every test runs headless on an STA thread with a live `Dispatcher` — see `README.md`
for how that works and what it cannot do.

Two deliberate content gaps:

- **WinForms interop.** `WindowsFormsHost` would pull WinForms into both content
  libraries for a single row, so row 088 does `HwndSource`/`HwndHost` plus P/Invoke
  instead — the harder and more transferable half.
- **Wall-clock performance.** No row asserts elapsed time; a timing test is noise on
  a loaded machine. Rows 076–080 assert *that* the mechanism fired instead —
  container identity across a scroll, `IsFrozen`, the number of measure passes an
  invalidation caused.

**Status: 35 ✅ / 65 ⬜**

## 01-beginner (001–035)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | ClrToDependencyProperty | `DependencyProperty.Register`, metadata default, `GetValue`/`SetValue`, `ClearValue` | ✅ |
| 002 | CoerceAndValidate | `ValidateValueCallback`, `CoerceValueCallback`, `PropertyChangedCallback`, `CoerceValue` | ✅ |
| 003 | ObservableViewModelBase | `INotifyPropertyChanged`, `SetProperty` + `[CallerMemberName]`, no event without a real change | ✅ |
| 004 | CodeBehindToBinding | `SetBinding`, `Binding.Path`, `BindingMode.TwoWay`, `UpdateSourceTrigger` | ✅ |
| 005 | RelayCommand | `ICommand`, `CanExecute`, `CommandManager.RequerySuggested` | ✅ |
| 006 | ReadOnlyDependencyProperty | `RegisterReadOnly`, `DependencyPropertyKey`, write through the key only | ✅ |
| 007 | AttachedProperty | `RegisterAttached`, static `GetX`/`SetX`, read from a parent | ✅ |
| 008 | MetadataInheritance | `FrameworkPropertyMetadata`, `Inherits`, `AffectsMeasure` | ✅ |
| 009 | PropertyValuePrecedence | local value vs style setter vs default, `DependencyPropertyHelper.GetValueSource` | ✅ |
| 010 | DependentPropertyFanOut | one field change raising several `PropertyChanged` names | ✅ |
| 011 | NotifyAllProperties | `PropertyChangedEventArgs(string.Empty)` semantics | ✅ |
| 012 | LegacyEventToInpc | keep a bespoke `XChanged` event firing while adding `INotifyPropertyChanged`, since only the latter reaches a real `Binding` | ✅ |
| 013 | TwoWayUpdateSourceTrigger | `UpdateSourceTrigger.Explicit`, `BindingExpression.UpdateSource` | ✅ |
| 014 | StringFormatAndFallbacks | `StringFormat`, `FallbackValue`, `TargetNullValue` | ✅ |
| 015 | RelativeSourceBinding | `RelativeSource.Self`, `FindAncestor`, `AncestorLevel` | ✅ |
| 016 | DataContextInheritance | inherited `DataContext` down a tree, and where it stops | ✅ |
| 017 | ValueConverter | `IValueConverter`, `ConvertBack`, `DependencyProperty.UnsetValue` | ✅ |
| 018 | ConverterParameterAndCulture | `ConverterParameter`, `ConverterCulture` | ✅ |
| 019 | MultiBinding | `MultiBinding`, `IMultiValueConverter` | ✅ |
| 020 | RequerySuggested | `CommandManager.InvalidateRequerySuggested`, weak handler storage | ✅ |
| 021 | RoutedCommandBinding | `RoutedCommand`, `CommandBinding`, `ApplicationCommands` | ✅ |
| 022 | StyleSetters | `Style`, `Setter`, applying a style in code | ✅ |
| 023 | ImplicitStyleByType | `TargetType` style keyed by type in a dictionary | ✅ |
| 024 | StyleBasedOn | `BasedOn` inheritance and setter override order | ✅ |
| 025 | StaticVersusDynamicResource | swap a resource at runtime; only `DynamicResource` follows | ✅ |
| 026 | MergedResourceDictionaries | `MergedDictionaries`, lookup order, last-wins | ✅ |
| 027 | DataTrigger | `Style.Triggers`, `DataTrigger`, `MultiDataTrigger` | ✅ |
| 028 | MeasureArrangeContract | `MeasureOverride`/`ArrangeOverride`, `DesiredSize` vs `RenderSize` | ✅ |
| 029 | GridStarAndAuto | star vs auto vs pixel; assert the definitions, not just rectangles | ✅ |
| 030 | MarginPaddingAlignment | `Margin`, `Padding`, `HorizontalAlignment`/`VerticalAlignment` | ✅ |
| 031 | SharedSizeGroup | `Grid.IsSharedSizeScope`, `SharedSizeGroup` | ✅ |
| 032 | ItemsControlDataTemplate | `ItemsSource`, `DataTemplate`, generated containers | ✅ |
| 033 | ObservableCollectionUpdates | `INotifyCollectionChanged` reaching the generated items | ✅ |
| 034 | DataTemplateSelector | `DataTemplateSelector.SelectTemplate` | ✅ |
| 035 | RoutedEventRouting | bubbling vs tunnelling (`Preview*`), `Handled` | ✅ |

## 02-intermediate (036–070)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | GenericHostBootstrap | `Host.CreateApplicationBuilder`, resolve the shell view model instead of `new`ing it | ⬜ |
| 037 | ViewModelFactory | `IServiceProvider`, transient view models, a factory delegate | ⬜ |
| 038 | OptionsAndConfiguration | `IOptions<T>`, configuration binding, validation on start | ⬜ |
| 039 | LoggingIntegration | `ILogger<T>` in a view model, scopes | ⬜ |
| 040 | ViewModelBaseHardening | `SetProperty` with a comparer, dependent-property fan-out, reentrancy guard | ⬜ |
| 041 | ViewModelFirstNavigation | `CurrentViewModel` + `ContentControl` + `DataType`-keyed `DataTemplate` | ⬜ |
| 042 | DialogServiceAbstraction | `IDialogService` behind an interface, asserted without a window | ⬜ |
| 043 | NotifyDataErrorInfo | `INotifyDataErrorInfo`, per-property errors, `HasErrors` | ⬜ |
| 044 | DataErrorInfoLegacy | `IDataErrorInfo`, `ValidatesOnDataErrors` | ⬜ |
| 045 | ValidationRules | `ValidationRule`, `Validation.GetErrors`, `Validation.HasError` | ⬜ |
| 046 | AsyncVoidToAsyncCommand | an async command with `IsExecuting` gating instead of `async void` | ⬜ |
| 047 | ProgressReporting | `IProgress<T>`, `Progress<T>` marshalling back to the dispatcher | ⬜ |
| 048 | DispatcherInvokeAsyncPriorities | `Dispatcher.InvokeAsync`, execution order across priorities | ⬜ |
| 049 | SynchronizationContextCapture | why `await` returns to the UI thread; the cost of `ConfigureAwait(false)` | ⬜ |
| 050 | CommandCancellation | `CancellationTokenSource`, cancelling a running command | ⬜ |
| 051 | CollectionSynchronization | `BindingOperations.EnableCollectionSynchronization` | ⬜ |
| 052 | BackgroundWorkerToTask | migrate `ProgressChanged`/`RunWorkerCompleted` to `Task` + `IProgress<T>` | ⬜ |
| 053 | CollectionViewSourceBasics | `ICollectionView`, `CurrentItem`, `MoveCurrentTo` | ⬜ |
| 054 | SortAndGroup | `SortDescriptions`, `GroupDescriptions` | ⬜ |
| 055 | FilterPredicate | `ICollectionView.Filter`, `Refresh` | ⬜ |
| 056 | DeferRefresh | batch several changes, count the refreshes collapsed | ⬜ |
| 057 | EditableObjectTransactions | `IEditableObject` begin/cancel/end edit | ⬜ |
| 058 | ControlTemplateAndTemplateBinding | retemplate a control, `TemplateBinding`, `GetTemplateChild` | ⬜ |
| 059 | VisualStateManager | `VisualStateGroup`, `VisualStateManager.GoToState` | ⬜ |
| 060 | AttachedBehavior | replace code-behind with an attached-property behavior | ⬜ |
| 061 | FreezableBrush | `Freeze`, `IsFrozen`, `CanFreeze`, sharing across threads | ⬜ |
| 062 | CustomPanel | a real `Panel` with `MeasureOverride`/`ArrangeOverride` | ⬜ |
| 063 | VirtualizationSwitches | `VirtualizingStackPanel.IsVirtualizing`, `VirtualizationMode`, `ScrollUnit` | ⬜ |
| 064 | CustomMarkupExtension | `MarkupExtension.ProvideValue`, `IProvideValueTarget` | ⬜ |
| 065 | TemplatesAsResources | `DataTemplate` keyed by `DataType`, implicit lookup | ⬜ |
| 066 | Localization | satellite resources, `ResourceManager`, switching culture at runtime | ⬜ |
| 067 | GlobalExceptionHooks | `Dispatcher.UnhandledException`, `DispatcherUnhandledExceptionFilter` | ⬜ |
| 068 | SettingsMigration | a versioned settings store upgrading an old shape | ⬜ |
| 069 | ConverterCulture | `ConverterCulture` vs `Thread.CurrentUICulture` | ⬜ |
| 070 | BindingDiagnostics | `PresentationTraceSources.TraceLevel`, catching a silent binding failure | ⬜ |

## 03-advanced (071–090)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | NonInpcSourceLeak | binding to a plain CLR property roots the source via `PropertyDescriptor` | ⬜ |
| 072 | EventHandlerAsGcRoot | a handler keeping a view model alive, proven with `WeakReference` | ⬜ |
| 073 | WeakEventManager | `PropertyChangedEventManager`, a custom `WeakEventManager` | ⬜ |
| 074 | DependencyPropertyDescriptorTrap | `AddValueChanged` never releasing its target | ⬜ |
| 075 | LeakDiagnosis | find the root in a deliberately leaking object graph | ⬜ |
| 076 | ContainerRecycling | `VirtualizationMode.Recycling`, container identity across a scroll | ⬜ |
| 077 | FrozenResources | freeze brushes and geometries shared across a tree | ⬜ |
| 078 | AsyncAndPriorityBinding | `Binding.IsAsync`, `PriorityBinding` fallback order | ⬜ |
| 079 | RenderOptions | `BitmapScalingMode`, `CachingHint`, `EdgeMode` | ⬜ |
| 080 | LayoutInvalidationCost | `AffectsMeasure` vs `AffectsRender`, counting measure passes | ⬜ |
| 081 | CrossThreadCollectionUpdates | a background producer feeding a bound collection safely | ⬜ |
| 082 | DispatcherPriorityStarvation | `Background` work starved by higher-priority queue items | ⬜ |
| 083 | BackgroundPipelineToUi | `Channel<T>` into dispatcher-batched UI updates | ⬜ |
| 084 | DefaultStyleKeyAndGeneric | `DefaultStyleKeyProperty.OverrideMetadata`, `Themes/Generic.xaml` | ⬜ |
| 085 | TemplateParts | `TemplatePartAttribute`, `OnApplyTemplate`, `GetTemplateChild` | ⬜ |
| 086 | AdornerLayer | a custom `Adorner`, `AdornerLayer.GetAdornerLayer` | ⬜ |
| 087 | CommandManagerIntegration | a control exposing `RoutedUICommand`s and its own `CommandBinding`s | ⬜ |
| 088 | HwndHostInterop | `HwndHost`, `HwndSource`, a `WndProc` hook via P/Invoke | ⬜ |
| 089 | DrawingVisualAndRenderTargetBitmap | the visual layer, render to a bitmap, assert pixels | ⬜ |
| 090 | TimersVersusClocks | `DispatcherTimer` vs `AnimationClock`/`ClockController` | ⬜ |

## 04-expert (091–100)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | ModularShellComposition | regions and a shell without a framework | ⬜ |
| 092 | ScopedViewLifetimes | a DI scope per view, disposal on navigating away | ⬜ |
| 093 | TestableNavigationService | navigation and dialogs behind interfaces, asserted headless | ⬜ |
| 094 | BindingDiagnosticsLayer | capture binding failures into a report | ⬜ |
| 095 | FeatureModulesFromConfiguration | enable and wire modules from configuration | ⬜ |
| 096 | MarkupExtensionDsl | a small markup-extension DSL over bindings | ⬜ |
| 097 | UndoRedoOverThePropertyStore | record `DependencyProperty` changes into an undo stack | ⬜ |
| 098 | TracingAndDiagnostics | an `ActivitySource`-based trace of UI operations | ⬜ |
| 099 | MigrationSeam | host WPF content behind an abstraction a modern shell can drive | ⬜ |
| 100 | CapstoneControl | property system, templates, commands and virtualization in one control | ⬜ |
