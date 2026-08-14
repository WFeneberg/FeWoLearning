# Kotlin — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100, and
the ⬜ rows are the work queue. Planned Kotlin exercises live in folders
`exercises/<tier>/exNNN_<slug>/` that contain the stub plus its sibling JUnit
test.

**Status: 60 ✅ / 40 ⬜**

## Beginner (001–035) — language fundamentals

`val`/`var`, strings, collections, null-safety, data classes, extensions,
scope functions, generics, and introductory JUnit assertions.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 001 | val_var_basics          | `val`, `var`, type inference                          | ✅     |
| 002 | string_templates        | string templates, multiline strings                   | ✅     |
| 003 | ranges_and_when         | ranges, `when`, exhaustiveness                        | ✅     |
| 004 | collection_filter_map   | list transformations, immutable collections           | ✅     |
| 005 | nullable_basics         | nullable types, smart casts                           | ✅     |
| 006 | elvis_operator          | `?:`, defaults, early returns                         | ✅     |
| 007 | safe_calls_chain        | `?.`, chaining, nullable navigation                   | ✅     |
| 008 | data_class_point        | data classes, copy, equality                          | ✅     |
| 009 | enum_when_branch        | enums, `when`, associated behavior                    | ✅     |
| 010 | sealed_result           | sealed classes, exhaustive branching                  | ✅     |
| 011 | extension_function      | extension functions, receivers                        | ✅     |
| 012 | default_named_args      | defaults, named arguments                             | ✅     |
| 013 | destructuring_pairs     | destructuring, pairs, data classes                    | ✅     |
| 014 | list_mutability         | `List` vs `MutableList`, copying                      | ✅     |
| 015 | map_count_words         | maps, `getOrDefault`, counting                        | ✅     |
| 016 | higher_order_basics     | passing functions, lambdas                            | ✅     |
| 017 | lambda_capture          | closures, captured state                              | ✅     |
| 018 | scope_let_run           | `let`, `run`, temporary scopes                        | ✅     |
| 019 | scope_apply_also        | `apply`, `also`, receiver vs argument                 | ✅     |
| 020 | string_builder_dsl      | `buildString`, DSL-like blocks                        | ✅     |
| 021 | object_singleton        | `object`, singleton lifetime                          | ✅     |
| 022 | companion_factory       | companion objects, factory methods                    | ✅     |
| 023 | interface_default_impl  | interfaces, default implementations                   | ✅     |
| 024 | inheritance_override    | open classes, overriding, `super`                     | ✅     |
| 025 | generic_box             | generic classes, type parameters                      | ✅     |
| 026 | reified_type_check      | inline reified generics                               | ✅     |
| 027 | sequence_lazy_basics    | sequences, laziness, terminal operations              | ✅     |
| 028 | regex_validation        | regular expressions, destructuring matches            | ✅     |
| 029 | result_run_catching     | `Result`, `runCatching`, recovery                     | ✅     |
| 030 | local_date_parsing      | Java time interop, parsing dates                      | ✅     |
| 031 | big_decimal_money       | `BigDecimal`, precision, rounding                     | ✅     |
| 032 | junit_assertions        | basic JUnit assertions, AAA                           | ✅     |
| 033 | property_custom_getter  | custom getters, derived properties                    | ✅     |
| 034 | delegated_lazy          | `lazy`, deferred initialization                       | ✅     |
| 035 | operator_overloading_pt | operator functions, domain ergonomics                 | ✅     |

## Intermediate (036–070) — idioms, APIs, testing

