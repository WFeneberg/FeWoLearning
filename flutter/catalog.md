# Flutter/Dart — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100.
All 100 exercises live in folders `exercises/<tier>/exNNN_<slug>/` containing
the stub plus its sibling `package:test`/`flutter_test` test, with a matching
reference solution under `solutions/<tier>/exNNN_<slug>/`.

**Status: 100 ✅ / 0 ⬜** — content-complete but **unverified** (no
Flutter/Dart SDK on this machine has ever compiled or run any of it; see
`README.md`).

## Beginner (001–035) — Dart language fundamentals

Pure Dart: variables, null safety, collections, classes, mixins, generics,
and `package:test` basics. No Flutter widgets yet — these run with plain
`dart test`.

| #   | Slug                        | Concepts                                              | Status |
|-----|-----------------------------|--------------------------------------------------------|--------|
| 001 | var_final_const             | `var`, `final`, `const`, type inference                | ✅     |
| 002 | string_interpolation        | string templates, multiline strings                    | ✅     |
| 003 | null_safety_basics          | nullable types, `??`, `??=`                             | ✅     |
| 004 | collection_if_for           | collection-for, collection-if, list literals            | ✅     |
| 005 | switch_pattern              | records, switch expressions, pattern guards             | ✅     |
| 006 | list_map_basics             | `List`/`Map` literals, indexing                         | ✅     |
| 007 | spread_operator             | spread (`...`), null-aware spread (`...?`)              | ✅     |
| 008 | named_positional_params     | named vs positional parameters, `required`              | ✅     |
| 009 | default_param_values        | default parameter values                                | ✅     |
| 010 | arrow_functions              | arrow syntax (`=>`), expression bodies                  | ✅     |
| 011 | class_basics                 | classes, constructors, fields                           | ✅     |
| 012 | named_constructors           | named constructors, initializer lists                   | ✅     |
| 013 | getters_setters              | custom getters/setters                                  | ✅     |
| 014 | enum_basic                   | enums, enum values                                      | ✅     |
| 015 | enhanced_enum                | enhanced enums with fields/methods                      | ✅     |
| 016 | mixin_basics                  | mixins, `with`                                          | ✅     |
| 017 | abstract_class                | abstract classes, abstract methods                      | ✅     |
| 018 | interface_implements          | implicit interfaces, `implements`                       | ✅     |
| 019 | extension_methods             | extension methods on existing types                     | ✅     |
| 020 | cascade_notation              | cascade operator (`..`), chained calls                  | ✅     |
| 021 | exception_try_catch           | `try`/`catch`/`finally`, custom exceptions               | ✅     |
| 022 | exception_rethrow             | `rethrow`, exception hierarchies                        | ✅     |
| 023 | late_variables                | `late`, lazy initialization                             | ✅     |
| 024 | iterable_where_map            | `Iterable.where`/`map`/`reduce`                          | ✅     |
| 025 | fold_and_reduce               | `fold` vs `reduce`, accumulator patterns                | ✅     |
| 026 | sort_comparator               | `List.sort`, `Comparator`, `compareTo`                   | ✅     |
| 027 | set_operations                | `Set`, union/intersection/difference                     | ✅     |
| 028 | string_manipulation           | split/join/trim/padLeft                                 | ✅     |
| 029 | regexp_basics                 | `RegExp`, matching, `replaceAll`                         | ✅     |
| 030 | typedef_function_type         | `typedef`, function types as values                     | ✅     |
| 031 | generics_basics               | generic functions/classes, type parameters              | ✅     |
| 032 | operator_overloading           | operator overloading (`==`, `+`)                        | ✅     |
| 033 | equality_hashcode              | `==` and `hashCode` contract                            | ✅     |
| 034 | factory_constructor            | factory constructors, caching instances                 | ✅     |
| 035 | test_package_assertions        | `package:test` basics, `group`/`setUp`                  | ✅     |

## Intermediate (036–070) — async, streams, first widgets

Futures, streams, isolates, JSON, sealed classes/patterns, then the first
`StatelessWidget`/`StatefulWidget` exercises using `flutter_test`.

