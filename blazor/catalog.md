# Blazor — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned.

"Beginner" means **Blazor** beginner, not C# beginner: ex001 is a component with a
`[Parameter]`, not a `FizzBuzz`. Plain C# language drills belong to the `dotnet/` track.

Stubs live in `exercises/<tier>/ExNNN_<Slug>.razor`, their bUnit tests in
`tests/<tier>/ExNNN_<Slug>Tests.cs`, reference solutions in
`solutions/<tier>/ExNNN_<Slug>.razor`, and a manual demo page in
`host/Components/Demos/<Tier>/ExNNN.razor`. Tier namespaces are pinned by a
folder-level `_Imports.razor` (`@namespace FeWoLearning.Blazor.Exercises.Beginner`
and friends), because `01-beginner` is not a valid C# identifier.

**Status: 50 ✅ / 50 ⬜**

## Beginner (001–035) — component fundamentals

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | HelloComponent | `[Parameter]`, one-way binding of a computed member | ✅ |
| 002 | ParameterDefaults | parameter defaults, nullable/blank parameter handling | ✅ |
| 003 | ConditionalRendering | `@if`/`else if`/`else`, render precedence | ✅ |
| 004 | ListRendering | `@foreach`, list projection, empty list | ✅ |
| 005 | KeyedListDiffing | `@key`, component instance identity across reorder | ✅ |
| 006 | ClickEventCallback | `@onclick`, `EventCallback<T>`, stateless child | ✅ |
| 007 | CounterState | component-owned state, implicit re-render | ✅ |
| 008 | TwoWayBinding | `Value`/`ValueChanged` contract for `@bind-Value` | ✅ |
| 009 | BindFormat | hand-wired `value=` + `@onchange` (no `@bind` directive), date formatting/parsing round-trip, invariant culture | ✅ |
| 010 | BindEventOnInput | hand-wired `@oninput` vs `@onchange` timing (no `@bind` directive) | ✅ |
| 011 | ChildContent | `RenderFragment`, omitting an absent fragment | ✅ |
| 012 | NamedFragments | multiple named `RenderFragment` parameters | ✅ |
| 013 | TemplatedFragment | `@typeparam`, `RenderFragment<TItem>`, empty template | ✅ |
| 014 | AttributeSplatting | `CaptureUnmatchedValues`, `@attributes` | ✅ |
| 015 | DynamicCssClass | computed class strings, enum-driven styling | ✅ |
| 016 | InlineStyleBinding | computed inline style from a `double` percentage, clamping, invariant formatting | ✅ |
| 017 | OnInitialized | `OnInitialized` runs once, not per parameter change | ✅ |
| 018 | OnParametersSet | `OnParametersSet` runs on every parameter change | ✅ |
| 019 | OnAfterRenderFirst | `OnAfterRender(bool firstRender)`, render counting | ✅ |
| 020 | DisposableComponent | `@implements IDisposable`, subscribe/unsubscribe symmetry | ✅ |
| 021 | EventArgsHandling | `KeyboardEventArgs`, filtering modifier keys | ✅ |
| 022 | StopPropagation | `@onclick:stopPropagation`, nested handlers | ✅ |
| 023 | InputTextBinding | `@bind` to a local field | ✅ |
| 024 | NumericInputParsing | `@bind` to `int`, rejecting unparsable input | ✅ |
| 025 | SelectBinding | `@bind` on `<select>`, option projection | ✅ |
| 026 | CheckboxGroup | multi-selection state, stable result ordering | ✅ |
| 027 | RadioGroup | single-selection state, mutual exclusion | ✅ |
| 028 | CascadingValueBasics | `CascadingValue`/`[CascadingParameter]` | ✅ |
| 029 | NamedCascadingValue | `Name`-matched cascading values of the same type | ✅ |
| 030 | ComponentComposition | child registers itself with its parent | ✅ |
| 031 | ChildToParentCallback | `EventCallback` re-renders the parent automatically | ✅ |
| 032 | MarkupStringRendering | `MarkupString` vs escaped text | ✅ |
| 033 | EmptyStateFallback | three-state rendering, exact user-facing copy | ✅ |
| 034 | NestedParameterFlow | parameters do not flow implicitly through levels | ✅ |
| 035 | TabsComposition | capstone: cascaded parent, registration, active state | ✅ |

