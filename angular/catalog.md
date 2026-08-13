# Angular — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100, and
the ⬜ rows are the work queue. Each exercise is a folder
`exercises/<tier>/exNNN_<slug>/` holding the stub plus its `*.spec.ts`.
Components are **standalone** and use **signals**; tests run headless through
Jest (`jest-preset-angular`), not Karma.

**Status: 100 ✅ / 0 ⬜**

## Beginner (001–035) — components & DI

Standalone components, interpolation & binding, `@Input`/`@Output`, `@if`/`@for`
control flow, event binding, services & `providedIn`, dependency injection,
signals (`signal`/`computed`), template refs, built-in pipes, lifecycle hooks.

| #   | Slug                      | Concepts                                              | Status |
|-----|---------------------------|-------------------------------------------------------|--------|
| 001 | pricing_service           | DI, service, validation                               | ✅     |
| 002 | interpolation_binding     | `{{ }}`, property binding, `[attr.]`                  | ✅     |
| 003 | signal_basics             | `signal()`, `set`, `update`, reading in templates      | ✅     |
| 004 | computed_signal           | `computed()`, derived state, laziness                 | ✅     |
| 005 | event_binding_click       | `(click)`, handler methods, `$event`                  | ✅     |
| 006 | input_property            | `@Input()`, required inputs, defaults                 | ✅     |
| 007 | input_signal              | `input()` signal inputs, transforms                   | ✅     |
| 008 | output_emitter            | `@Output()`, `EventEmitter`, parent wiring            | ✅     |
| 009 | output_function           | `output()`, typed emit                                | ✅     |
| 010 | control_flow_if           | `@if` / `@else`, template conditions                  | ✅     |
| 011 | control_flow_for          | `@for`, `track`, `$index`, `@empty`                   | ✅     |
| 012 | control_flow_switch       | `@switch` / `@case` / `@default`                      | ✅     |
| 013 | class_binding             | `[class.x]`, `[ngClass]`                              | ✅     |
| 014 | style_binding             | `[style.x]`, units, `[ngStyle]`                       | ✅     |
| 015 | two_way_binding           | `[(ngModel)]`, `FormsModule` import                   | ✅     |
| 016 | model_signal              | `model()` two-way signal binding                      | ✅     |
| 017 | service_provided_in_root  | `@Injectable({providedIn:'root'})`, singleton scope    | ✅     |
| 018 | inject_function           | the `inject()` function vs constructor injection      | ✅     |
| 019 | injection_token_config    | `InjectionToken`, providing configuration values      | ✅     |
| 020 | component_provider_scope  | component-level `providers`, per-instance services    | ✅     |
| 021 | lifecycle_oninit          | `ngOnInit`, initialization order                      | ✅     |
| 022 | lifecycle_ondestroy       | `ngOnDestroy`, cleanup, `DestroyRef`                  | ✅     |
| 023 | template_ref_var          | `#ref` template variables, reading DOM nodes          | ✅     |
| 024 | view_child_signal         | `viewChild()`, querying a child component             | ✅     |
| 025 | pipe_date_currency        | built-in `DatePipe`, `CurrencyPipe`, locales          | ✅     |
| 026 | pipe_json_slice           | `JsonPipe`, `SlicePipe`, `KeyValuePipe`               | ✅     |
| 027 | host_binding              | `host` metadata, `@HostBinding`, `@HostListener`       | ✅     |
| 028 | ng_template_outlet        | `ng-template`, `NgTemplateOutlet`, context            | ✅     |
| 029 | attribute_directive       | a simple attribute directive with `ElementRef`        | ✅     |
| 030 | signal_array_update       | immutable array updates in signals                    | ✅     |
| 031 | signal_object_update      | `update()` with object spread, change detection        | ✅     |
| 032 | component_composition     | nesting components, passing data down                 | ✅     |
| 033 | untracked_read            | `untracked()`, avoiding unwanted dependencies         | ✅     |
| 034 | signal_equality_fn        | custom `equal` on a signal, skipping notifications    | ✅     |
| 035 | testbed_basics            | `TestBed.configureTestingModule`, `ComponentFixture`   | ✅     |

## Intermediate (036–070) — reactive patterns

Reactive forms, `HttpClient` + `HttpTestingController`, RxJS operators,
`AsyncPipe`, custom pipes, structural directives, content projection,
`inject()`, route params, `effect()`, `linkedSignal`.

