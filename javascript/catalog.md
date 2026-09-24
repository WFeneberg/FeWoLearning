# JavaScript — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + tests + solution present, red and green verified) ·
⬜ planned.

This table is the track's progress ledger and its work queue: the next five ⬜
rows are the next batch. Each exercise is
`exercises/<tier>/exNNN_<slug>/index.js` with its reference at
`solutions/<tier>/exNNN_<slug>/index.js` — plus any sibling modules a row
needs. The tests live **once**, under `tests/<tier>/exNNN_<slug>.test.js`, and
import through the `@ex` alias that `vitest.shared.js` points at either content
tree: `npm test` is the red run, `npm run test:solutions` the green one.

**Status: 70 ✅ / 30 ⬜**

This track is **the language itself** — the runtime object model, coercion,
closures, prototypes, the job queue, iterators, proxies. Types belong to
`typescript/`, and framework work to `vue/` and `angular/`; nothing here
imports a UI library. Everything runs on plain Node (ESM).

## 01-beginner (001–035)

| #   | Slug | Concepts | State |
|-----|------|----------|-------|
| 001 | value_types | primitives vs objects, `typeof`, why `typeof null` is `"object"` | ✅ |
| 002 | equality_and_coercion | `==` vs `===`, `Object.is`, `NaN` | ✅ |
| 003 | truthiness | the eight falsy values, `??` vs `||`, `??=` | ✅ |
| 004 | number_precision | binary floating point, `Number.EPSILON`, `toFixed`, integer checks | ✅ |
| 005 | string_methods | `trim`/`padStart`/`replaceAll`/`at`, immutability of strings | ✅ |
| 006 | template_literals | interpolation, multiline, expression nesting | ✅ |
| 007 | array_mutation | `push`/`splice`/`slice`, mutating vs copying, `toSpliced` | ✅ |
| 008 | array_pipeline | `map`/`filter`/`reduce` | ✅ |
| 009 | array_search | `find`/`findLast`/`some`/`every`/`includes` vs `indexOf` | ✅ |
| 010 | array_sort | comparator contract, sort stability, `toSorted` | ✅ |
| 011 | object_literals | shorthand, computed keys, key ordering rules | ✅ |
| 012 | destructuring | object/array patterns, defaults, renaming, swapping | ✅ |
| 013 | rest_and_spread | rest parameters, spread in calls and literals, shallow copy | ✅ |
| 014 | default_parameters | evaluated per call, earlier parameters in scope | ✅ |
| 015 | arrow_functions | concise bodies, no own `this`/`arguments`, returning an object | ✅ |
| 016 | closures | a counter factory, shared vs fresh captures, the loop-var trap | ✅ |
| 017 | scope_and_hoisting | `var` vs `let`/`const`, the temporal dead zone | ✅ |
| 018 | this_binding | `call`/`apply`/`bind`, a method losing its receiver | ✅ |
| 019 | optional_chaining | `?.`, `?.()`, `?.[]`, short-circuit semantics | ✅ |
| 020 | json_roundtrip | `stringify`/`parse`, replacer and reviver, what JSON drops | ✅ |
| 021 | dates | `Date`, ISO strings, UTC vs local, date arithmetic | ✅ |
| 022 | map_and_set | `Map`/`Set` over plain objects, insertion order, key identity | ✅ |
| 023 | object_statics | `keys`/`values`/`entries`/`fromEntries`/`assign` | ✅ |
| 024 | getters_and_setters | accessor properties in a literal, computed on read | ✅ |
| 025 | classes | `class`, constructor, methods, `instanceof` | ✅ |
| 026 | inheritance | `extends`, `super`, overriding, `super` in a method | ✅ |
| 027 | static_and_private | `static` members, `#private` fields, `#x in obj` | ✅ |
| 028 | errors | `throw`, `try`/`catch`/`finally`, a custom `Error` subclass | ✅ |
| 029 | regex_basics | `test`/`match`/`replace`, groups, flags | ✅ |
| 030 | named_groups | named capture groups, `matchAll`, `/g` statefulness | ✅ |
| 031 | for_of_vs_for_in | iterating values vs keys, why `for..in` walks the prototype | ✅ |
| 032 | iterable_protocol | making an object iterable with `Symbol.iterator` | ✅ |
| 033 | generators | `function*`, `yield`, spreading a generator, laziness | ✅ |
| 034 | modules | named vs default exports, live bindings, `import *` | ✅ |
| 035 | labeled_loops | labeled `break`/`continue`, escaping nested loops | ✅ |

## 02-intermediate (036–070)