## Intermediate (036–070) — EditForm/validation, DI, JS interop, navigation, async lifecycle

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | EditFormBasics | `EditForm`, `EditContext`, `OnValidSubmit` vs `OnSubmit` | ✅ |
| 037 | DataAnnotationsValidation | `DataAnnotationsValidator`, `[Required]`/`[Range]`, `ValidationSummary` | ✅ |
| 038 | CustomValidationAttribute | custom `ValidationAttribute`, `IsValid` override | ✅ |
| 039 | ValidationMessageDisplay | `ValidationMessage<T>`, per-field error display | ✅ |
| 040 | EditContextFieldState | `EditContext.IsModified`, `MarkAsUnmodified`, field CSS class | ✅ |
| 041 | CustomFieldValidation | manual `EditContext` validation via a custom validator component | ✅ |
| 042 | NestedModelValidation | nested complex object validation, `EditContext` over sub-models | ✅ |
| 043 | ScopedStateContainer | DI scoped service, shared state across components, `StateHasChanged` | ✅ |
| 044 | SingletonVsScopedState | singleton vs scoped service lifetime in Blazor Server | ✅ |
| 045 | CascadingServiceInjection | `[Inject]` property injection vs constructor injection | ✅ |
| 046 | StateContainerNotification | event-based state container, subscribe/unsubscribe on `Dispose` | ✅ |
| 047 | OptionsPatternComponent | `IOptions<T>` injected into a component, configuration binding | ✅ |
| 048 | FactoryInjectedComponent | `IServiceProvider`/factory-resolved dependency | ✅ |
| 049 | JsInteropInvoke | `IJSRuntime.InvokeVoidAsync`, bUnit `JSInterop` mock setup | ✅ |
| 050 | JsInteropReturnValue | `IJSRuntime.InvokeAsync<T>`, mocked return value | ✅ |
| 051 | JsInteropElementReference | `ElementReference` passed to JS, mocked interop call | ⬜ |
| 052 | JsInteropModule | `IJSObjectReference`, JS module isolation, `DisposeAsync` | ⬜ |
| 053 | JsInteropUnmatchedInvocation | `JSInterop.Mode` Strict vs Loose, asserting an unexpected call fails | ⬜ |
| 054 | NavigationManagerBasics | `NavigationManager.NavigateTo`, bUnit `FakeNavigationManager` | ⬜ |
| 055 | NavigationLocationChanged | `NavigationManager.LocationChanged` subscribe/unsubscribe | ⬜ |
| 056 | QueryStringParsing | `NavigationManager.Uri`, `QueryHelpers.ParseQuery` | ⬜ |
| 057 | NavigationInterception | `RegisterLocationChangingHandler`, cancelling a navigation | ⬜ |
| 058 | BindDirectiveModifiers | `@bind:event` (custom update-trigger event), `@bind:format`, `@bind:after` (post-update hook) | ⬜ |
| 059 | PersistComponentStateBasics | `PersistentComponentState`, `RegisterOnPersisting`, restore on init | ⬜ |
| 060 | PersistComponentStateRoundtrip | persisting typed state as JSON, subscription disposal | ⬜ |
| 061 | RefCaptureBasics | `@ref` capturing an element reference and a component reference, calling a method on a child instance | ⬜ |
| 062 | AsyncOnInitialized | `async OnInitializedAsync`, loading-state rendering | ⬜ |
| 063 | CancellationOnDispose | `CancellationTokenSource` created in `OnInitialized`, cancelled in `Dispose` | ⬜ |
| 064 | SetParametersAsyncOverride | overriding `SetParametersAsync`, calling `base` | ⬜ |
| 065 | DebouncedAsyncSearch | `Task.Delay`-based debounce, cancelling a superseded request | ⬜ |
| 066 | ErrorBoundaryBasics | `ErrorBoundary`, `ErrorContent`, `Recover()` | ⬜ |
| 067 | ErrorBoundaryLoggingHandler | custom `ErrorBoundary` subclass, `OnErrorAsync` override | ⬜ |
| 068 | GenericComponentBasics | `@typeparam T`, generic component parameter inference | ⬜ |
| 069 | GenericConstraintComponent | generic type constraints (`where T : IComparable<T>`) on a component | ⬜ |
| 070 | GenericListComponent | generic component wrapping a list, reusable `RenderFragment<T>` template | ⬜ |