| #   | Slug                        | Concepts                                            | Status |
|-----|-----------------------------|-----------------------------------------------------|--------|
| 036 | counter                     | standalone component, signals                       | ✅     |
| 037 | form_control_basics         | `FormControl`, value, `setValue`, `valueChanges`     | ✅     |
| 038 | form_group_nested          | `FormGroup`, nested groups, `patchValue`             | ✅     |
| 039 | form_builder               | `FormBuilder`, concise group construction           | ✅     |
| 040 | form_sync_validators       | `Validators.required`/`minLength`, error surfacing   | ✅     |
| 041 | form_custom_validator      | a `ValidatorFn`, cross-field validation             | ✅     |
| 042 | form_async_validator       | `AsyncValidatorFn`, pending state                   | ✅     |
| 043 | form_array                 | `FormArray`, adding and removing controls           | ✅     |
| 044 | form_status_flags          | `touched`/`dirty`/`pristine`, submit gating         | ✅     |
| 045 | http_get_typed             | `HttpClient.get<T>`, `provideHttpClient`            | ✅     |
| 046 | http_testing_controller    | `HttpTestingController`, `expectOne`, `flush`        | ✅     |
| 047 | http_error_handling        | `HttpErrorResponse`, `catchError`, retries          | ✅     |
| 048 | http_params_headers        | `HttpParams`, `HttpHeaders`, query building         | ✅     |
| 049 | rxjs_map_filter            | `map`, `filter`, the pipe operator                  | ✅     |
| 050 | rxjs_switch_map            | `switchMap`, cancelling stale inner streams         | ✅     |
| 051 | rxjs_debounce_distinct     | `debounceTime`, `distinctUntilChanged`              | ✅     |
| 052 | rxjs_combine_latest        | `combineLatest`, `withLatestFrom`                   | ✅     |
| 053 | rxjs_subject_state         | `BehaviorSubject`, imperative state pushes          | ✅     |
| 054 | rxjs_to_signal             | `toSignal`, `toObservable`, interop                 | ✅     |
| 055 | async_pipe_template        | `AsyncPipe`, subscription lifecycle in templates     | ✅     |
| 056 | custom_pipe_pure           | a pure `PipeTransform`, memoization                 | ✅     |
| 057 | custom_pipe_impure         | `pure: false`, when it is justified                 | ✅     |
| 058 | structural_directive       | a structural directive with `TemplateRef`           | ✅     |
| 059 | content_projection_single  | `<ng-content>`, default content                     | ✅     |
| 060 | content_projection_select  | `select=` multi-slot projection                     | ✅     |
| 061 | content_child_query        | `contentChild()`, querying projected content        | ✅     |
| 062 | route_params_snapshot      | `ActivatedRoute.snapshot`, reading params           | ✅     |
| 063 | route_params_observable    | `paramMap` as a stream, reacting to changes         | ✅     |
| 064 | router_navigation          | `Router.navigate`, `queryParams`, `RouterLink`      | ✅     |
| 065 | effect_side_effects        | `effect()`, cleanup functions, allowed writes       | ✅     |
| 066 | linked_signal              | `linkedSignal()`, resettable derived state          | ✅     |
| 067 | signal_service_store       | a signal-based service as shared state              | ✅     |
| 068 | injector_hierarchies       | element vs environment injectors, resolution order  | ✅     |
| 069 | provide_use_factory        | `useFactory`, `useExisting`, `useValue`             | ✅     |
| 070 | testing_signal_component   | asserting signal state through a fixture           | ✅     |

## Advanced (071–090) — architecture & performance

| #   | Slug                        | Concepts                                            | Status |
|-----|-----------------------------|-----------------------------------------------------|--------|
| 071 | route_guard_can_activate    | `CanActivateFn`, redirecting unauthenticated users  | ✅     |
| 072 | route_guard_can_deactivate  | `CanDeactivateFn`, unsaved-changes prompts         | ✅     |
| 073 | route_resolver              | `ResolveFn`, pre-loading route data                 | ✅     |
| 074 | lazy_loaded_routes          | `loadComponent`, `loadChildren`, code splitting     | ✅     |
| 075 | onpush_change_detection     | `OnPush`, immutability requirements                 | ✅     |
| 076 | change_detector_ref         | `markForCheck`, `detach`, manual detection          | ✅     |
| 077 | http_interceptor_auth       | functional interceptors, attaching headers          | ✅     |
| 078 | http_interceptor_retry      | interceptor-level retry and backoff                 | ✅     |
| 079 | control_value_accessor      | `ControlValueAccessor`, a custom form control       | ✅     |
| 080 | dynamic_component_loading   | `ViewContainerRef.createComponent`, inputs          | ✅     |
| 081 | defer_block                 | `@defer`, triggers, placeholder and loading blocks   | ✅     |
| 082 | directive_composition       | `hostDirectives`, composing behaviour               | ✅     |
| 083 | fake_async_tick             | `fakeAsync`, `tick`, `flushMicrotasks`              | ✅     |
| 084 | testing_router_harness      | `RouterTestingHarness`, navigating in tests        | ✅     |
| 085 | signal_store_pattern        | a typed signal store with actions and selectors     | ✅     |
| 086 | resource_async_signal       | `resource()`, async loading as signal state        | ✅     |
| 087 | virtual_scroll_list         | windowed rendering over a large list                | ✅     |
| 088 | error_handler_global        | a custom `ErrorHandler`, reporting                  | ✅     |
| 089 | zoneless_change_detection   | `provideExperimentalZonelessChangeDetection`        | ✅     |
| 090 | performance_track_by        | `@for` `track` correctness and DOM reuse            | ✅     |

## Expert (091–100) — systems & design

| #   | Slug                       | Concepts                                             | Status |
|-----|----------------------------|------------------------------------------------------|--------|
| 091 | feature_sliced_architecture | feature boundaries, barrel-free imports, layering    | ✅     |
| 092 | typed_data_access_layer    | typed repositories, DTO mapping, error envelopes     | ✅     |
| 093 | signal_store_library       | a reusable store factory with typed actions          | ✅     |
| 094 | ssr_hydration_component    | SSR-safe rendering, hydration-stable state           | ✅     |
| 095 | schema_driven_form_renderer | schema to reactive form, dynamic controls           | ✅     |
| 096 | design_system_a11y_widget  | ARIA roles and states, keyboard navigation, focus    | ✅     |
| 097 | plugin_injection_tokens    | multi-provider extension points, `multi: true`       | ✅     |
| 098 | websocket_live_service     | live data service, reconnect, signal projection      | ✅     |
| 099 | micro_frontend_shell       | multiple bootstrapped apps, isolation, teardown      | ✅     |
| 100 | i18n_layer                 | message extraction, runtime locale switching         | ✅     |
