# TypeScript Track

TypeScript as a *language*, not as the type layer of a framework — `vue/` and
`angular/` already cover that. Roughly 60 % type system and 40 % runtime
semantics, weighted toward the two things furthest from C#: **structural
instead of nominal typing**, and **a single-threaded event loop instead of a
thread pool**.

Tested with **Vitest 5.0.1** and **TypeScript 7.0.2**, both pinned exactly.

## Setup (once)

```powershell
cd typescript
npm install
```

## Commands

| Action                          | Command                              |
|---------------------------------|--------------------------------------|
| Run all tests (the **red** run) | `npm test`                           |
| Run against solutions (**green**)| `npm run test:solutions`            |
| Watch mode                      | `npm run test:watch`                 |
| Run tests by name               | `npm run test:one -- "narrowing"`    |
| Type gate, solutions            | `npm run typecheck:solutions`        |
| Type gate, exercises            | `npm run typecheck`                  |

## Layout

```
exercises/<tier>/exNNN_slug/index.ts   # the stub you edit
solutions/<tier>/exNNN_slug/index.ts   # reference implementation
tests/<tier>/exNNN_slug.test.ts        # runtime facts
tests/<tier>/exNNN_slug.test-d.ts      # type-level facts
```

Tests live **once**, outside both content trees, and import every exercise
through the `@ex/…` alias. `vitest.shared.ts` points that alias — and the
matching `tsconfig` — at `exercises/` or `solutions/`. That is this track's
equivalent of the .NET tracks' `-p:UseSolutions=true`, and it means
`solutions/` is type-checked and executed on every green run: it **cannot
drift silently** the way `vue/`'s and `go/`'s did before the 2026-08-03 audit.

Not every row has both kinds of test. A row about `infer` has no interesting
runtime, and a row about microtask ordering has no interesting type. Where a
row is graded one way only, its stub header says so and why.

Stubs keep the tree compiling: runtime stubs `throw new Error("TODO: …")`,
type-level stubs are `unknown`.

## How a type test lies

Every track in this repo carries such a register. These six were measured on
this machine against the pinned versions above, not reasoned about.

**1. `toMatchTypeOf` is green against `any`.** Measured:
`expectTypeOf<any>().toMatchTypeOf<string>()` passes, because it is an
assignability check and `any` is assignable to everything. Grade with
**`toEqualTypeOf` only** — it correctly rejects `any`, `never` *and*
`unknown`, all three measured. A row whose only assertion is `toMatchTypeOf`
grades nothing.

**2. A broken typecheck reports green, not red.** Hit twice while building
this track: an `include` glob matching no files (`TS18003`), and then TS 7's
removed `baseUrl` (`TS5102`). In both cases Vitest printed
`Type Errors: no errors` and counted **every type fact as passed**. So:

- `npm run typecheck:solutions` is a separate gate, because `tsc` exits
  non-zero on a config error where Vitest swallows it;
- and, as in `avalonia/`, **read the count, not just the word `Failed`**. The
  red run must show the expected number of failures, not merely some.

**3. A fact the stub's signature already satisfies grades nothing.** Found in
this very first batch: ex002's type test originally asserted
`Config["host"] === string`, which the stub declares before any work is done —
it passed on the untouched tree. It now asserts the whole merged shape, which
only the completed merge satisfies. Any fact about a declaration the stub
already carries must be widened until the stub fails it.

**4. Tuple element labels are not graded.** Measured: `[number, number]`
satisfies a `toEqualTypeOf<[latitude: number, longitude: number]>()` fact
exactly as the labelled version does, because labels are erased for
assignability. A row about labelled tuples can assert arity and element types
and nothing more; write the labels for the reader.

**5. An optional parameter is not an optional property.** Measured:
`Parameters<typeof css>` for `css(value: number, unit = "px", ...)` is
`[value: number, unit?: string | undefined, ...extras: string[]]` — the
`| undefined` is explicit, where `exactOptionalPropertyTypes` keeps it *out*
of an optional property's type (ex005). `toEqualTypeOf` distinguishes the two,
so a `Parameters` fact must spell the union out.

**6. A fresh object literal at a call site is excess-property-checked.** This
is why ex002's runtime test passes an inferred local rather than a literal: a
literal carrying a property the stub's interface does not yet declare is a
*type* error in the test file, which muddies the red count with a failure that
is not the exercise's. Assign to a local first, or keep the literal minimal.

## TypeScript 7

New to this repo, and it bites immediately: **TS 7 removed `baseUrl`**
(`TS5102`). Path aliases are plain `paths` entries resolved relative to the
`tsconfig`. Because the configs import `./vitest.shared.ts` by its real
extension, they also set `allowImportingTsExtensions` — legal here since
everything is `noEmit`.

`tsconfig.*.json` additionally turn on `noUncheckedIndexedAccess` and
`exactOptionalPropertyTypes`. Both are deliberate: several exercises exist
precisely to drill what they change, starting at ex005 and ex007.

## Expected state of the untouched tree

| Run | Expectation |
|-----|-------------|
| `npm test` | every exercise fact red, none passing |
| `npm run test:solutions` | every fact green |
| `npm run typecheck:solutions` | exit 0, zero errors |
| `npm run typecheck` | exit 1, one error per unimplemented type-level stub, **all of them in `.test-d.ts` files** — an error anywhere else is a defect |

Measured 2026-09-22 at 10 / 100 exercises: 67 facts, 67 red / 0 passed on the
untouched tree, 67 / 0 green against `solutions/`, 25 expected exercise-side
type errors and 0 on the solutions side.

See [`catalog.md`](catalog.md) — the 100-row progress ledger and the work
queue.