| #   | Slug                          | Concepts                                              | Status |
|-----|--------------------------------|--------------------------------------------------------|--------|
| 036 | future_basics                  | `Future`, `then`/`catchError`                           | ✅     |
| 037 | async_await_basics             | `async`/`await`, sequential futures                     | ✅     |
| 038 | future_wait                    | `Future.wait`, parallel futures                         | ✅     |
| 039 | stream_basics                  | `Stream`, `listen`, `async*`                            | ✅     |
| 040 | stream_transform               | `Stream.map`/`where`/`transform`                        | ✅     |
| 041 | stream_controller               | `StreamController`, broadcast streams                   | ✅     |
| 042 | isolate_compute                | `Isolate.run` / compute-style offloading                | ✅     |
| 043 | json_encode_decode              | `dart:convert`, `jsonEncode`/`jsonDecode`               | ✅     |
| 044 | json_model_fromjson             | manual `fromJson`/`toJson` model mapping                | ✅     |
| 045 | generic_repository               | generic repository/service pattern                      | ✅     |
| 046 | extension_type                   | Dart 3 extension types                                  | ✅     |
| 047 | sealed_classes                   | sealed classes, exhaustive `switch`                     | ✅     |
| 048 | pattern_destructuring             | destructuring patterns, records                         | ✅     |
| 049 | async_generators                  | `async*`, `yield`, Stream generators                     | ✅     |
| 050 | completer_basics                  | `Completer`, bridging callback APIs                      | ✅     |
| 051 | timer_basics                       | `Timer`, `Timer.periodic`, cancellation                  | ✅     |
| 052 | zone_error_handling                | `runZonedGuarded`, uncaught async errors                 | ✅     |
| 053 | stateless_widget_basics            | `StatelessWidget`, `build()`, widget tree                | ✅     |
| 054 | widget_testing_basics              | `flutter_test`, `WidgetTester.pumpWidget`                | ✅     |
| 055 | stateful_widget_basics             | `StatefulWidget`, `State`, `setState`                    | ✅     |
| 056 | widget_finder_basics               | `find.text`/`find.byType`, `tester.tap`                  | ✅     |
| 057 | inherited_widget_basics            | `InheritedWidget`, `dependOnInheritedWidgetOfExactType`  | ✅     |
| 058 | value_notifier_basics              | `ValueNotifier`, `ValueListenableBuilder`                | ✅     |
| 059 | change_notifier_basics             | `ChangeNotifier`, `notifyListeners`                      | ✅     |
| 060 | future_builder_widget              | `FutureBuilder`, async UI states                         | ✅     |
| 061 | stream_builder_widget              | `StreamBuilder`, async UI states                         | ✅     |
| 062 | form_validation_widget             | `Form`, `TextFormField`, validators                      | ✅     |
| 063 | navigator_basic_routes             | `Navigator.push`/`pop`, named routes                     | ✅     |
| 064 | key_widget_identity                | widget keys, `ValueKey`/`UniqueKey` identity             | ✅     |
| 065 | theme_data_basics                  | `ThemeData`, `Theme.of(context)`                         | ✅     |
| 066 | gesture_detector_basics            | `GestureDetector`, tap/drag callbacks                    | ✅     |
| 067 | animation_controller_basics        | `AnimationController`, `Tween`                           | ✅     |
| 068 | layout_constraints_basics          | `Row`/`Column`/`Expanded`, constraint flow               | ✅     |
| 069 | golden_test_basics                 | widget snapshot testing (`matchesGoldenFile`-style)      | ✅     |
| 070 | mockito_service_mock               | mocking a service with `mockito` in a widget test        | ✅     |

## Advanced (071–090) — state management, custom rendering, testing

Provider/Riverpod/Bloc, stream composition, `CustomPainter`/`RenderObject`,
platform interop, and integration/golden testing.

| #   | Slug                             | Concepts                                              | Status |
|-----|------------------------------------|--------------------------------------------------------|--------|
| 071 | provider_basics                    | `package:provider`, `ChangeNotifierProvider`            | ✅     |
| 072 | riverpod_basics                    | `flutter_riverpod` providers, `ref.watch`               | ✅     |
| 073 | bloc_basics                        | `flutter_bloc`, Bloc/Cubit, events & states              | ✅     |
| 074 | stream_combine_latest              | combining multiple streams (rxdart-style)                | ✅     |
| 075 | stream_debounce_search             | debounced search-as-you-type stream pipeline             | ✅     |
| 076 | custom_painter_basics              | `CustomPainter`, `Canvas` drawing                        | ✅     |
| 077 | render_object_basics               | `RenderObject`/`RenderBox` custom layout                 | ✅     |
| 078 | implicit_animation_widgets         | `AnimatedContainer`/`AnimatedOpacity`                    | ✅     |
| 079 | hero_animation_basics              | `Hero` widget, shared element transitions                | ✅     |
| 080 | custom_scroll_view_slivers         | `CustomScrollView`, Sliver widgets                       | ✅     |
| 081 | dependency_injection_basics        | `get_it` / service locator pattern                       | ✅     |
| 082 | repository_pattern_flutter         | repository pattern, data/domain separation               | ✅     |
| 083 | error_boundary_widget              | `ErrorWidget.builder`, graceful error UI                 | ✅     |
| 084 | accessibility_semantics            | `Semantics` widget, screen-reader labels                 | ✅     |
| 085 | platform_channel_basics            | `MethodChannel`, platform interop                        | ✅     |
| 086 | shared_preferences_basics          | persisting simple key/value state                        | ✅     |
| 087 | golden_toolkit_multi_device        | multi-device golden test matrices                        | ✅     |
| 088 | integration_test_basics            | `integration_test` package, end-to-end flow              | ✅     |
| 089 | riverpod_async_notifier            | `AsyncNotifier`, async state with retry                  | ✅     |
| 090 | bloc_to_bloc_communication         | coordinating multiple Blocs/Cubits                       | ✅     |

## Expert (091–100) — architecture, tooling, performance

Hand-rolled state containers, plugin architecture, bidirectional platform
channels, isolate worker pools, and app-shell routing.

| #   | Slug                              | Concepts                                              | Status |
|-----|--------------------------------------|--------------------------------------------------------|--------|
| 091 | custom_state_management_arch         | hand-rolled Redux-like state container                  | ✅     |
| 092 | command_query_widget_arch            | CQRS-ish separation in a Flutter feature module         | ✅     |
| 093 | plugin_architecture_basics           | federated plugin package structure                      | ✅     |
| 094 | platform_channel_bidirectional       | two-way `MethodChannel` + `EventChannel`                | ✅     |
| 095 | isolate_worker_pool                  | `Isolate`-based worker pool for CPU-bound work           | ✅     |
| 096 | performance_profiling_widgets        | `RepaintBoundary`, avoiding rebuild storms               | ✅     |
| 097 | dependency_graph_di_container        | custom DI container with scoped lifetimes               | ✅     |
| 098 | offline_first_sync_engine            | local cache + sync queue architecture                    | ✅     |
| 099 | feature_flag_rollout_widget          | remote-config-driven feature flagging                    | ✅     |
| 100 | modular_app_shell_router             | multi-module app shell with a declarative router         | ✅     |
