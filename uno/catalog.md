# Uno Platform — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned.

"Beginner" means **Uno/WinUI** beginner, not C# beginner: ex001 registers a
`DependencyProperty`, not a `FizzBuzz`. Plain C# language drills belong to the
`dotnet/` track.

Stubs live in `exercises/<tier>/ExNNN_<Slug>.cs` (plus `.xaml` + `.xaml.cs` when the
exercise is about markup), their xunit tests in `tests/<tier>/ExNNN_<Slug>Tests.cs`,
reference solutions in `solutions/<tier>/`. Tier namespaces are
`FeWoLearning.Uno.Exercises.Beginner/.Intermediate/.Advanced/.Expert`, because
`01-beginner` is not a valid C# identifier.

Every test runs headless against the real Skia `Uno.UI` — see `README.md` for how
that works and what it costs.

`ItemsControl` and `ListView` never realise their items without a live visual tree,
so collection exercises are built on `ItemsRepeater` — see "What the harness cannot
do" in `README.md`.

**Status: 35 ✅ / 65 ⬜**

## Beginner (001–035) — the object model, XAML, layout, binding

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | HelloProperty | `DependencyProperty.Register`, `GetValue`/`SetValue`, `partial` requirement | ✅ |
| 002 | PropertyChangedCallback | `PropertyMetadata` callback, old vs new value | ✅ |
| 003 | AttachedProperty | `RegisterAttached`, static `GetX`/`SetX` accessors | ✅ |
| 004 | ObservableModel | `INotifyPropertyChanged`, raise only on a real change | ✅ |
| 005 | OneWayBinding | `SetBinding`, `Binding.Path`/`Source`, source pushes to target | ✅ |
| 006 | TwoWayBinding | `BindingMode.TwoWay`, target writes back to the source | ✅ |
| 007 | ValueConverter | `IValueConverter`, `Convert`/`ConvertBack`, culture argument | ✅ |
| 008 | ConverterParameter | parameter-driven conversion, invariant parsing | ✅ |
| 009 | XamlUserControl | `x:Class`, `InitializeComponent`, `x:Name`, code-behind | ✅ |
| 010 | GridPlacement | `RowDefinitions`/`ColumnDefinitions`, `Grid.Row`/`Grid.Column` | ✅ |
| 011 | StarSizing | `*` vs `Auto` vs absolute, `ActualWidth` after arrange | ✅ |
| 012 | StackPanelSpacing | `Orientation`, `Spacing`, how `DesiredSize` accumulates | ✅ |
| 013 | MarginPadding | margin vs padding in the measure pass | ✅ |
| 014 | AlignmentStretch | `HorizontalAlignment`/`VerticalAlignment` vs `Stretch` | ✅ |
| 015 | CanvasPositioning | `Canvas.Left`/`Top`/`ZIndex`, no layout negotiation | ✅ |
| 016 | RelativePanelAlign | `RelativePanel.RightOf`/`AlignBottomWith` constraint solving | ✅ |
| 017 | VisibilityCollapsed | `Visibility.Collapsed` leaves layout, `Opacity` does not | ✅ |
| 018 | DataContextInheritance | `DataContext` flows down the tree, local override | ✅ |
| 019 | DataTemplateBasics | `DataTemplate`, `ContentControl.ContentTemplate` | ✅ |
| 020 | ItemsRepeaterBinding | `ItemsRepeater`, `ItemsSource` + `ItemTemplate`, one element per item | ✅ |
| 021 | ObservableCollectionUpdates | `INotifyCollectionChanged` reaching the visual tree | ✅ |
| 022 | StaticResource | `ResourceDictionary`, `{StaticResource}` lookup walk | ✅ |
| 023 | ThemeResource | `ThemeDictionaries`, `RequestedTheme`, re-evaluation | ✅ |
| 024 | ImplicitStyle | `Style` with `TargetType` and no `x:Key` | ✅ |
| 025 | StyleInheritance | `BasedOn`, setter override order | ✅ |
| 026 | ControlTemplateBasics | `ControlTemplate`, `TemplateBinding` | ✅ |
| 027 | TemplatePartLookup | `OnApplyTemplate`, `GetTemplateChild`, the `PART_` contract | ✅ |
| 028 | VisualStateGroups | `VisualStateManager.GoToState`, state setters | ✅ |
| 029 | EventHandlers | `Click` and `RoutedEventArgs`, invoking a control through its automation peer | ✅ |
| 030 | PropertyChangeObservers | `RegisterPropertyChangedCallback`, watching a property you do not own | ✅ |
| 031 | CommandBinding | `ICommand`, `CanExecute`, `CanExecuteChanged` | ✅ |
| 032 | ValuePrecedence | local value vs style setter vs default value | ✅ |
| 033 | ClearValueAndDefault | `ClearValue` falls back to the style, then to the default | ✅ |
| 034 | SizeReporting | `DesiredSize` vs `ActualWidth`/`ActualHeight` vs `Width` | ✅ |
| 035 | XBindBasics | `x:Bind` with `x:DataType`, compile-time paths, OneTime by default | ✅ |

