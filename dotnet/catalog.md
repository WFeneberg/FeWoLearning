# .NET / C# — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100. Stubs
live in `exercises/<tier>/ExNNN_<Slug>.cs`, their xUnit tests in the sibling
`tests/` project at the same path with a `Tests` suffix. Namespaces follow the
tier (`FeWoLearning.Exercises.Beginner` and friends), not the `NN-tier` folder
name, because C# identifiers cannot start with a digit.

**Status: 100 ✅ / 0 ⬜**

## Beginner (001–035) — language fundamentals

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | FizzBuzz | control flow, modulo, string handling | ✅ |
| 002 | NullableValueTypes | nullable value types (int?), null propagation, conditional logic | ✅ |
| 003 | StringFormatting | string interpolation, format specifiers, string handling | ✅ |
| 004 | ArrayStatistics | arrays, loops, comparisons, accumulation | ✅ |
| 005 | ListOperations | List<T> basics, mutation vs. returning new lists, order preservation | ✅ |
| 006 | WordFrequency | Dictionary<K,V> basics, string splitting, case-insensitive comparison | ✅ |
| 007 | SumOfDigits | for/while loops, modulo, integer division | ✅ |
| 008 | CollatzSteps | while loop, modulo, integer arithmetic | ✅ |
| 009 | GradeClassifier | switch expression, pattern matching, range/relational patterns | ✅ |
| 010 | TrafficLight | enum basics, switch expressions, extension methods | ✅ |
| 011 | PointTuple | tuples, tuple deconstruction, basic math (Math.Sqrt) | ✅ |
| 012 | PointRecord | record basics, value equality, immutable data | ✅ |
| 013 | AgeCalculator | DateTime arithmetic, date comparisons, leap years | ✅ |
| 014 | CountdownTimer | DateTime/TimeSpan arithmetic, string formatting | ✅ |
| 015 | SafeDivide | exceptions, try/catch, nullable value types | ✅ |
| 016 | FibonacciSequence | custom IEnumerable<T> via yield, iterator methods, deferred execution | ✅ |
| 017 | FilterEvenSquares | LINQ Where/Select, Enumerable.Range, lambda expressions | ✅ |
| 018 | FileLineCounter | file read/write, File.WriteAllLines/ReadAllLines, string.IsNullOrWhiteSpace | ✅ |
| 019 | ParseOrDefault | int.TryParse, out parameters, defensive parsing | ✅ |
| 020 | PalindromeCheck | string algorithms, case-insensitive comparison, filtering characters | ✅ |
| 021 | BubbleSort | array sorting algorithm, nested loops, swapping elements | ✅ |
| 022 | FactorialRecursive | recursion, base cases, argument validation | ✅ |
| 023 | VectorStruct | struct value semantics, copy-by-value, immutable-style operations | ✅ |
| 024 | SafeLookup | nullable reference types, arrays, predicates (Func<string, bool>) | ✅ |
| 025 | ParamsSum | params arrays, variable-length argument lists, aggregation | ✅ |
| 026 | TryParseCoordinates | out parameters, string splitting, TryParse, boolean return values | ✅ |
| 027 | MatrixIndexer | indexers, 2D arrays, operator/member overloading basics | ✅ |
| 028 | VectorOperators | operator overloading, value equality, struct design | ✅ |
| 029 | MathUtilsStatic | static classes/methods, conditional logic, boundary handling | ✅ |
| 030 | ShapeInterface | interfaces, properties, polymorphism basics | ✅ |
| 031 | AnimalInheritance | abstract class, inheritance, method overriding, polymorphism | ✅ |
| 032 | GenericStack | generic classes, generic type parameters, basic collection wrapping | ✅ |
| 033 | ObjectInitializerBuilder | object initializers, collection initializers, simple data classes | ✅ |
| 034 | StringBuilderJoin | StringBuilder usage, string escaping, iteration | ✅ |
| 035 | QueueStackSimulation | Queue<T>, Stack<T>, FIFO vs LIFO ordering | ✅ |