## Advanced (071–090) — render performance, virtualization, custom inputs, auth, render modes

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | ShouldRenderOptimization | `ShouldRender` override, skipping redundant re-renders | ⬜ |
| 072 | KeyDiffingDeepDive | `@key` under reorder/insert/delete, instance reuse vs recreation | ⬜ |
| 073 | VirtualizeItemsProvider | `Virtualize`, `ItemsProviderResult<T>`, paged loading | ⬜ |
| 074 | VirtualizeFixedItemSize | `Virtualize` with `ItemSize`, placeholder rendering, overscan | ⬜ |
| 075 | CustomInputBaseText | `InputBase<T>` derivative, `TryParseValueFromString` override | ⬜ |
| 076 | CustomInputBaseNumeric | `InputBase<T>` for a value type, `FormatValueAsString`, `CurrentValueAsString` | ⬜ |
| 077 | CustomValidatorComponent | custom validator component wired into `EditContext` | ⬜ |
| 078 | CrossFieldValidator | cross-field validation via `EditContext.OnValidationRequested` | ⬜ |
| 079 | DynamicComponentBasics | `DynamicComponent`, `Type` + `Parameters` dictionary | ⬜ |
| 080 | DynamicComponentParameterMapping | building and validating a `DynamicComponent` parameter dictionary | ⬜ |
| 081 | CascadingAuthStateBasics | `CascadingAuthenticationState`, `AuthenticationStateProvider`, `AuthorizeView` | ⬜ |
| 082 | CustomAuthenticationStateProvider | custom `AuthenticationStateProvider`, `NotifyAuthenticationStateChanged` | ⬜ |
| 083 | HandleEventCustomSync | `IHandleEvent` override, suppressing the automatic post-event re-render | ⬜ |
| 084 | HandleAfterRenderCustom | `IHandleAfterRender` override, custom post-render behaviour | ⬜ |
| 085 | RenderModeInteractiveServer | `RenderMode.InteractiveServer` semantics for a component | ⬜ |
| 086 | RenderModeStaticSsr | static SSR rendering, no interactivity, event handlers ignored | ⬜ |
| 087 | ComponentStatePreservationAcrossRenderMode | preserving state across a render-mode boundary | ⬜ |
| 088 | RenderFragmentCaching | caching a `RenderFragment` to avoid re-allocation, referential-equality pitfalls | ⬜ |
| 089 | SectionContentOutlet | `SectionContent`/`SectionOutlet`, content projection across layout boundaries | ⬜ |
| 090 | SupplyParameterFromQueryCapstone | `[SupplyParameterFromQuery]`, capstone combining several advanced-tier techniques | ⬜ |

## Expert (091–100) — render tree internals, custom infrastructure

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | RenderTreeBuilderManual | hand-written `BuildRenderTree` via `RenderTreeBuilder`, `OpenElement`/`CloseElement`/`AddAttribute` | ⬜ |
| 092 | RenderTreeBuilderConditional | `RenderTreeBuilder` branching, sequence numbers and diff stability | ⬜ |
| 093 | CustomComponentBaseLifecycle | custom `ComponentBase`-style base class reimplementing `StateHasChanged`/render scheduling | ⬜ |
| 094 | CustomComponentBaseRenderHook | custom base class hooking `SetParametersAsync`/`OnAfterRender` without `ComponentBase` | ⬜ |
| 095 | CustomRouterMatching | custom route-matching logic layered over `Router`, route constraints | ⬜ |
| 096 | CustomRouterFallback | custom not-found/fallback handling in a hand-rolled router component | ⬜ |
| 097 | RenderFragmentComposedInCode | composing `RenderFragment` values programmatically from delegates | ⬜ |
| 098 | RenderFragmentComposedTemplates | composing `RenderFragment<T>` templates in code, higher-order fragment functions | ⬜ |
| 099 | DiffAlgorithmKeyMismatch | reasoning about the render-tree diff algorithm, key mismatches forcing subtree replacement | ⬜ |
| 100 | StreamingSsrCapstone | streaming SSR semantics, capstone combining `RenderTreeBuilder` and diffing understanding | ⬜ |