## Intermediate (036–070) — controls, custom layout, MVVM/MVUX, app services

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | CustomTemplatedControl | `Control` subclass, `DefaultStyleKey`, style dictionary | ⬜ |
| 037 | TemplatePartContract | a part contract that survives a missing part | ⬜ |
| 038 | ControlVisualStates | property changes driving `GoToState` | ⬜ |
| 039 | CustomPanelMeasure | `MeasureOverride`, honouring the available size | ⬜ |
| 040 | CustomPanelArrange | `ArrangeOverride`, final rects, clipping | ⬜ |
| 041 | WrapPanel | a full custom panel: wrapping, line breaking, desired size | ⬜ |
| 042 | AttachedBehavior | attached property that subscribes and unsubscribes events | ⬜ |
| 043 | InheritedContext | propagating a value down a subtree without a global | ⬜ |
| 044 | MarkupExtension | a custom `MarkupExtension` used from XAML | ⬜ |
| 045 | ItemsRepeaterLayout | `ItemsRepeater` + `ItemsSource` + an explicit `Layout` | ⬜ |
| 046 | ElementFactoryContract | a custom `ElementFactory`: `GetElement`/`RecycleElement`, reuse over rebuild | ⬜ |
| 047 | ItemContainerStates | an item container control with Selected/Unselected visual states | ⬜ |
| 048 | ObservableObjectBase | an INPC base with `[CallerMemberName]` and an equality guard | ⬜ |
| 049 | AsyncCommand | async `ICommand`, busy flag, exception capture | ⬜ |
| 050 | InputValidation | `INotifyDataErrorInfo`, per-property errors | ⬜ |
| 051 | FunctionalConverter | a reusable converter parameterised by a delegate | ⬜ |
| 052 | BindingFallbacks | `FallbackValue`, `TargetNullValue`, failed paths | ⬜ |
| 053 | RelativeSourceBinding | `RelativeSource` `Self` and `TemplatedParent` | ⬜ |
| 054 | ElementNameBinding | `ElementName` bindings and name scopes | ⬜ |
| 055 | XLoadDeferral | `x:Load`, realisation, `FindName` before and after | ⬜ |
| 056 | ResourceDictionaryMerging | merged dictionaries and lookup precedence | ⬜ |
| 057 | ThemeSwitching | switching theme at runtime: what re-evaluates, what does not | ⬜ |
| 058 | CultureAwareFormatting | binding under an explicit culture, `FlowDirection` mirroring | ⬜ |
| 059 | FrameNavigation | `Frame.Navigate`/`GoBack`, `OnNavigatedTo` | ⬜ |
| 060 | NavigationParameters | typed parameters, back stack, state on return | ⬜ |
| 061 | SettingsStorage | `ApplicationData.LocalSettings` round-trip | ⬜ |
| 062 | FileStorageAsync | `StorageFolder`/`StorageFile` async round-trip | ⬜ |
| 063 | DispatcherMarshalling | background work publishing to the UI thread safely | ⬜ |
| 064 | MvuxFeedBasics | `Uno.Extensions.Reactive` `Feed<T>`, data/error/progress axes | ⬜ |
| 065 | MvuxStateUpdates | `State<T>`, `Update`/`Set`, immutable records | ⬜ |
| 066 | MvuxListFeed | `ListFeed<T>`, empty vs loaded, pagination shape | ⬜ |
| 067 | MvuxCommands | commands from async methods, parameter feeds | ⬜ |
| 068 | HostingDependencyInjection | `IHost`, `IServiceCollection`, resolving a view model | ⬜ |
| 069 | ConditionalPlatformCode | one seam, per-platform implementations, a testable core | ⬜ |
| 070 | CapabilityProbe | feature detection instead of platform checks | ⬜ |

