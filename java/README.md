# Java Track

A 100-exercise progression mirroring the other language tracks in this
monorepo: stubs under `exercises/` that the learner implements, paired with
reference implementations under `solutions/`. See [`catalog.md`](catalog.md)
for the full 100-row ledger — **100 / 100** seeded (stub + test + solution
present for every exercise).

## Unverified — no compiler has ever run over this track

**Important:** unlike every other track in this repo, the Java track has never
been compiled or executed. The machine this track was authored on has no JDK
and no Gradle installed, so every stub, test, and solution here was written
and hand-reviewed line by line, but none of it has been checked by `javac` or
run by `gradle test`. Treat this whole track as **pending verification** on a
machine with a real JDK 21 + Gradle toolchain. If you hit a compile error or a
test that doesn't behave as documented, that's expected until someone runs the
suite for the first time and fixes what surfaces.

## Build layout

There is no separate `src/main` + `src/test` split. Each exercise's stub and
its sibling JUnit test live together under
`exercises/<tier>/exNNN_slug/`, and the whole `exercises/` tree is registered
as a single `test` source set in `build.gradle` (the `test` source set already
depends on `main`, and `main` is empty here — there's nothing to compile ahead
of it).

`solutions/` mirrors the same `<tier>/exNNN_slug/` layout but is **never**
added to any source set, because solutions deliberately reuse the same
class/package names as their matching stubs. To check a solution, overlay it
onto the matching stub file in a throwaway copy of the tree and run the tests
there — don't add `solutions/` to the build.

```
java/
  build.gradle             # single `test` source set over exercises/
  settings.gradle
  resources/               # classpath-root resources (see ex084 below)
  catalog.md               # 100-row progress ledger
  exercises/<tier>/exNNN_slug/
    <ClassName>.java       # stub — throws at runtime, still compiles
    <ClassName>Test.java   # JUnit 5 test, same package
  solutions/<tier>/exNNN_slug/
    <ClassName>.java       # reference implementation, same public API as the stub
```

Difficulty tiers and numbering match every other track:
`01-beginner` (001–035), `02-intermediate` (036–070), `03-advanced` (071–090),
`04-expert` (091–100).

### ex084's extra resource file

`ex084_service_loader_plugin` exercises `java.util.ServiceLoader`, which
requires a provider-configuration file on the classpath root at
`META-INF/services/<fully-qualified-service-interface-name>`. Because the
`test` source set's `java` roots point at `exercises/` (not a conventional
`src/test/resources`), that file lives in a separate top-level `resources/`
directory instead, wired in as the `test` source set's `resources` root:

```
resources/META-INF/services/fewolearning.exercises.advanced.ex084_service_loader_plugin.ServiceLoaderPlugin$Greeter
```

Don't move or rename this file — it must stay at the classpath root for
`ServiceLoader` to find it.

## Toolchain

`build.gradle` pins `JavaLanguageVersion.of(21)` and depends on
`org.junit:junit-bom:5.11.0` (`junit-jupiter` + `junit-platform-launcher`).
Nothing else is required to add — no wrapper is checked in yet, so a working
`gradle` (or the Gradle wrapper) and a JDK 21 need to be installed before any
of this can run for the first time.

## Commands (once a JDK + Gradle are available)

| Action              | Command |
|---------------------|---------|
| Run all tests       | `gradle test` |
| Run one exercise     | `gradle test --tests "*MiniDiContainerTest*"` |

Run every command from inside this `java/` folder, not the repo root.

## Verifying a solution manually

Since `solutions/` is excluded from the build, checking a reference
implementation means overlaying it onto its stub in a scratch copy:

1. Copy the whole `java/` folder to a throwaway location.
2. For the exercise you want to check, copy
   `solutions/<tier>/exNNN_slug/<ClassName>.java` over
   `exercises/<tier>/exNNN_slug/<ClassName>.java` in the copy.
3. Delete the copy's `solutions/` folder (it's not part of the build anyway).
4. Run `gradle test --tests "*<ClassName>Test*"` in the copy and confirm green.

Repeat per exercise — solutions are not verified in bulk since they're not
part of any single build target.