## Intermediate (036–070) — idioms & the BCL

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | RomanNumerals | greedy algorithms, lookup tables, argument validation | ✅ |
| 037 | GroupByCategory | LINQ GroupBy, projection with Select, aggregation (Count/Sum), ordering, working with records/tuples as results | ✅ |
| 038 | AggregateTotals | LINQ Aggregate (seed + accumulator func + result selector), tuples, immutable accumulation semantics | ✅ |
| 039 | OrderCustomerJoin | LINQ Join, key selectors, projection into anonymous/typed results | ✅ |
| 040 | CaseInsensitiveSet | IEqualityComparer<T>, HashSet<T> with a custom comparer, hash-code contracts (Equals/GetHashCode must agree) | ✅ |
| 041 | GenericRepository | generics, generic constraints (where T : IEntity), dictionaries, nullable reference types | ✅ |
| 042 | StringExtensions | extension methods, static classes, string/char manipulation | ✅ |
| 043 | SpanSliceParser | Span<T>/ReadOnlySpan<T> slicing, IndexOf on spans, int.Parse(ReadOnlySpan<char>), manual tokenizing loops, avoiding allocations on hot parsing paths | ✅ |
| 044 | MemoryBufferProcessor | Memory<T>/Span<T> basics, Memory<T>.Slice, ReadOnlySpan<T> iteration, chunked processing, argument validation | ✅ |
| 045 | DisposableResourcePool | IDisposable, the using pattern, defensive state checks, idempotency | ✅ |
| 046 | AsyncDataFetcher | async/await, Task<T> return types, Task.Delay, Task.WhenAll, preserving order across concurrent operations, argument validation | ✅ |
| 047 | ParallelTaskAggregator | Task composition, Task.WhenAll, async/await, LINQ over tasks | ✅ |
| 048 | RecordEqualityCheck | records, nested records, with-expressions, structural equality (Equals/==), GetHashCode consistency | ✅ |
| 049 | StructVsClassMutation | struct vs class semantics, pass-by-value vs pass-by-reference, mutable fields, defensive copying | ✅ |
| 050 | CustomExceptionHierarchy | custom exception hierarchies, exception constructors, error codes, throwing/propagating specific exception types | ✅ |
| 051 | ComparablePriority | IComparable<T>, custom ordering, List<T>.Sort, tie-breaking logic | ✅ |
| 052 | JsonSerializeConfig | System.Text.Json, JsonSerializerOptions, naming policies, ignoring null values when serializing | ✅ |
| 053 | JsonCustomConverter | System.Text.Json, JsonConverter<T>, Read/Write overrides, JsonSerializerOptions composition, value-type equality | ✅ |
| 054 | RegexEmailValidator | regular expressions, Regex.IsMatch, anchoring a pattern | ✅ |
| 055 | RegexLogParser | regular expressions, named capture groups, nullable records/tuples | ✅ |
| 056 | FuncPipeline | delegates, Func<>, composition, LINQ Aggregate | ✅ |
| 057 | EventPublisher | events, delegates, EventArgs, encapsulated mutable state | ✅ |
| 058 | LazyLinqPipeline | deferred execution, Select projections, side effects, iterator semantics (yield-based sequences re-run their source on each pull) | ✅ |
| 059 | ReadonlyCollectionWrapper | IReadOnlyList<T> vs. List<T>, ReadOnlyCollection<T>, the difference between a live view and a defensive copy/snapshot | ✅ |
| 060 | RecordInheritanceShapes | record inheritance, positional/init-only properties, value equality across a type hierarchy, virtual/abstract members on records | ✅ |
| 061 | PatternMatchingLists | list patterns, slice patterns, property patterns, pattern guards | ✅ |
| 062 | ValueTaskCache | ValueTask<T> vs Task<T>, synchronous vs asynchronous completion, avoiding unnecessary allocations on the hot (cached) path | ✅ |
| 063 | ObservableCounter | IObservable<T>/IObserver<T>, the subscription/unsubscription pattern, IDisposable, push-based notification | ✅ |
| 064 | CustomComparerSort | IComparer<T>, List<T>.Sort(IComparer<T>), tie-break comparisons | ✅ |
| 065 | MemoizedFibonacci | recursion, memoization, dictionaries, algorithmic complexity | ✅ |
| 066 | Tokenizer | string parsing/tokenizing, character classification, StringBuilder, input validation via exceptions | ✅ |
| 067 | BinarySearchImpl | divide-and-conquer, integer overflow-safe midpoint, loop invariants | ✅ |
| 068 | MergeSortImpl | recursion, divide and conquer, array slicing/merging | ✅ |
| 069 | CustomHashSet | hashing, bucketing, equality comparison, generic collections | ✅ |
| 070 | ProducerConsumerQueue | lock statement, shared mutable state, basic synchronization, Queue<T> as the backing store, TryDequeue-style non-blocking pop | ✅ |

