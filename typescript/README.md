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

A row may add sibling modules next to its index.ts when it needs them —
ex026 has a units.ts, ex027 four files and a deliberate import cycle.
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

Every track in this repo carries such a register. These seven were measured on
this machine against the pinned versions above, not reasoned about.

**1. `toMatchTypeOf` is green against `any`.** Measured:
`expectTypeOf<any>().toMatchTypeOf<string>()` passes, because it is an
assignability check and `any` is assignable to everything. Grade with
**`toEqualTypeOf` only** — it correctly rejects `any`, `never` *and*
`unknown`, all three measured. A row whose only assertion is `toMatchTypeOf`
grades nothing.

**1b. The same hole reopens whenever a fact writes its own `extends`.**
`unknown` absorbs every assignability check, so a fact of the form
`X extends StubType ? true : false` is green while the stub is still
`unknown`. This has bitten three rows — ex008, ex047, ex050 — and the fix
each time was to **assert the accepted and the rejected cases as one
tuple**: `[true, true, false, false]` cannot be satisfied by a type that
accepts everything. Where a row has a positive and a negative half, they
belong in one fact, not two.

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

**6b. Control-flow analysis narrows an annotated variable by its
initializer.** Measured: `const value: string | null | undefined = "x"` is
already `string` at the next line, so a fact asserting that an assertion
function narrowed it is green on the untouched stub. Obtain the value from
a CALL whose declared return type is the union — the checker has nothing
to narrow from there. (An `unknown` annotation is not narrowed this way,
which is why ex052's assertString fact worked and its assertDefined one did
not.)

**7. `@ts-expect-error` is how a *rejection* gets graded.** Several rows are
about something the checker must refuse — an overload set hiding its
implementation signature (ex010), a parameter narrowed to `never` (ex013).
A fact cannot assert "this does not compile" directly, but
`@ts-expect-error` inverts it: the comment is itself an error when the line
below it compiles, so the fact is red exactly while the stub still accepts
the call and turns green when the finished code rejects it. Measured working
in both directions here. It is the only tool in the register that grades an
absence, and it is worth reaching for whenever a row's subject is what the
type system *forbids*.

One mechanical trap with it, measured at ex051: it suppresses only the line
IMMEDIATELY after it, and the compiler does not necessarily report a bad
call at the line where the call starts — a mismatched `pipe2(f, g)` is
reported at its first argument. A call broken across several lines
therefore leaves the directive unused, which is itself an error, and the
fact fails against correct code. Keep the offending call on one line.

Rows are also checked against a **plausible wrong implementation**, not just
against the stub — the second probe `Architecture/` and `security/` both
insist on. It has already earned its keep: returning `JSON.parse`'s result
unchanged (so `any`) fails ex012's fact, and dropping `as const` fails both
of ex011's, confirming those facts grade the mechanism rather than the value.

## Writing a stub that can be graded

One rule covers most of it: **a stub's signature must be WIDER than the
solution's.** Declare parameters and returns as `unknown`, so that

- every call a test makes compiles against the stub as well as the finished
  code — a narrower stub turns the test's calls into type errors in the test
  file instead of failing facts (this cost ex009 a rewrite);
- and the type facts still go red, because `unknown` is not what they expect.

That is why ex016's `identity(_value: unknown): unknown`, ex017's
`readonly unknown[]` and ex018's `key: unknown` look the way they do. The
learner's work is to narrow them.

Where widening is impossible — making a parameter optional, turning one into
a rest — the row either hands the learner that part of the signature and
grades the body (ex009), reaches the behaviour through a widened local
reference in the test, or is graded at the type level only.

**For a class, the same rule reads: declare every member with the LOOSEST
modifiers and let the learner tighten them.** ex021's stub carries
`id: unknown = undefined` and a public `balance`, so making `id` a readonly
string and `balance` private is what turns the facts green. Declaring them
correctly and leaving only the bodies to write would pre-satisfy every one of
them. The same applies to ex023, whose stub Shape is a plain class with public
methods precisely so that `abstract` and `protected` have somewhere to go.

Note the asymmetry with `@ts-expect-error` this creates. A fact that grades a
refusal is red while the stub still *permits* the thing — so a stub member
must be permissive, or the expect-error is satisfied before any work is done
and the fact is green from the start. Two of ex024's facts were written and
then deleted for exactly that: its `fahrenheit` is a getter with no setter in
the stub already, so “cannot be assigned” was true on the untouched tree.

## A row that rebuilds a library type cannot detect delegation

