# .NET / C# — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

## Seeded so far

| #   | Tier         | Slug            | Concepts                          | Status |
|-----|--------------|-----------------|-----------------------------------|--------|
| 001 | Beginner     | FizzBuzz        | control flow, modulo, switch expr | ✅     |
| 036 | Intermediate | RomanNumerals   | greedy, lookup tables, validation | ✅     |
| 071 | Advanced     | LruCache        | generics, LinkedList+Dictionary   | ✅     |

## Beginner (001–035) — language fundamentals
Value types & nullable, string formatting & interpolation, arrays/`List<T>`,
`Dictionary<K,V>`, `foreach`/`for`/`while`, pattern matching basics, `enum`,
tuples, `record` basics, `DateTime`/`TimeSpan`, exceptions & `try/catch`,
`IEnumerable` intro, simple LINQ (`Where`/`Select`), file read/write, `TryParse`.

## Intermediate (036–070) — idioms & the BCL
Full LINQ (`GroupBy`/`Aggregate`/`Join`), `IEqualityComparer`, generics &
constraints, extension methods, `Span<T>`/`Memory<T>` basics, `IDisposable`/`using`,
`async`/`await` fundamentals, `Task` composition, `record` value semantics,
`struct` vs `class`, custom exceptions, `IComparable`, JSON (`System.Text.Json`),
regular expressions, delegates/`Func`/`Action`, events.

## Advanced (071–090) — concurrency, performance, patterns
`Channel<T>` & producer/consumer, `Parallel`/PLINQ, `SemaphoreSlim`,
`CancellationToken` propagation, `IAsyncEnumerable`, expression trees, source
generators (intro), `ArrayPool<T>`, ref structs & `stackalloc`, custom awaitables,
DI container fundamentals, middleware/pipeline pattern, the Result/Option pattern,
minimal-API endpoints, benchmarking with BenchmarkDotNet.

## Expert (091–100) — architecture & framework depth
Clean/hexagonal architecture slice, CQRS + mediator, a small **Blazor** component
with state, an **Avalonia** MVVM view + view-model, a **WPF** value converter &
binding, an **Uno** shared view, Roslyn analyzer, a tiny DSL/interpreter,
event-sourced aggregate, high-throughput pipeline with backpressure.
