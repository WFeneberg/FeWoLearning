# JavaScript

The language itself, on plain Node — the object model, coercion, closures,
prototypes, the job queue, iterators, proxies, workers. Types belong to
[`typescript/`](../typescript), framework work to [`vue/`](../vue) and
[`angular/`](../angular); nothing here imports a UI library or a DOM.

100 exercises in the repo's four tiers: `01-beginner` 001–035,
`02-intermediate` 036–070, `03-advanced` 071–090, `04-expert` 091–100.
[`catalog.md`](catalog.md) is the ledger and the work queue.

## Setup

```
cd javascript
npm install
```

Node 26 / npm 11. No Docker, no Windows desktop session, no global tooling.

**Verified end-to-end 2026-09-24**: 1176 facts. `npm test` reports
100 files / 1176 failed / 0 passed on the untouched tree in about 10 s;
`npm run test:solutions` reports 1176 passed / 0 failed in about 3 s. The
two runs must report the same TOTAL — see lie #2 below.

## Commands

| What | Command |
|------|---------|
| Run everything (the **red** run — stubs) | `npm test` |
| Run everything against `solutions/` (the **green** run) | `npm run test:solutions` |
| One exercise | `npm run test:one -- "ex001"` |
| Watch | `npm run test:watch` |

## Layout

```
exercises/<tier>/exNNN_<slug>/index.js   # the stub you implement
solutions/<tier>/exNNN_<slug>/index.js   # the reference
tests/<tier>/exNNN_<slug>.test.js        # the grading, written ONCE
```

Tests live once and import through the `@ex` alias, which
[`vitest.shared.js`](vitest.shared.js) points at `exercises/` for `npm test`
and at `solutions/` for `npm run test:solutions`. That is the same mechanism
`typescript/` uses, and it is why `solutions/` here cannot drift silently the
way the repo's overlay-only tracks can — one suite grades both trees.

A row may add sibling modules next to its `index.js` (ex034 and ex086 do);
both trees must carry the same file names, since the alias swaps the whole
directory.

Stubs `throw new Error("TODO: …")` from the function body, so every module
still imports cleanly while unfinished.

## How a JavaScript test lies

Six ways, all of which have been hit while building this track. The general
rule behind them: **assert the mechanism, not an outcome several mechanisms
produce.**

1. **`toThrow()` with no matcher is green against every stub.** A stub throws
   `Error("TODO: …")`, which satisfies a bare `expect(fn).toThrow()` and even
   `toThrow(Error)`. Any row whose subject is throwing must assert a custom
   error class the stub does not construct, or match the message.
2. **A throw during a test file's *evaluation* reports 0 tests, not N
   failures** — a stub called at module level to build a fixture takes the
   whole file down, which looks exactly like a clean red run. So: **the red
   and green runs must report the same TOTAL.** Anything a stub can throw from
   belongs inside a test or behind a factory.
3. **`toEqual` cannot see identity, and `toBe` cannot see content.** A row
   about copying (`slice`, spread, `structuredClone`, an immutable update)
   must assert both — the result equals, *and* is not the same reference, and
   the input is unchanged. Without the third assertion a mutating
   implementation passes.
4. **A mutation test needs a snapshot taken before the call.** Asserting the
   input afterwards against a literal written in the test is fine; asserting
   it against a variable that *is* the input proves nothing.
5. **No async fact waits on a timer.** Where an ordering must be forced, the
   tests use a deferred (`Promise.withResolvers()`), so the sequence is the
   test's decision rather than the clock's. Time-based rows (debounce,
   throttle, backoff) run on `vi.useFakeTimers()` or take an injected sleep.
6. **Laziness is not observable from the result.** A generator, an iterator
   helper or a stream that produces the right values may still have consumed
   its whole source. Count the pulls with an instrumented source, or make the
   source endless — bounded by the consumer, never by the producer.

## Measured notes

- **`Object.groupBy` returns a null-prototype object** (ex058), so
  `toEqual({…})` against a plain literal fails in Vitest. Compare
  `Object.entries(...)`, or assert per key.
- **`for..in` walks the prototype chain but skips non-enumerable keys**, which
  is why class methods never show up in one (ex031, ex045) — they are
  non-enumerable, unlike a function assigned to `.prototype` by hand.
- **`structuredClone` throws `DataCloneError` on a function**, clones `Map`,
  `Set`, `Date`, `RegExp` and cycles, and drops nothing silently except a
  property whose value it cannot clone — there is no "best effort" mode
  (ex049).
- **`Intl` output is not stable across ICU versions**, so ex067 asserts parts
  (`formatToParts`) and explicit locales, never a formatted string compared to
  a literal with a particular space character in it. The ambient locale on
  this machine is `de-CH`; every Intl call in this track names its locale.
- **`DisposableStack` and `Symbol.dispose` exist in Node 26, but the `using`
  *declaration* is syntax** — ex082 grades `Symbol.dispose`,
  `DisposableStack`, `.adopt`, `.defer` and `.move` through explicit
  `.dispose()` calls, so the row does not depend on whether the bundler in
  front of Vitest passes `using` through.
- **Vitest's module runner is not plain Node ESM**, and ex086 measures two
  differences. Reading a not-yet-initialised `const` across an import cycle
  is a `ReferenceError` on `node file.mjs` and plain `undefined` here, since
  Vite rewrites modules into its own runtime; and the object `await import()`
  resolves to is extensible here, where a real module namespace is sealed.
  A row about module semantics must therefore assert what holds in both, or
  it grades the bundler.
- **A revoked proxy cannot be handed to a Vitest matcher.** Measured at
  ex074: `expect(revoked).not.toBe(other)` throws `TypeError: Cannot perform
  'has' on a proxy that has been revoked`, because the matcher probes the
  value for an asymmetric-matcher marker first. Compare with `Object.is(...)`
  inside the assertion, which runs no trap.
- **`JSON.stringify` probes a STRING key**, `toJSON`, before serialising — so
  a proxy that throws on unknown string keys (ex071) is fine with spread and
  template literals and dies inside a logger. The symbol exemption such a
  proxy needs does not cover it.
- **`toEqual` compares symbol-keyed properties**, so an array carrying
  `Symbol.isConcatSpreadable` is not `toEqual` a plain literal with the same
  elements (ex075). Spread it first.
- **A worker's entry file is loaded by Node, not by Vitest.** Measured at
  ex098: `new Worker(new URL("@ex/…", import.meta.url))` fails, because the
  alias is a bundler concept with no meaning at runtime. Build the URL
  inside the exercise module from its own `import.meta.url`, and keep
  `worker.js` plain ESM with `node:` imports only — it gets no transform,
  no alias and no test globals. Running such a module under
  `node --input-type=module -e …` also fails: the worker inherits the flag
  and refuses it.
- **`SharedArrayBuffer` needs no flag in Node** (ex099) and `Atomics.wait` is
  forbidden on the main thread — the ring buffer row is graded
  single-threaded with `Atomics.load`/`store`, and ex098 is the row that
  actually starts a worker.
