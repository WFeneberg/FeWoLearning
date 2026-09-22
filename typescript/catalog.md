# TypeScript — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + tests + solution present, red and green verified) ·
⬜ planned.

This table is the track's progress ledger and its work queue: the next five ⬜
rows are the next batch. Each exercise is `exercises/<tier>/exNNN_<slug>/index.ts`
with its reference at `solutions/<tier>/exNNN_<slug>/index.ts` — plus any sibling
modules the row needs, as ex026 and ex027 have; the tests live
once under `tests/<tier>/` as `exNNN_<slug>.test.ts` (runtime facts) and
`exNNN_<slug>.test-d.ts` (type-level facts). Not every row has both kinds — a
row about `infer` has no runtime to speak of, and a row about microtask
ordering has no interesting type.

**Status: 35 ✅ / 65 ⬜** — `01-beginner` complete.

Type-level facts are graded with `toEqualTypeOf` only. `toMatchTypeOf` is
green against `any` and must never carry a row on its own — see
[`README.md`](README.md) §"How a type test lies".

## Beginner (001–035) — the type system's ground floor

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | structural_typing | structural assignability, excess-property checks on fresh object literals, why a matching shape is enough | ✅ |
| 002 | type_vs_interface | `type` alias vs `interface`, what only each can do, when the choice is observable | ✅ |
| 003 | literal_types | string/number literal types, widening, `as const` | ✅ |
| 004 | union_narrowing | `typeof` narrowing, control-flow analysis, narrowing lost across closures | ✅ |
| 005 | optional_and_readonly | optional properties, `readonly`, `exactOptionalPropertyTypes` vs `undefined` | ✅ |
| 006 | tuple_basics | fixed-length tuples, labelled elements, rest elements | ✅ |
| 007 | safe_indexing | `noUncheckedIndexedAccess`, why `arr[0]` is `T \| undefined` | ✅ |
| 008 | function_types | call signatures, return typing, `void` vs `undefined` | ✅ |
| 009 | params_defaults_rest | optional parameters, defaults, typed rest parameters | ✅ |
| 010 | overloads | overload signatures vs the implementation signature | ✅ |
| 011 | enum_vs_as_const | numeric `enum` pitfalls, the `as const` object + union idiom | ✅ |
| 012 | unknown_vs_any | `unknown` forces narrowing, `any` disables checking | ✅ |
| 013 | never_exhaustiveness | `never` as the empty type, exhaustive `switch` | ✅ |
| 014 | type_predicates | user-defined guards, `x is T` | ✅ |
| 015 | discriminated_unions | tagged unions, switching on the discriminant | ✅ |
| 016 | generics_and_constraints | first type parameters, `extends` constraints | ✅ |
| 017 | generic_array_helpers | `first`/`last`/`chunk` with preserved element types | ✅ |
| 018 | keyof_basics | `keyof`, a typed property getter | ✅ |
| 019 | typeof_operator | deriving a type from a value with `typeof` | ✅ |
| 020 | indexed_access | `T["key"]`, nested indexed access, `T[number]` | ✅ |
| 021 | class_basics | fields, parameter properties, access modifiers | ✅ |
| 022 | implements_is_structural | `implements` checks but does not create nominality | ✅ |
| 023 | abstract_classes | `abstract` members, protected contracts | ✅ |
| 024 | accessors_and_static | getters/setters, `static`, static blocks | ✅ |
| 025 | private_hash_fields | `#field` vs `private`, the runtime difference | ✅ |
| 026 | modules_and_type_imports | named vs default exports, `import type`, module-level state as a singleton | ✅ |
| 027 | module_resolution_barrels | re-exports, barrel files, circular import hazards | ✅ |
| 028 | error_subclasses | extending `Error`, `cause`, `instanceof` narrowing | ✅ |
| 029 | catch_is_unknown | the catch binding is `unknown`, narrowing it safely | ✅ |
| 030 | result_type | a `Result<T, E>` union instead of exceptions | ✅ |
| 031 | async_await_basics | `async` return types, awaiting, error propagation | ✅ |
| 032 | event_loop_ordering | microtasks before macrotasks; a single thread, not a pool | ✅ |
| 033 | promise_all_tuple | `Promise.all` inferring a tuple, not an array | ✅ |
| 034 | iterables | `Symbol.iterator`, `for…of`, spreading an iterable | ✅ |
| 035 | parsing_unknown_json | `JSON.parse` returns `any`; validating into a real type | ✅ |