| #   | Slug | Concepts | State |
|-----|------|----------|-------|
| 036 | promise_basics | the constructor, `then`/`catch`/`finally`, settle-once | ✅ |
| 037 | async_await | `async` functions, awaiting, how rejections surface | ✅ |
| 038 | promise_combinators | `all`/`allSettled`/`race`/`any`, `AggregateError` | ✅ |
| 039 | job_queue_ordering | sync vs microtask vs timer, `queueMicrotask` | ✅ |
| 040 | sequential_vs_parallel | awaiting in a loop vs starting first and awaiting later | ✅ |
| 041 | async_iteration | `for await..of`, async generators | ✅ |
| 042 | abort_controller | `AbortController`/`AbortSignal`, `reason`, `throwIfAborted` | ✅ |
| 043 | event_target | `EventTarget`, `CustomEvent`, `once`, removing a listener | ✅ |
| 044 | error_cause | the `cause` option, chained diagnosis, `AggregateError` | ✅ |
| 045 | prototype_chain | `Object.create`, `getPrototypeOf`, shadowing, `hasOwn` | ✅ |
| 046 | constructor_functions | the pre-`class` model, `prototype`, `new.target` | ✅ |
| 047 | property_descriptors | `defineProperty`, `writable`/`enumerable`/`configurable` | ✅ |
| 048 | freeze_and_seal | `freeze`/`seal`/`preventExtensions`, shallow immutability | ✅ |
| 049 | structured_clone | `structuredClone` vs JSON: cycles, `Map`, `Date`, functions | ✅ |
| 050 | weak_collections | `WeakMap` for private data, `WeakSet`, `WeakRef` | ✅ |
| 051 | symbols | unique keys, `Symbol.for`, `toStringTag`, `toPrimitive` | ✅ |
| 052 | currying | closures for currying, `bind` for partial application | ✅ |
| 053 | composition | `compose`/`pipe`, `reduceRight`, point-free plumbing | ✅ |
| 054 | memoize | `Map`-backed memoization, key derivation, cache invalidation | ✅ |
| 055 | debounce_throttle | trailing debounce and leading throttle on fake timers | ✅ |
| 056 | deep_equal | recursive structural comparison, `NaN`, `Date`, arrays, cycles | ✅ |
| 057 | deep_clone | recursive clone by hand, cycle handling with a seen-map | ✅ |
| 058 | group_by | `Object.groupBy` / `Map.groupBy`, and doing it with `reduce` | ✅ |
| 059 | flat_and_flatmap | `flat(depth)`, `flatMap`, flattening recursively | ✅ |
| 060 | array_from | `Array.from` with an iterable plus a map fn, `Array.of`, holes | ✅ |
| 061 | manual_iterator | a hand-written iterator object, `return()` on early exit | ✅ |
| 062 | generator_delegation | `yield*`, two-way `gen.next(value)` | ✅ |
| 063 | generator_state_machine | a generator as a coroutine driving a state machine | ✅ |
| 064 | tagged_templates | tag functions, `strings.raw`, interleaving values | ✅ |
| 065 | lazy_properties | a getter that replaces itself on first read | ✅ |
| 066 | mixins | `Object.assign` mixins vs the class-factory pattern | ✅ |
| 067 | intl | `Intl.NumberFormat`/`DateTimeFormat`/`RelativeTimeFormat`, explicit locales | ✅ |
| 068 | text_encoding | `TextEncoder`/`TextDecoder`, code units vs code points | ✅ |
| 069 | typed_arrays | `Uint8Array`, `DataView`, endianness, views over one buffer | ✅ |
| 070 | bigint | arbitrary precision, no mixing with `Number`, conversions | ✅ |

## 03-advanced (071–090)

| #   | Slug | Concepts | State |
|-----|------|----------|-------|
| 071 | proxy_basics | `get`/`set`/`has`/`deleteProperty` traps | ⬜ |
| 072 | reflect_and_receiver | `Reflect.*`, why a trap must forward the receiver | ⬜ |
| 073 | proxy_invariants | traps a non-configurable property will not let you lie about | ⬜ |
| 074 | revocable_proxy | `Proxy.revocable`, capability revocation | ⬜ |
| 075 | well_known_symbols | `Symbol.hasInstance`, `toPrimitive`, `species` | ⬜ |
| 076 | iterator_helpers | `.map`/`.filter`/`.take`/`.drop` on iterators, laziness | ⬜ |
| 077 | concurrency_pool | a task pool with a fixed concurrency limit | ⬜ |
| 078 | retry_backoff | retry with backoff, deterministic via an injected sleep | ⬜ |
| 079 | event_emitter | a Node-style emitter: `on`/`once`/`off`/`emit`, listener errors | ⬜ |
| 080 | observable_lite | a push stream with teardown on unsubscribe | ⬜ |
| 081 | web_streams | `ReadableStream`, `TransformStream`, piping | ⬜ |
| 082 | disposables | `Symbol.dispose`, `DisposableStack`, LIFO teardown | ⬜ |
| 083 | generator_cancellation | `gen.return()`/`gen.throw()`, `finally` in a generator | ⬜ |
| 084 | super_lookup | home objects: `super` in an object literal, accessor inheritance | ⬜ |
| 085 | immutable_updates | nested updates with structural sharing, no mutation | ⬜ |
| 086 | dynamic_import | `import()`, import cycles, module singleton state | ⬜ |
| 087 | async_local_storage | `node:async_hooks` context propagation across awaits | ⬜ |
| 088 | lru_cache | `Map` insertion order as the recency list | ⬜ |
| 089 | binary_records | parsing a length-prefixed binary record with `DataView` | ⬜ |
| 090 | regex_advanced | lookbehind, sticky `y`, unicode property escapes, replacer fn | ⬜ |

## 04-expert (091–100)

| #   | Slug | Concepts | State |
|-----|------|----------|-------|
| 091 | template_dsl | a tagged-template query builder that cannot be injected into | ⬜ |
| 092 | deep_reactive_proxy | nested proxies, change paths, arrays, identity caching | ⬜ |
| 093 | effects_runtime | a generator-driven effect interpreter (yield a request, resume) | ⬜ |
| 094 | structured_concurrency | a task group: first failure cancels the siblings | ⬜ |
| 095 | async_pipeline | async-generator stages, early exit, `finally` cleanup order | ⬜ |
| 096 | signals | dependency tracking, computed values, batched notification | ⬜ |
| 097 | transducers | composable reducers, one pass, early termination | ⬜ |
| 098 | worker_threads | `node:worker_threads`, message passing, transferables | ⬜ |
| 099 | atomics_ring_buffer | `SharedArrayBuffer` + `Atomics` single-producer ring buffer | ⬜ |
| 100 | promise_from_scratch | a Promises/A+ `then` with real microtask scheduling | ⬜ |