## Advanced (071–090) — concurrency, performance, patterns

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | LruCache | generics, LinkedList + Dictionary, eviction policy | ✅ |
| 072 | ChannelPipeline | System.Threading.Channels, async producer/consumer, backpressure, ordering guarantees | ✅ |
| 073 | ParallelSum | Parallel.ForEach, thread-local partitioning, Interlocked accumulation | ✅ |
| 074 | SemaphoreThrottle | SemaphoreSlim, async/await, try/finally release discipline, bounded concurrency | ✅ |
| 075 | CancellableOperation | async/await, CancellationToken propagation, ThrowIfCancellationRequested, cooperative cancellation semantics | ✅ |
| 076 | AsyncStreamReader | IAsyncEnumerable<T>, async iterators (yield return in an async method), Task.Delay, CancellationToken / [EnumeratorCancellation] | ✅ |
| 077 | ExpressionTreeBuilder | expression trees, Expression.Parameter/Constant/Lambda, delegate compilation | ✅ |
| 078 | ReflectionPropertyMapper | System.Reflection (PropertyInfo, GetValue/SetValue), type compatibility checks, generic constraints, member caching | ✅ |
| 079 | ArrayPoolBuffer | System.Buffers.ArrayPool<T>, Span<T>, idempotent IDisposable, resource ownership | ✅ |
| 080 | StackallocParser | ref structs, Span<T>/ReadOnlySpan<T>, stackalloc, slicing, MemoryExtensions parsing (no LINQ, no string.Split allocations) | ✅ |
| 081 | CustomAwaitable | awaitable/awaiter pattern, INotifyCompletion, manual continuations | ✅ |
| 082 | SimpleDiContainer | reflection, generics, constructor injection, lifetime management | ✅ |
| 083 | MiddlewarePipeline | delegate composition, closures, the "onion" middleware pattern (as used by ASP.NET Core, Express, etc.) | ✅ |
| 084 | ResultOptionPattern | discriminated-union-style types, functional combinators, short-circuiting | ✅ |
| 085 | MinimalEndpointHandler | DTO validation, discriminated-union-style result modelling, pure functions decoupled from a web framework | ✅ |
| 086 | BenchmarkComparison | dependency injection for testability, delegates, avoiding wall-clock/non-determinism in unit tests, simple statistics | ✅ |
| 087 | InterlockedCounter | System.Threading.Interlocked, race conditions, concurrent correctness | ✅ |
| 088 | ReaderWriterCache | ReaderWriterLockSlim, read/write lock scopes, thread safety | ✅ |
| 089 | ObjectPoolImpl | object pooling pattern, reference-identity tracking, resource reuse | ✅ |
| 090 | CircuitBreaker | state machines, resilience patterns, injecting a clock for determinism | ✅ |

## Expert (091–100) — architecture & framework depth

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | CleanArchitectureSlice | dependency inversion (ports & adapters), keeping orchestration/business rules in the application layer independent of infrastructure, testable hexagonal design | ✅ |
| 092 | CqrsMediator | CQRS, mediator pattern, generics with variance, dependency inversion | ✅ |
| 093 | BlazorCounterComponent | async event-handler state mutation, serializing concurrent async work (SemaphoreSlim), render/markup diffing, bUnit-style assertions against rendered markup | ✅ |
| 094 | AvaloniaMvvmViewModel | INotifyPropertyChanged, ICommand/RelayCommand pattern, derived ("CanIncrement") properties, command re-evaluation | ✅ |
| 095 | WpfValueConverter | IValueConverter Convert/ConvertBack contract, ConverterParameter handling, defensive type-checking at a binding boundary | ✅ |
| 096 | UnoSharedView | INotifyPropertyChanged, computed/derived bindable properties, change-detection, deterministic precedence rules, guard clauses | ✅ |
| 097 | RoslynAnalyzer | recursive-descent parsing, syntax tree modeling, reachability analysis, diagnostics reporting | ✅ |
| 098 | TinyDslInterpreter | tokenizing, recursive-descent parsing, precedence climbing, evaluation | ✅ |
| 099 | EventSourcedAggregate | event sourcing, aggregate replay, command validation, immutable events | ✅ |
| 100 | BackpressurePipeline | System.Threading.Channels, ValueTask synchronous-completion semantics, IAsyncEnumerable, backpressure vs. unbounded buffering | ✅ |