## Intermediate (036–070) — inference, transformation, the loop

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | mapped_types | `{ [K in keyof T]: … }` | ⬜ |
| 037 | mapped_modifiers | adding and stripping `readonly` and `?` with `+`/`-` | ⬜ |
| 038 | key_remapping | the `as` clause in a mapped type, filtering keys to `never` | ⬜ |
| 039 | conditional_types | `T extends U ? X : Y` | ⬜ |
| 040 | infer_basics | `infer` in a conditional type | ⬜ |
| 041 | rebuild_partial_required | reimplementing `Partial` and `Required` | ⬜ |
| 042 | rebuild_pick_omit | reimplementing `Pick` and `Omit` | ⬜ |
| 043 | rebuild_record | reimplementing `Record`, constrained key types | ⬜ |
| 044 | rebuild_returntype | reimplementing `ReturnType` and `Parameters` | ⬜ |
| 045 | rebuild_exclude_extract | reimplementing `Exclude`/`Extract` over unions | ⬜ |
| 046 | distributive_conditionals | naked type parameters distribute; `[T] extends [U]` stops it | ⬜ |
| 047 | template_literal_types | building string types by interpolation | ⬜ |
| 048 | intrinsic_string_types | `Uppercase`/`Capitalize` and friends | ⬜ |
| 049 | constrained_key_generics | `get<T, K extends keyof T>` and its inference | ⬜ |
| 050 | generic_defaults | default type arguments and when they are picked | ⬜ |
| 051 | inference_sites | contextual typing, where inference succeeds and where it gives up | ⬜ |
| 052 | assertion_functions | `asserts x is T`, why it needs an explicit annotation | ⬜ |
| 053 | in_and_instanceof_narrowing | narrowing with `in` and `instanceof` | ⬜ |
| 054 | typed_reducer | a discriminated-union action reducer | ⬜ |
| 055 | assert_never | an exhaustiveness helper that fails the build on a new case | ⬜ |
| 056 | promise_combinators | `allSettled`/`race`/`any` semantics and their result types | ⬜ |
| 057 | aggregate_error | `Promise.any` rejection, `AggregateError` | ⬜ |
| 058 | abort_signal | cancellation with `AbortController`, cooperative abort | ⬜ |
| 059 | generators | `function*`, the three type parameters of `Generator` | ⬜ |
| 060 | async_generators | `for await…of`, async iteration | ⬜ |
| 061 | iterator_protocol | implementing `Symbol.iterator` by hand | ⬜ |
| 062 | this_typing | `this` parameters, `ThisParameterType`, arrow vs function | ⬜ |
| 063 | bind_call_apply | typed `bind`/`call`/`apply`, `OmitThisParameter` | ⬜ |
| 064 | typed_reduce | generic accumulators, why the seed drives inference | ⬜ |
| 065 | deep_readonly | a recursive readonly mapped type | ⬜ |
| 066 | function_variance | parameter bivariance vs `strictFunctionTypes` | ⬜ |
| 067 | index_signatures | index signature vs `Record`, unknown keys | ⬜ |
| 068 | satisfies_operator | `satisfies` — checking without widening | ⬜ |
| 069 | const_type_parameters | `const` type parameters and deep literal inference | ⬜ |
| 070 | awaited_recursive | reimplementing `Awaited`, nested thenables | ⬜ |

## Advanced (071–090) — recursion, nominality, runtime metaprogramming

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | recursive_flatten | a recursive conditional type flattening nested arrays | ⬜ |
| 072 | deep_partial | recursive `Partial`, stopping at the right leaves | ⬜ |
| 073 | deep_readonly_arrays | recursion through arrays and tuples without losing arity | ⬜ |
| 074 | dotted_paths | deriving `"a.b.c"` path strings from an object type | ⬜ |
| 075 | get_by_path | the value type at a dotted path, and the runtime getter | ⬜ |
| 076 | branded_types | emulating nominal typing with a brand | ⬜ |
| 077 | validated_ids | smart constructors, an id that cannot be forged | ⬜ |
| 078 | generic_builder | a fluent builder accumulating known keys in its type | ⬜ |
| 079 | typed_event_emitter | an event map, payloads inferred per event name | ⬜ |
| 080 | declaration_merging | merging interfaces, and merging into a function | ⬜ |
| 081 | module_augmentation | augmenting another module's declarations | ⬜ |
| 082 | class_decorators | standard (TC39) class decorators | ⬜ |
| 083 | method_decorators | method decorators and the decorator context object | ⬜ |
| 084 | proxy_traps | `Proxy` with typed `get`/`set`/`has` traps | ⬜ |
| 085 | tiny_di_container | a metadata-free DI container, typed by token | ⬜ |
| 086 | async_pipeline | composing async generators into a pipeline | ⬜ |
| 087 | task_group | structured concurrency: fail-fast, cancel siblings | ⬜ |
| 088 | bounded_queue | an async queue with backpressure | ⬜ |
| 089 | variance_annotations | `in`/`out` on type parameters | ⬜ |
| 090 | pipe_and_compose | variadic `pipe`/`compose` typed through the chain | ⬜ |

## Expert (091–100) — the type level as a language

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | split_string_type | `Split<S, D>` by recursive template-literal matching | ⬜ |
| 092 | route_params | extracting `:id` parameters from a route literal | ⬜ |
| 093 | tuple_arithmetic | addition and subtraction via tuple length | ⬜ |
| 094 | tail_recursive_types | tail-recursive accumulation and why it survives depth | ⬜ |
| 095 | recursion_depth_limits | where "excessively deep" starts, and how to go past it | ⬜ |
| 096 | typed_query_builder | a mini query builder inferring its row shape | ⬜ |
| 097 | state_machine_types | illegal transitions rejected at compile time | ⬜ |
| 098 | dts_authoring | a hand-written `.d.ts` for an untyped JS module | ⬜ |
| 099 | json_serializable | a `Json<T>` type that rejects non-serializable members | ⬜ |
| 100 | schema_inference | a hand-rolled schema whose output type is inferred from it | ⬜ |