## Advanced (071–090) — layout engines, diagnostics, lifetime

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | NonVirtualizingLayout | implementing `NonVirtualizingLayout` for `ItemsRepeater` | ⬜ |
| 072 | VirtualizingLayoutWindow | `VirtualizingLayout`, realisation window, recycling | ⬜ |
| 073 | InvalidationTracking | `InvalidateMeasure`/`InvalidateArrange`, counting passes | ⬜ |
| 074 | LayoutCycles | measuring during arrange, and how to avoid the loop | ⬜ |
| 075 | DependencyPropertyCost | boxing, cached `DependencyProperty` lookups, struct payloads | ⬜ |
| 076 | ContainerPooling | pooling visuals, resetting state on reuse | ⬜ |
| 077 | BindingDiagnostics | `BindingExpression`, `UpdateSource`/`UpdateTarget`, failure surfaces | ⬜ |
| 078 | TemplatedControlInheritance | subclassing a templated control and extending its states | ⬜ |
| 079 | CompositeContentModel | building a control from parts with a content model | ⬜ |
| 080 | CustomAutomationPeer | `AutomationPeer`, patterns, programmatic invocation | ⬜ |
| 081 | TemplateSwapping | replacing a `Template` at runtime: re-lookup, re-applied states, old-part cleanup | ⬜ |
| 082 | StoryboardLogic | `Storyboard`, `DoubleAnimation`, `SkipToFill`, `Completed` | ⬜ |
| 083 | EasingFunctions | easing math and where the framework evaluates it | ⬜ |
| 084 | RenderTransforms | `RotateTransform`/`ScaleTransform`, `TransformToVisual` | ⬜ |
| 085 | ClippingGeometry | `Clip`, geometry parsing, bounds arithmetic | ⬜ |
| 086 | DesignTokenSystem | a token resource system with light/dark and `BasedOn` styles | ⬜ |
| 087 | ControlLibraryStyles | `Themes/Generic.xaml`, default style lookup from a library | ⬜ |
| 088 | RuntimeXamlLoading | `XamlReader.Load`, error handling, dynamic UI | ⬜ |
| 089 | WeakEventHandling | the weak event pattern, unsubscribing without leaking | ⬜ |
| 090 | LeakDiagnostics | proving a visual was released with `WeakReference` + GC | ⬜ |

## Expert (091–100) — architecture

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | PlatformSeam | a partial-class seam, per-platform implementation, tested core | ⬜ |
| 092 | MvuxPipeline | feed combinators, error and progress propagation | ⬜ |
| 093 | MvuxDynamicFeeds | feeds derived from feeds, cancellation, replay | ⬜ |
| 094 | NavigationRegions | `Uno.Extensions.Navigation` routes and navigators | ⬜ |
| 095 | HostConfiguration | `IHostBuilder`, options binding, environment overrides | ⬜ |
| 096 | BehaviorFramework | a small attached-behaviour framework with lifetime management | ⬜ |
| 097 | FlexLayoutEngine | a flex-style layout engine as a reusable `Layout` | ⬜ |
| 098 | DiagnosticsTracing | structured tracing of layout and binding work | ⬜ |
| 099 | CapstoneControl | one control: template, states, peer, tokens, tests | ⬜ |
| 100 | FeatureModules | composing a screen from independently registered modules | ⬜ |
