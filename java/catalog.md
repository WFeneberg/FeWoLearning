# Java — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100, and
the ⬜ rows are the work queue. Planned Java exercises live in folders
`exercises/<tier>/exNNN_<slug>/` that contain the stub plus its sibling JUnit
test.

**Status: 86 ✅ / 14 ⬜**

## Beginner (001–035) — syntax, types, control flow

Primitives, strings, arrays, collections, methods, enums, records, dates,
exceptions, and introductory JUnit assertions.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 001 | primitive_math          | primitives, arithmetic, integer division              | ✅     |
| 002 | boolean_logic           | booleans, comparisons, short-circuiting               | ✅     |
| 003 | string_formatting       | string methods, formatting, interpolation alternatives| ✅     |
| 004 | array_statistics        | arrays, loops, min/max/average                        | ✅     |
| 005 | list_operations         | `List`, add/remove, iteration                         | ✅     |
| 006 | hashmap_word_count      | `Map`, counting, `merge`                              | ✅     |
| 007 | sum_of_digits           | loops, modulo, integer decomposition                  | ✅     |
| 008 | collatz_steps           | looping, mutation, termination conditions             | ✅     |
| 009 | grade_classifier        | branching, ranges, validation                         | ✅     |
| 010 | traffic_light_enum      | enums, `switch`, behavior by variant                  | ✅     |
| 011 | point_record            | records, value semantics, accessors                   | ✅     |
| 012 | optional_basics         | `Optional`, presence checks, defaults                 | ✅     |
| 013 | switch_expression       | switch expressions, exhaustiveness                    | ✅     |
| 014 | text_block_report       | text blocks, multiline formatting                     | ✅     |
| 015 | local_date_basics       | `LocalDate`, parsing, date arithmetic                 | ✅     |
| 016 | big_decimal_money       | `BigDecimal`, scale, rounding                         | ✅     |
| 017 | loop_continue_break     | `for`, `while`, `continue`, `break`                   | ✅     |
| 018 | method_overloading      | overload resolution, signatures                       | ✅     |
| 019 | varargs_join            | varargs, arrays from calls                            | ✅     |
| 020 | immutable_list_copy     | defensive copying, `List.copyOf`                      | ✅     |
| 021 | comparator_sort         | comparators, sorting, reversed order                  | ✅     |
| 022 | predicate_filter        | predicates, filtering, reusable conditions            | ✅     |
| 023 | stream_map_sum          | streams, `mapToInt`, terminal operations              | ✅     |
| 024 | stream_grouping         | `Collectors.groupingBy`, aggregation                  | ✅     |
| 025 | file_read_lines         | `Files.readAllLines`, UTF-8                           | ✅     |
| 026 | path_operations         | `Path`, normalize, resolve                            | ✅     |
| 027 | try_with_resources      | automatic closing, resource safety                    | ✅     |
| 028 | custom_exception        | extending exceptions, meaningful messages             | ✅     |
| 029 | regex_validation        | regex, matcher groups, validation                     | ✅     |
| 030 | string_builder          | mutable text assembly, efficiency                     | ✅     |
| 031 | class_invariant         | constructors, validation, encapsulation               | ✅     |
| 032 | inheritance_override    | inheritance, overriding, `super`                      | ✅     |
| 033 | interface_default_method| interfaces, default methods                           | ✅     |
| 034 | record_validation       | compact constructors, invariants                      | ✅     |
| 035 | junit_assertions        | basic JUnit assertions, Arrange-Act-Assert            | ✅     |

## Intermediate (036–070) — collections, generics, streams, I/O

