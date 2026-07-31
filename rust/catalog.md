# Rust — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded · ⬜ planned.

## Seeded so far

| #   | Tier         | Module        | Concepts                       | Status |
|-----|--------------|---------------|--------------------------------|--------|
| 001 | Beginner     | ex001_anagram | iterators, chars, sorting      | ✅     |
| 036 | Intermediate | ex036_rle     | peekable, String, round-trip   | ✅     |

## Beginner (001–035) — fundamentals
`let`/`mut`, shadowing, scalar & compound types, ownership intro, `String` vs
`&str`, slices, `Vec`, `HashMap`, `match`, `if let`, `Option`, `Result`, `enum`,
`struct`, methods & `impl`, iterators (`map`/`filter`/`collect`), `for` loops.

## Intermediate (036–070) — idioms & the borrow checker
Borrowing & lifetimes intro, traits & `impl Trait`, generics & bounds, `derive`
macros, error handling with `?` and custom errors (`thiserror`-style), closures &
`Fn` traits, `Iterator` implementations, `From`/`Into`, `Cow`, pattern matching
depth, modules & visibility, `Box<dyn Trait>`, unit & integration tests.

## Advanced (071–090) — ownership at scale, concurrency
Explicit lifetimes, `Rc`/`RefCell`/`Arc`/`Mutex`, interior mutability, threads &
`std::thread`, channels (`mpsc`), `Send`/`Sync`, trait objects vs generics,
`unsafe` fundamentals, custom smart pointers (`Deref`), zero-cost iterators, a
generic LRU cache, declarative macros (`macro_rules!`), `no_std`-friendly code.

## Expert (091–100) — systems & abstractions
A typestate-pattern builder, an arena allocator, a small `async` executor, a
lock-free counter, a parser-combinator library, a mini interpreter, a `Future`
implementation by hand, a thread pool, a zero-copy binary decoder, a procedural
macro (intro).