Nullability edge cases, richer collection operators, variance, delegated
properties, file APIs, coroutines, flows, and time-controlled tests.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 036 | counter_state           | encapsulated mutable state                            | ✅     |
| 037 | require_not_null        | `requireNotNull`, fail-fast validation                | ✅     |
| 038 | collection_group_by     | `groupBy`, transformation after grouping              | ✅     |
| 039 | collection_associate_by | `associateBy`, key collisions                         | ✅     |
| 040 | sequence_pipeline       | lazy multi-step transformations                       | ✅     |
| 041 | inline_higher_order     | `inline`, non-local returns                           | ✅     |
| 042 | tailrec_accumulator     | tail recursion, accumulator threading                 | ✅     |
| 043 | nullable_receiver_ext   | extensions on nullable receivers                      | ✅     |
| 044 | infix_functions         | infix notation, readability tradeoffs                 | ✅     |
| 045 | variance_out            | declaration-site covariance                           | ✅     |
| 046 | variance_in             | contravariance, consumers                             | ✅     |
| 047 | star_projection         | star projections, safe reads                          | ✅     |
| 048 | sealed_ui_state         | state modeling, exhaustive `when`                     | ✅     |
| 049 | data_object_singleton   | `data object`, singleton identity                     | ✅     |
| 050 | value_class_email       | value classes, validation                             | ✅     |
| 051 | delegated_observable    | `Delegates.observable`, change hooks                  | ✅     |
| 052 | delegated_map_backed    | map-backed properties, dynamic models                 | ✅     |
| 053 | comparator_then_by      | comparator composition                                | ✅     |
| 054 | pair_triple_transform   | `Pair`, `Triple`, decomposition                       | ✅     |
| 055 | result_recover          | recovering from failures                              | ✅     |
| 056 | exception_wrapping      | domain exceptions, causes                             | ✅     |
| 057 | file_use_lines          | `useLines`, resource safety                           | ✅     |
| 058 | path_copy_move          | NIO path operations                                   | ✅     |
| 059 | regex_named_groups      | named groups, match extraction                        | ✅     |
| 060 | junit_parameterized     | parameterized JUnit tests                             | ✅     |
| 061 | coroutine_launch_join   | structured coroutines, `launch`, `join`               | ⬜     |
| 062 | coroutine_async_await   | `async`, `await`, concurrency                         | ⬜     |
| 063 | coroutine_supervisor    | `SupervisorJob`, isolated failures                    | ⬜     |
| 064 | channel_pipeline        | channels, producer/consumer                           | ⬜     |
| 065 | flow_map_filter         | cold flows, transformations                           | ⬜     |
| 066 | state_flow_store        | `StateFlow`, UI-facing state                          | ⬜     |
| 067 | shared_flow_events      | `SharedFlow`, broadcast events                        | ⬜     |
| 068 | mutex_protected_state   | `Mutex`, suspending critical sections                 | ⬜     |
| 069 | test_dispatcher_time    | virtual time, coroutine test scheduler                | ⬜     |
| 070 | json_manual_parser      | parsing, validation, recoverable errors               | ⬜     |

## Advanced (071–090) — coroutines, DSLs, interop

Flow coordination, actors, builders, reflection, interoperability, and
design-level Kotlin patterns.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 071 | coroutine_retry_backoff | retry loops, delay policy                             | ⬜     |
| 072 | flow_combine_latest     | combining streams of state                            | ⬜     |
| 073 | flow_debounce_search    | debounce, cancellation, latest results                | ⬜     |
| 074 | flow_flatmap_latest     | `flatMapLatest`, replacing stale work                 | ⬜     |
| 075 | select_expression       | selecting the first available coroutine event         | ⬜     |
| 076 | actor_counter           | actor model, serialized state changes                 | ⬜     |
| 077 | sequence_vs_list_perf   | eager vs lazy pipelines                               | ⬜     |
| 078 | builder_type_safe_dsl   | receivers, fluent builders                            | ⬜     |
| 079 | receiver_lambdas_html   | nested receivers, mini DSL                            | ⬜     |
| 080 | delegate_validation     | custom delegates, centralized validation              | ⬜     |
| 081 | reflection_callable_ref | reflection, callable references                       | ⬜     |
| 082 | sealed_error_hierarchy  | rich domain errors, matching                          | ⬜     |
| 083 | dsl_marker_scope        | `@DslMarker`, receiver isolation                      | ⬜     |
| 084 | lazy_thread_safety      | lazy modes, concurrency tradeoffs                     | ⬜     |
| 085 | java_interop_optionals  | Java interop, `Optional`, platform types              | ⬜     |
| 086 | annotation_use_site     | use-site targets, annotations                         | ⬜     |
| 087 | object_expression_listener | anonymous objects, interfaces                      | ⬜     |
| 088 | map_get_or_put_cache    | caching, mutation, thread-safety caveats              | ⬜     |
| 089 | result_pipeline         | composing `Result`-like workflows                     | ⬜     |
| 090 | parser_combinator_mini  | higher-order parsers, composition                     | ⬜     |

## Expert (091–100) — architecture, tooling, domain DSLs

Infrastructure, worker coordination, interpreters, and type-safe internal DSLs.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 091 | coroutine_worker_pool   | bounded worker pools, graceful shutdown               | ⬜     |
| 092 | command_bus             | typed command dispatch                                | ⬜     |
| 093 | schema_form_renderer    | schema-driven rendering, validation                   | ⬜     |
| 094 | immutable_snapshot_store| persistent-style state snapshots                      | ⬜     |
| 095 | plugin_registry         | extension points, discovery                           | ⬜     |
| 096 | expression_evaluator    | tokenization, AST evaluation                          | ⬜     |
| 097 | retrying_http_client    | policy composition, suspend networking façade         | ⬜     |
| 098 | markdown_ast_renderer   | tree traversal, rendering                             | ⬜     |
| 099 | rules_engine            | predicates, priorities, evaluation                    | ⬜     |
| 100 | type_safe_sql_dsl       | builders, scope control, SQL rendering                | ⬜     |