ex041–ex045 ask the learner to write Partial, Pick, Omit, Record,
ReturnType, Exclude and friends from scratch. A fact asserts the resulting
type and cannot see how it was reached, so `type MyPartial<T> = Partial<T>`
passes — measured, not assumed.

Rather than pretend otherwise, every one of those five rows also carries a
type the standard library has no answer for: `PartialBy`, a `MyOmit`
constrained to `keyof T`, `MyRecord`'s PropertyKey constraint, the missing
`AsyncReturnType`, and `MyNonNullable` built from the learner's own
`MyExclude`. Those cannot be delegated, and each row's header says plainly
which part is on trust.

The same five rows carry the one probe worth keeping: ex045's facts fail
against a NON-distributive `[T] extends [U]` version (4 of them), so that
row genuinely grades distribution rather than the answer.

## Two inference traps, measured

Both cost a green run while building `02-intermediate`, and both are the
kind of thing that reads as correct.

**A zero-parameter function matches a signature that takes parameters.** So
`T extends (first: infer P, ...rest: never[]) => unknown ? P : never` does
NOT give `never` for `() => void` — it matches, and `P` infers as `unknown`.
Infer the whole parameter tuple and destructure it instead. ex040's header
says so.

**A mapped type is homomorphic only when its source is exactly `keyof T`.**
`{ [K in keyof T]: T[K] }` copies `readonly` and `?` across for free;
`{ [K in Extract<keyof T, string>]: T[K] }`, which looks equivalent, drops
both. Verified by probe: the second form fails ex037's preservation fact.
This is why `Partial` and `Readonly` can be one line each, and why a helper
that "just filters the keys a bit" quietly loses modifiers.

## A crash is worse than a red fact

`Architecture/` has the rule that a fact which HANGS is worse than one that
fails, because the suite stalls and reports nothing. This track has a sharper
version, measured at ex034: spreading an endless iterable —
`[...values].slice(0, count)`, the obvious shape for a `take` — does not fail
that row's fact. It takes the whole test PROCESS down with a V8
out-of-memory crash, exit 134, with no catchable error and no timeout to
save it. The learner sees a heap dump instead of a red line.

Nothing in the harness can prevent that, so the stub header warns about it
explicitly. Any later row that feeds an unbounded source to learner code
should do the same.

## Module rows are runtime-graded, necessarily

A missing export is an error at the IMPORT site, so a stub must already
declare every name its tests import — which pre-satisfies any fact about the
module's structure. ex026 was written with two type facts and both were
measured green on the untouched tree; the file is gone rather than weakened.
The same reasoning covers the choice between `export { x as y } from "./m"`
and a wrapper function, and between `export default f` and a separate
declaration: measured, a wrapper passes every one of ex026's facts, exactly
as the re-export does.

ex027 is the exception that proves it. Its facts load the barrel with
`await import(...)` and read it as a `Record<string, unknown>`, which never
errors on a name that is not there — so "the barrel does not expose this
yet" becomes a failing fact instead of a type error in the test file.

## Three things about classes that no test can see

Recorded so a future row does not claim to grade them:

- **Parameter properties.** `constructor(public readonly id: string)` and a
  field declaration plus an assignment produce an identical class type. ex021
  asks for the former and grades the field's type and modifiers.
- **`implements`.** It checks the class at its declaration and adds nothing to
  its type, so deleting it from a correct class changes neither types nor
  behaviour. ex022's subject is precisely that, and its facts grade the
  consequence — who is assignable to the interface — with a class that never
  declares it as the control.
- **Which mechanism set a static.** A `static { }` block and a field
  initializer leave identical evidence, so ex024 grades the value.

And one that *is* worth knowing before writing such a row, measured here:
**a `readonly` static cannot be assigned from a static block.** Both
`Cls.X = …` and `this.X = …` inside `static { }` fail with TS2540, unlike a
readonly instance field, which a constructor may assign. A readonly static
needs a plain initializer; a static block is for the mutable ones. The green
run caught this — the first draft of ex024's solution did not compile.

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

Measured 2026-09-22 at 55 / 100 exercises — all of `01-beginner` and
`02-intermediate` through ex055: 440 facts, 440 red / 0 passed on the
untouched tree, 440 / 0 green against `solutions/`, 206 expected
exercise-side type errors and 0 on the solutions side.

The ratio shifts as the tiers go on: a type-level row contributes one type
error per unimplemented type, so `02-intermediate` adds far more of them per
exercise than `01-beginner` did. ex040 has no runtime facts at all.

See [`catalog.md`](catalog.md) — the 100-row progress ledger and the work
queue.