Generics, collection APIs, streams, modern Java language features, concurrency
basics, and richer test coverage.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 036 | counter_service         | stateful class, methods, invariants                   | ✅     |
| 037 | generic_box             | generic classes, type parameters                      | ✅     |
| 038 | generic_pair            | multiple type parameters, tuple-like modeling         | ✅     |
| 039 | bounded_generic_max     | bounds, `Comparable`, reusable algorithms             | ✅     |
| 040 | wildcard_read_write     | PECS, wildcards, variance intuition                   | ✅     |
| 041 | comparable_person       | `Comparable`, consistent ordering                     | ✅     |
| 042 | comparator_then_compare | comparator chaining, derived sort keys                | ✅     |
| 043 | map_compute_if_absent   | lazy insertion, nested collections                    | ✅     |
| 044 | deque_stack_queue       | `Deque`, stack and queue semantics                    | ✅     |
| 045 | priority_queue_scheduler| priority queues, natural ordering                     | ✅     |
| 046 | stream_flatmap          | flattening nested collections                         | ✅     |
| 047 | collector_partitioning  | `partitioningBy`, summarizing                         | ✅     |
| 048 | collector_teeing        | combining collectors, dual aggregation                | ✅     |
| 049 | optional_pipeline       | `map`, `flatMap`, `orElseGet`                         | ✅     |
| 050 | exception_translation   | wrapping low-level failures                           | ✅     |
| 051 | checked_vs_unchecked    | exception design, API tradeoffs                       | ✅     |
| 052 | enum_strategy           | behavior per enum constant                            | ✅     |
| 053 | sealed_shape_area       | sealed hierarchies, exhaustive branching              | ✅     |
| 054 | pattern_instanceof      | pattern matching for `instanceof`                     | ✅     |
| 055 | pattern_switch          | pattern matching `switch`                             | ✅     |
| 056 | http_client_get         | `HttpClient`, request/response basics                 | ✅     |
| 057 | completable_future_chain| async composition, continuations                      | ✅     |
| 058 | completable_future_allof| waiting for many tasks                                | ✅     |
| 059 | executor_service_batch  | task submission, shutdown                             | ✅     |
| 060 | synchronized_counter    | synchronized methods, race conditions                 | ✅     |
| 061 | reentrant_lock_guard    | `ReentrantLock`, `try/finally`                        | ✅     |
| 062 | atomic_integer_counter  | atomics, lock-free increments                         | ✅     |
| 063 | nio_walk_files          | `Files.walk`, filtering paths                         | ✅     |
| 064 | properties_config       | `Properties`, config loading                          | ✅     |
| 065 | locale_number_format    | locale-aware formatting                               | ✅     |
| 066 | time_zone_conversion    | `ZonedDateTime`, zone conversions                     | ✅     |
| 067 | javadoc_contracts       | API documentation, edge-case contracts                | ✅     |
| 068 | junit_parameterized     | parameterized tests, input tables                     | ✅     |
| 069 | junit_exception_testing | failure assertions, messages                          | ✅     |
| 070 | test_double_handrolled  | hand-rolled test doubles, interaction checks          | ✅     |

## Advanced (071–090) — concurrency, JVM features, architecture

Parallelism, async coordination, reflection, annotations, service loading,
records, and design-level exercises.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 071 | lru_cache               | maps, eviction policy, recency tracking               | ✅     |
| 072 | custom_collector        | `Collector` contract, mutable reduction               | ✅     |
| 073 | spliterator_batching    | custom traversal, characteristics                     | ✅     |
| 074 | parallel_stream_pitfalls| parallel streams, statefulness, ordering             | ✅     |
| 075 | fork_join_sum           | fork/join tasks, work splitting                       | ✅     |
| 076 | producer_consumer_queue | blocking queues, coordination                         | ✅     |
| 077 | rate_limiter            | time windows, thread safety                           | ✅     |
| 078 | bounded_executor        | semaphore back-pressure, pools                        | ✅     |
| 079 | virtual_threads_basics  | virtual threads, blocking style concurrency           | ✅     |
| 080 | structured_task_scope   | structured concurrency, cancellation                  | ✅     |
| 081 | record_builder          | records, builder ergonomics                           | ✅     |
| 082 | annotation_retention    | annotation targets, runtime visibility                | ✅     |
| 083 | reflection_method_invoke| reflection, invocation, accessibility                 | ✅     |
| 084 | service_loader_plugin   | `ServiceLoader`, pluggable implementations            | ✅     |
| 085 | sealed_result_type      | typed success/failure outcomes                        | ✅     |
| 086 | money_value_object      | equality, precision, domain modeling                  | ✅     |
| 087 | csv_parser              | parsing, validation, recoverable errors               | ⬜     |
| 088 | retry_backoff           | retries, jitter/backoff policy                        | ⬜     |
| 089 | cache_with_expiry       | clocks, stale data, synchronization                   | ⬜     |
| 090 | hexagonal_port_adapter  | ports/adapters, dependency inversion                  | ⬜     |

## Expert (091–100) — frameworks, tooling, DSLs

Parser design, DI, annotation processing, lightweight infrastructure, and
cross-cutting patterns.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 091 | mini_di_container       | reflection, constructor wiring, scopes                | ⬜     |
| 092 | expression_parser       | tokenization, precedence, AST evaluation              | ⬜     |
| 093 | rule_engine             | predicates, composition, execution order              | ⬜     |
| 094 | event_bus               | publish/subscribe, synchronous dispatch               | ⬜     |
| 095 | annotation_processor    | code generation, compile-time validation              | ⬜     |
| 096 | jdbc_row_mapper         | JDBC basics, mapping rows to objects                  | ⬜     |
| 097 | batch_file_pipeline     | chunked processing, fault reporting                   | ⬜     |
| 098 | markdown_table_renderer | text rendering, alignment rules                       | ⬜     |
| 099 | tiny_template_engine    | placeholders, escaping, rendering contexts            | ⬜     |
| 100 | command_dispatcher      | command routing, extensibility                        | ⬜     |