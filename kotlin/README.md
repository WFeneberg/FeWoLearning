# Kotlin Track

A 100-exercise progression mirroring the other language tracks in this
monorepo: stubs under `exercises/` that the learner implements, paired with
reference implementations under `solutions/`. See [`catalog.md`](catalog.md)
for the full 100-row ledger — **100 / 100** seeded (stub + test + solution
present for every exercise).

## Unverified — no compiler has ever run over this track

**Important:** unlike `dotnet/`, `python/`, `vue/`, `go/`, `rust/`, and
`angular/`, this track has never been compiled or executed. The machine this
track was authored on has **no JDK, no Gradle, and no Kotlin compiler
installed**, so every stub, test, and solution here was written and
hand-reviewed line by line — self-reviewed for brace/paren balance, import
correctness, and signature compatibility with the stubs — but none of it has
ever been checked by `kotlinc` or run by `gradle test`. Treat this whole track
as **pending verification** on a machine with a real JDK 21 + Gradle + Kotlin
toolchain. If you hit a compile error or a test that doesn't behave as
documented, that's expected until someone runs the suite for the first time
and fixes what surfaces. (Same honest caveat as the `java/` track, which is in
the same unverified state.)

## Build layout

There is no separate `src/main` + `src/test` split. Each exercise's stub and
its sibling JUnit test live together under
`exercises/<tier>/exNNN_slug/`, and the whole `exercises/` tree is registered
as a single `test` source set in `build.gradle.kts` (the `test` source set
already depends on `main`, and `main` is empty here — there's nothing to
compile ahead of it).

`solutions/` mirrors the same `<tier>/exNNN_slug/` layout but is **never**
added to any source set, because solutions deliberately reuse the same
package/class names as their matching stubs. To check a solution, overlay it
onto the matching stub file in a throwaway copy of the tree and run the tests
there — don't add `solutions/` to the build.

```
kotlin/
  build.gradle.kts         # single `test` source set over exercises/
  settings.gradle.kts
  catalog.md                # 100-row progress ledger
  exercises/<tier>/exNNN_slug/
    <FileName>.kt            # stub — TODO()s at runtime, still compiles
    <FileName>Test.kt        # JUnit 5 test, same package
  solutions/<tier>/exNNN_slug/
    <FileName>.kt             # reference implementation, same public API as the stub
```

Difficulty tiers and numbering match every other track:
`01-beginner` (001–035), `02-intermediate` (036–070), `03-advanced` (071–090),
`04-expert` (091–100).

Exercises favor top-level functions, data classes, and idiomatic null-safety
over classes-for-the-sake-of-classes. Coroutine-heavy exercises (channels,
flows, supervisors, actors) use `kotlinx-coroutines-core` in the stub
signatures and `kotlinx-coroutines-test`'s `runTest { ... }` with virtual time
in their tests.

## Toolchain

`build.gradle.kts` applies the Kotlin JVM plugin (`kotlin("jvm") version
"2.0.21"`), targets `jvmToolchain(21)`, and depends on
`org.junit:junit-bom:5.11.0` (`junit-jupiter` + `junit-platform-launcher`) plus
`org.jetbrains.kotlinx:kotlinx-coroutines-core:1.9.0` and
`kotlinx-coroutines-test:1.9.0`. These versions are believed-current as of
authoring but **could not be verified** — bump them if Gradle reports they
don't exist once a real toolchain is used. No Gradle wrapper is checked in
yet, so a working `gradle` (or the wrapper) and a JDK 21 need to be installed
before any of this can run for the first time.

## Commands (once a JDK + Gradle are available)

| Action            | Command |
|-------------------|---------|
| Run all tests     | `gradle test` |
| Run one exercise  | `gradle test --tests "*RulesEngineTest*"` |

Run every command from inside this `kotlin/` folder, not the repo root.

## Verifying a solution manually

Since `solutions/` is excluded from the build, checking a reference
implementation means overlaying it onto its stub in a scratch copy:

1. Copy the whole `kotlin/` folder to a throwaway location.
2. For the exercise you want to check, copy
   `solutions/<tier>/exNNN_slug/<FileName>.kt` over
   `exercises/<tier>/exNNN_slug/<FileName>.kt` in the copy.
3. Delete the copy's `solutions/` folder (it's not part of the build anyway).
4. Run `gradle test --tests "*<FileName>Test*"` in the copy and confirm green.

Repeat per exercise — solutions are not verified in bulk since they're not
part of any single build target, and (per the caveat above) none of this has
been run even once yet.
