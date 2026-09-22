# `typescript/` — Track Design

**Date:** 2026-09-22
**Status:** approved; ex001–ex005 land in the first session, ex006–ex100 follow
in batches of five per `CLAUDE.md`.

## Why the track exists

`vue/` and `angular/` already drill TypeScript *inside a framework*. Neither
drills the language. This track is about TypeScript itself, weighted roughly
60 % type system / 40 % runtime semantics — the two areas furthest from C#,
which is the owner's home ecosystem: **structural instead of nominal typing**,
and **a single-threaded event loop instead of a thread pool**.

## Toolchain

Node 26.7.0 / npm 11.19.0, **Vitest 5.0.1**, **TypeScript 7.0.2**, both pinned
exactly. Vitest prints its own warning that the typecheck runner is
experimental and does not follow SemVer, so a floating range would break the
track on a patch bump.

**TypeScript 7 removed `baseUrl`** (`TS5102`). Path aliases are plain `paths`
entries resolved relative to the `tsconfig`. This is new to the repo — every
other Node track predates it.

## Layout

```
typescript/
  exercises/<tier>/exNNN_slug/index.ts   # stub the learner edits
  solutions/<tier>/exNNN_slug/index.ts   # reference, same module shape
  tests/<tier>/exNNN_slug.test.ts        # runtime facts
  tests/<tier>/exNNN_slug.test-d.ts      # type-level facts
  catalog.md  README.md  package.json
  tsconfig.exercises.json  tsconfig.solutions.json  vitest.config.ts
```

Tests live **once**, outside both content trees, and import through `@ex/…`.
This departs from the colocation `vue/` uses and matches the `tests/` project
every .NET track here has.

## Red/green: the `UseSolutions` equivalent

`vitest.config.ts` points the `@ex` alias at `exercises/` or, when
`USE_SOLUTIONS=true`, at `solutions/`, and selects the matching tsconfig for
the typecheck pass. So:

| | command |
|---|---|
| red | `npm test` |
| green | `npm run test:solutions` |

One suite, two targets. `solutions/` is therefore type-checked and executed on
every green run and **cannot drift silently**.

Two alternatives were rejected:

- **Copy the test into `solutions/`** (the `vue/` mechanism). `CLAUDE.md`
  records that this is exactly what hid five broken `vue/` solutions and four
  defective `go/` tests until the 2026-08-03 audit. Two copies drift.
- **Overlay into a throwaway tree** (the `python`/`go`/`rust` recipe). Correct,
  but a manual step per check instead of a flag.

## How a type test lies — measured 2026-09-22

Every track here carries such a register. Three entries, each measured on this
machine with the pinned versions above, not reasoned about:

1. **`toMatchTypeOf` is green against `any`.**
   `expectTypeOf<any>().toMatchTypeOf<string>()` passes — it is an
   assignability check, and `any` is assignable to everything. Grade with
   **`toEqualTypeOf` only**, which correctly rejects `any`, `never` *and*
   `unknown` (all three measured).

2. **A broken typecheck reports green, not red.** Hit twice while probing:
   an `include` glob matching nothing (`TS18003`), then TS 7's removed
   `baseUrl` (`TS5102`). In both cases Vitest printed `Type Errors: no errors`
   and counted every type fact as **passed**. The guard is therefore a separate
   `npm run typecheck:solutions` — `tsc` exits non-zero on a config error where
   Vitest swallows it — plus the `avalonia/` rule: **read the test count, not
   just the word `Failed`**.

3. **A type stub must be `unknown`, never `any`.** `unknown` fails
   `toEqualTypeOf` (so the stub is red) and still compiles (so the tree
   builds). Runtime stubs `throw new Error("TODO: …")`. Both preserve the
   repo-wide invariant that an unfinished stub compiles.

## Catalog shape

100 rows in the repo's standard four difficulty tiers. Unlike `security/`,
`telemetry/` and `Architecture/`, difficulty is a meaningful axis for a
language track, so tiers rather than subject blocks.

| Tier | Rows | Content |
|---|---|---|
| `01-beginner` | 001–035 | literal types, unions, narrowing, structural vs nominal typing, `interface` vs `type`, tuples, `enum` vs `as const`, first generics, classes, modules |
| `02-intermediate` | 036–070 | constraints, `keyof`/`typeof`, indexed access, mapped types, conditional types with `infer`, discriminated unions, type guards and assertion functions, re-implementing utility types, Promise combinators, generators |
| `03-advanced` | 071–090 | recursive conditional types, variance, branded types, generic fluent APIs, declaration merging, module augmentation, decorators, `Proxy`/`Reflect`, async iterators, `satisfies`, `const` type parameters |
| `04-expert` | 091–100 | template-literal parsers at the type level, tail-recursive type arithmetic, hand-written `.d.ts`, instantiation depth and type performance, `in`/`out` variance annotations, exhaustive state machines |

## Per-batch recipe

As `CLAUDE.md` §"Adding or completing exercises", with two track-specific
additions at step 4:

- Confirm the red run's **count**, not just that it failed — a silently
  skipped typecheck inflates the passed column.
- Confirm no type fact uses `toMatchTypeOf` as its only assertion.

And at step 6 the type gate is `npm run typecheck:solutions`, which must be
clean. `npm run typecheck` over `exercises/` is expected to report one type
error per unimplemented type-level stub; that count is the red signal, not a
defect.
