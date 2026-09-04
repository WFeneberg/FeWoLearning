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

**Status: 35 ✅ / 65 ⬜**

## Beginner (001–035) — Avalonia fundamentals, ReactiveUI declarative

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | HelloView | `UserControl`, `x:DataType`, one-way binding into named `TextBlock`s | ✅ |
| 002 | LayoutStackPanel | `StackPanel` orientation and `Spacing`, stacked `Bounds` | ✅ |
| 003 | LayoutGrid | `RowDefinitions`/`ColumnDefinitions`, `Auto` vs `*`, `Grid.Row`/`Grid.Column` | ✅ |
| 004 | LayoutGridSpan | `Grid.ColumnSpan`, proportional star sizing | ✅ |
| 005 | LayoutDockPanel | `DockPanel.Dock`, `LastChildFill`, dock order | ✅ |
| 006 | AlignmentAndMargin | `HorizontalAlignment`/`VerticalAlignment`, `Margin` vs `Padding` | ✅ |
| 007 | LayoutWrapPanel | `WrapPanel` wrapping at a constrained width | ✅ |
| 008 | ObservableViewModel | `INotifyPropertyChanged` by hand, change-only notification | ✅ |
| 009 | ReactiveObjectBasics | `ReactiveObject`, `RaiseAndSetIfChanged`, `PropertyChanging` ordering | ✅ |
| 010 | CompiledBinding | nested-path re-resolution: the bound text follows when the intermediate object is replaced, not merely mutated | ✅ |
| 011 | BindingModes | `OneWay`, `TwoWay`, `OneWayToSource` | ✅ |
| 012 | TextBoxTwoWay | `TextBox.Text` two-way round-trip | ✅ |
| 013 | BindingStringFormat | `StringFormat`, invariant culture | ✅ |
| 014 | BindingFallback | `FallbackValue`, `TargetNullValue` | ✅ |
| 015 | ValueConverter | `IValueConverter` both directions | ✅ |
| 016 | ReactiveCommandBasics | `ReactiveCommand.Create`, `RxVoid`, invocation | ✅ |
| 017 | CommandCanExecute | `WhenAnyValue` feeding `canExecute`, button enablement | ✅ |
| 018 | CommandParameter | `ReactiveCommand<TParam, RxVoid>`, `CommandParameter` | ✅ |
| 019 | ButtonClickEvent | `Click` event handler versus a bound command | ✅ |
| 020 | CheckBoxBinding | `IsChecked` as `bool?`, three-state | ✅ |
| 021 | RadioGroupBinding | `RadioButton` `GroupName`, enum-backed selection | ✅ |
| 022 | SliderBinding | `Slider` `Value`/`Minimum`/`Maximum`, clamping | ✅ |
| 023 | ComboBoxSelection | `ItemsSource` plus `SelectedItem` | ✅ |
| 024 | ListBoxSelection | `SelectedIndex`, `SelectedItems`, selection mode | ✅ |
| 025 | ItemsControlTemplate | `ItemsControl` with a `DataTemplate` | ✅ |
| 026 | ObservableCollectionUpdates | add and remove reflected in the visual tree | ✅ |
| 027 | EmptyStateFallback | `IsVisible` driven by an empty collection | ✅ |
| 028 | StyleSelectors | `Style` `Selector` by type and by descendant | ✅ |
| 029 | StyleClasses | `Classes`, toggling a class at runtime | ✅ |
| 030 | PseudoClasses | `:pointerover`, `:disabled` selectors | ✅ |
| 031 | StaticAndDynamicResource | `ResourceDictionary`, `StaticResource` vs `DynamicResource` | ✅ |
| 032 | UserControlComposition | nesting a `UserControl`, exposing a CLR property | ✅ |
| 033 | StyledPropertyBasics | `StyledProperty<T>` registration, default value, styling | ✅ |
| 034 | AttachedPropertyUsage | consuming an attached property (`ToolTip.Tip`) | ✅ |
| 035 | ScrollViewerAndSizing | `ScrollViewer`, `MinWidth`/`MaxHeight` interaction | ✅ |

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
