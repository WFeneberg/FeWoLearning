# Flutter/Dart Track

A 100-exercise progression mirroring the other language tracks in this
monorepo: stubs under `exercises/` that the learner implements, paired with
reference implementations under `solutions/`. See [`catalog.md`](catalog.md)
for the full 100-row ledger — **100 / 100** seeded (stub + test + solution
present for every exercise).

## Unverified — no Flutter/Dart SDK has ever run over this track

**Important:** unlike `dotnet/`, `python/`, `vue/`, `go/`, `rust/`, and
`angular/`, this track has never been compiled or executed. The machine this
track was authored on has **no Flutter SDK and no Dart SDK installed**, so
every stub, test, and solution here was written and hand-reviewed line by
line — self-reviewed for brace/paren balance, import correctness, and
signature compatibility with the stubs — but none of it has ever been checked
by `dart analyze`, `dart test`, or `flutter test`. Treat this whole track as
**pending verification** on a machine with a real Flutter/Dart toolchain. If
you hit an analysis error or a test that doesn't behave as documented,
that's expected until someone runs the suite for the first time and fixes
what surfaces. (Same honest caveat as the `java/` and `kotlin/` tracks, which
started the same way.)

A few exercises are flagged as higher-risk than the rest, worth checking
first once a real toolchain is available:
- **ex069** (`golden_test_basics`) references a golden PNG under `goldens/`
  that doesn't exist yet — it needs `flutter test --update-goldens` on a real
  machine to generate the baseline before the test can pass.
- **ex094** (`platform_channel_bidirectional`) mocks both a `MethodChannel`
  and an `EventChannel` via `TestDefaultBinaryMessengerBinding` — plausible
  but unverified test-binding API usage.
- **ex095** (`isolate_worker_pool`) sends closures across isolates via
  `Isolate.run`; correct in spirit (Dart supports sending non-static
  closures that don't capture non-sendable state) but never exercised here.

## Build layout

There is no `test/` root directory the way a typical Dart/Flutter package
uses. Each exercise's stub and its sibling test live together under
`exercises/<tier>/exNNN_slug/`, the same convention every other track in this
monorepo uses — pass that directory (or the whole `exercises/` tree) to
`dart test` / `flutter test` explicitly, since it's not the package's default
test root.

`solutions/` mirrors the same `<tier>/exNNN_slug/` layout but is **never**
part of a test run, because solutions deliberately reuse the same file/
function names as their matching stubs. To check a solution, overlay it onto
the matching stub file in a throwaway copy of the tree and run the tests
there — don't point `dart test`/`flutter test` at a tree containing both.

```
flutter/
  pubspec.yaml              # Flutter package manifest (flutter/provider/riverpod/bloc/shared_preferences + dev-only test/mockito/golden_toolkit/integration_test)
  catalog.md                 # 100-row progress ledger
  exercises/<tier>/exNNN_slug/
    <slug>.dart               # stub — throws UnimplementedError at runtime, still analyzes/compiles
    <slug>_test.dart          # package:test (pure-Dart tiers) or flutter_test (widget tiers)
  solutions/<tier>/exNNN_slug/
    <slug>.dart                # reference implementation, same public API as the stub
```

Difficulty tiers and numbering match every other track:
`01-beginner` (001–035), `02-intermediate` (036–070), `03-advanced` (071–090),
`04-expert` (091–100).

`01-beginner` and most of `02-intermediate` are pure Dart — no widget tree,
so their tests import `package:test` directly and run under plain
`dart test`. From ex053 onward (`stateless_widget_basics`), exercises build
actual widgets and their tests import `package:flutter_test` instead, which
needs the full Flutter SDK (not just Dart) to run.

## Toolchain

`pubspec.yaml` targets `sdk: '>=3.4.0 <4.0.0'` and `flutter: '>=3.22.0'`,
depends on the `flutter` SDK package plus `provider`, `flutter_riverpod`,
`flutter_bloc`, and `shared_preferences`, and dev-depends on `flutter_test`
(SDK), `test`, `mockito`, `golden_toolkit`, and `integration_test` (SDK).
These versions are believed-current as of authoring but **could not be
verified** — bump them if `pub` reports they don't exist once a real SDK is
installed. `get_it` and `rxdart` are deliberately **not** dependencies: the
exercises whose catalog concepts mention them (`dependency_injection_basics`,
`stream_combine_latest`, `stream_debounce_search`) hand-roll a minimal
equivalent instead ("in the style of get_it/rxdart"), to keep the dependency
surface smaller for code nobody has run yet.

## Commands (once a Flutter/Dart SDK is available)

| Action                          | Command |
|----------------------------------|---------|
| Install dependencies             | `flutter pub get` |
| Run all pure-Dart exercises       | `dart test exercises` |
| Run all exercises (incl. widgets) | `flutter test exercises` |
| Run one exercise                  | `dart test exercises/01-beginner/ex001_var_final_const/var_final_const_test.dart` |

Run every command from inside this `flutter/` folder, not the repo root.

## Verifying a solution manually

Since `solutions/` is excluded from any test run, checking a reference
implementation means overlaying it onto its stub in a scratch copy:

1. Copy the whole `flutter/` folder to a throwaway location.
2. For the exercise you want to check, copy
   `solutions/<tier>/exNNN_slug/<slug>.dart` over
   `exercises/<tier>/exNNN_slug/<slug>.dart` in the copy.
3. Delete the copy's `solutions/` folder (it's never part of a test run
   anyway).
4. Run `dart test <path-to-the-exercise-folder>` (or `flutter test` for a
   widget exercise) in the copy and confirm green.

Repeat per exercise — solutions are not verified in bulk since they're not
part of a single test target, and (per the caveat above) none of this has
been run even once yet.
