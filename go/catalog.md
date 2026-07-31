# Go — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded · ⬜ planned.

## Seeded so far

| #   | Tier     | Package    | Concepts                        | Status |
|-----|----------|------------|---------------------------------|--------|
| 001 | Beginner | fizzbuzz   | control flow, strconv, modulo   | ✅     |
| 071 | Advanced | lrucache   | generics, container/list, map   | ✅     |

## Beginner (001–035) — fundamentals
Variables & types, `for` (the only loop), slices & arrays, maps, `range`,
functions & multiple returns, `struct`, methods, pointers, `error` values &
`errors.New`, `strings`/`strconv`, `fmt`, runes & bytes, `sort`, basic I/O.

## Intermediate (036–070) — idioms & stdlib
Interfaces & satisfaction, `error` wrapping (`%w`, `errors.Is/As`), goroutines &
`sync.WaitGroup`, channels, `select`, `sync.Mutex`/`RWMutex`, `context`, generics
(type params & constraints), `encoding/json`, `io.Reader`/`Writer`, `bufio`,
table-driven tests, `time`, `regexp`, functional options.

## Advanced (071–090) — concurrency & performance
Worker pools, fan-in/fan-out, pipelines with `context` cancellation, rate limiting,
`sync.Pool`, atomic operations, `errgroup`, deadlock/race debugging (`-race`),
custom `error` trees, `container/heap`, reflection basics, benchmarking (`testing.B`),
memory-aware buffer reuse, graceful shutdown.

## Expert (091–100) — systems & architecture
A concurrent in-memory key-value store, an HTTP router from scratch, middleware
chaining, a pub/sub broker, a job scheduler, a streaming line processor, a
connection pool, a small state machine, a plugin architecture via interfaces, a
load-shedding server with backpressure.
