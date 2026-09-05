# Security Track

60 graded C# exercises in application security, across the two surfaces the
track owner actually ships: **web** (ASP.NET Core + Blazor) and **Windows
desktop** (.NET class libraries + WPF). All 60 are written and verified — see
`catalog.md` for the row-by-row ledger.

This track deliberately departs from the repo's usual 100-row / four-
difficulty-tier scheme (see "Catalog structure" below), and it deviates from
the repo-wide "`solutions/` stays out of the build" convention the same way
`avalonia/`, `blazor/`, `uno/`, `caliburn/` and `wpf/` do: `exercises/` and
`solutions/` compile the same type names into the same namespaces, and
`tests/` references **exactly one** of them via the `UseSolutions` MSBuild
property, so reference solutions are compile-checked on every green run and
cannot drift silently.

## Setup and commands

**Windows-only.** Block `04-desktop-wpf` additionally needs an **interactive
desktop session** — it opens real WPF elements — because WPF is; it will not
run headless, as a service, or in a session-0/RDP-disconnected context.

Run every command **from inside `security/`**, not the repo root.

`security/global.json` opts the test project into the
**`Microsoft.Testing.Platform`** runner
(`{"test":{"runner":"Microsoft.Testing.Platform"}}`), whose `dotnet test`
front-end is built around `--project`/`--solution` rather than a bare,
argument-less invocation. **Give it an explicit target:**

| Run | Command |
|---|---|
| Stubs (red) | `dotnet test --solution FeWoLearning.Security.slnx` |
| Solutions (green) | `dotnet test --solution FeWoLearning.Security.slnx -p:UseSolutions=true` |
| One exercise | `dotnet test --project tests/FeWoLearning.Security.Tests.csproj --filter-class "*Ex001*"` |

All three verified directly on this machine. The stub run reports **Total:
333, Failed: 329, Passed: 3, Skipped: 1**, exit code 2 (nonzero because
facts genuinely fail — that is the expected red picture, not a discovery
failure); the solutions run reports **Total: 333, Failed: 0, Passed: 332,
Skipped: 1**, with 0 build warnings on both builds (the 3 passing on the
stub run are the harness canaries — see "Per-row warnings" for the skip).
`--filter-class "*Ex001*"` narrows correctly to that exercise's 5 facts.
Per block: `01-web-aspnet` 131 facts, `02-web-blazor` 58,
`03-desktop-core` 104, `04-desktop-wpf` 37 (one of which is the skip), plus
the 3 harness canaries — 131+58+104+37+3 = 333.

A caveat this session tried hard to reproduce and could not: the design spec
and an earlier draft of this README claimed that a **bare, argument-less**
`dotnet test` in this directory reports "Es wurden keine Tests ausgeführt"
("no tests were run") with exit code 5, on the theory that MTP's front-end
mis-resolves a directory holding a `.slnx` to the already-built test DLL
rather than to the solution. Retested here five times — twice in Bash and
once in PowerShell against the already-built tree, and once more each for
`security/` and the pre-existing `wpf/` track in a throwaway `git worktree`
that had never been built at all — and every one of those five bare
invocations completed correctly with the full totals above (240 total / 235
failed / 5 passed for `wpf/`), never the exit-5/zero-tests failure. Whatever
produced the original observation was not reproducible in this environment
just now. If a bare `dotnet test` does report zero tests on your machine, the
fix is the explicit `--project`/`--solution` form above regardless of cause —
but do not assume it is necessary here without checking; it may not be.

The build-then-run-the-executable recipe below is still a valid fallback —
it is how every batch in this track was actually verified, it sidesteps
`dotnet test`'s front-end entirely, and it is what the `-filter` syntax
further down applies to:

| Step | Command |
|---|---|
| Build stubs (red) | `dotnet build` |
| Run stub tests | `tests\bin\Debug\net10.0-windows\FeWoLearning.Security.Tests.exe` |
| Build solutions (green) | `dotnet build -p:UseSolutions=true` |
| Run solution tests | `artifacts-solutions\bin\FeWoLearning.Security.Tests\debug\FeWoLearning.Security.Tests.exe` |

**Build output is German-locale on this machine** ("0 Warnung(en)", not
"0 Warning(s)"; "Es wurden keine Tests ausgeführt" rather than "no tests were
run"). A scripted check that greps for the English string silently passes
(or silently fails to catch) regardless of the real result — read the
numeric totals, not the words around them.

### Filter syntax

`dotnet test` itself takes `--filter-class "*Ex001*"` (verified above) to
scope to one exercise. The **built executable**, used with the fallback
recipe, has a separate, unrelated filter syntax: its `-filter` takes the
form `/Assembly/Namespace/Class/Method`, with wildcards allowed only at the
start and/or end of a segment — not in the middle — and a `|`-joined single
filter is rejected outright by xunit.v3 ("wildcards may only be at the
beginning and/or end"). **A union of several things needs repeated
`-filter` flags**, not one combined pattern.

One exercise:

```
tests\bin\Debug\net10.0-windows\FeWoLearning.Security.Tests.exe -filter "/*/*/Ex001*"
```

A whole block:

```
tests\bin\Debug\net10.0-windows\FeWoLearning.Security.Tests.exe -filter "/*/FeWoLearning.Security.Tests.WebAspNet/*"
```

A batch of five (repeated flags, one per exercise):

```
tests\bin\Debug\net10.0-windows\FeWoLearning.Security.Tests.exe -filter "/*/*/Ex001*" -filter "/*/*/Ex002*" -filter "/*/*/Ex003*" -filter "/*/*/Ex004*" -filter "/*/*/Ex005*"
```

Same flags against the solutions executable path above for the green run.

## Layout

```
security/
  FeWoLearning.Security.slnx
  Directory.Build.props          # ArtifactsPath redirect under UseSolutions — required, not cosmetic
  global.json                    # {"test":{"runner":"Microsoft.Testing.Platform"}}
  catalog.md                     # 60-row ledger, four attack-surface blocks
  exercises/                     # Microsoft.NET.Sdk.Razor; _support/ + four block folders
  solutions/                     # identical csproj, identical namespaces, identical _support/
  tests/                         # Microsoft.NET.Sdk (NOT Razor); _harness/ + four block folders
```

Namespaces are pinned per block, not per folder, because a folder name like
`01-web-aspnet` is not a valid C# identifier:

| Folder | Namespace |
|---|---|
| `01-web-aspnet/` | `FeWoLearning.Security.Exercises.WebAspNet` |
| `02-web-blazor/` | `FeWoLearning.Security.Exercises.WebBlazor` |
| `03-desktop-core/` | `FeWoLearning.Security.Exercises.DesktopCore` |
| `04-desktop-wpf/` | `FeWoLearning.Security.Exercises.DesktopWpf` |

Test namespaces mirror as `FeWoLearning.Security.Tests.<Block>`. `_support/`
(identical in `exercises/` and `solutions/`) holds shared fixtures — SQLite
seed, RSA/ECDSA key generation, a recording logger, the shared attack-payload
corpus — several exercises' tests depend on; it never carries a TODO and
never gets a `catalog.md` row. `tests/_harness/` holds the three harness entry
points (`WebHarness` over `TestServer`, `BlazorHarness` over `BunitContext`,
and `[WpfFact]` from `Xunit.StaFact` directly) plus `HarnessSmokeTests` — three
facts, one per harness, the only tests green on the untouched stub tree.

## How a security test lies

The rule this track lives or dies by:

> **An attack fact alone is worthless. Every attack fact must be paired with
> a use fact.**

1. **An attack fact with no use fact grades nothing.** A reject-everything
   implementation passes it for free. `Ex004_PathTraversalGuard`'s validator
   returning a constant `false` passes every traversal payload ever written —
   only a paired use fact ("the legitimate file is still served") catches
   that degenerate. Every batch in this track was checked by actually
   building reject-everything variants of its stubs and confirming the use
   facts, and only the use facts, failed against them; do the same before
   trusting a new exercise.

   **The next probe past that one is *wrong-but-implemented*** — an earnest
   implementation that does real work but picks the wrong mechanism. It is
   not degenerate, so the reject-everything probe never finds it, and it
   passes for the same reason a hard-coded digest passes: the facts assert an
   outcome that more than one mechanism produces. This is the failure mode
   the final whole-work review caught in **four** exercises that every
   per-batch review had already passed — `Ex007` (swap two of the four sinks'
   encoders for each other: **6/6 green**, because "something was escaped" is
   all the attack facts ever asked), `Ex023` (drop the magic-byte check and
   keep only the extension allowlist: **6/6 green**, because the disguised
   `report.pdf` was rejected at the extension gate before a byte was read),
   `Ex025` (a denylist instead of the allowlist `Ex036` teaches: **green on
   every payload**) and `Ex041` (plain `string.Equals`: **5/5 green**, and
   that one is not fixable — see trap 3). So after the reject-everything
   variant, build the **plausible wrong** one and run the facts against it.
   The repair is almost always the same shape: assert a property only the
   right mechanism has — a round-trip through the decoder that sink's real
   consumer would use, a use fact that forces the attacker's own case onto
   the happy path so the attack fact can no longer be satisfied by rejecting
   it, or an assertion on the parsed DOM rather than on known-bad substrings.

2. **A hard-coded crypto digest tests transcription, not behaviour**, and
   breaks the moment a legitimate parameter changes (a different salt, a
   different iteration count). Assert properties instead: a different salt
   yields a different hash, `Verify` round-trips its own `Hash`, a single
   flipped ciphertext byte is detected.

3. **A wall-clock timing assertion is flaky by construction**, so
   `Ex041_FixedTimeComparison` is graded **purely behaviourally** — never on
   elapsed time. Be honest about what that costs. Its five facts pin the
   *outcomes* of the comparison (a prefix does not match; a
   last-character-only difference does not match; identical matches; case
   matters; empty against empty matches) and nothing more, and
   `string.Equals(presented, expected, StringComparison.Ordinal)` satisfies
   all five — **measured**, by building exactly that and running them. The
   mechanism the exercise is actually about, hashing both sides to a fixed
   length and comparing with `CryptographicOperations.FixedTimeEquals`, is
   **not machine-checkable in this harness**: proving *which* comparison ran
   would need either a timing assertion (the flaky thing this trap exists to
   forbid) or reflection over IL, which is out of style everywhere in this
   repo. The requirement therefore lives in **the stub header**, as prose the
   learner reads, and the tests grade the behaviour that requirement implies.
   Do not add a timing assertion here to close the gap — it would not close
   it, and it would trade a documented limitation for a flaky test.

4. **`Assert.Throws` on a stub that already throws is a false green.** Every
   stub throws `NotImplementedException`, so a test asserting only "an
   exception was thrown" passes before the implementation exists. Assert the
   rejection *outcome* (a returned `false`, an unchanged state, a specific
   status code), or a locally defined exception type the stub's
   `NotImplementedException` cannot satisfy. This bites harder here than
   elsewhere in the repo, because so many security behaviours are naturally
   phrased as "this must be rejected". The sharper version this track found:
   xunit's `Assert.Throws<T>` requires an **exact** type match, not a base
   type — `AesGcm.Decrypt` throws `AuthenticationTagMismatchException`, a
   `CryptographicException` *subtype*, so a test asserting the base
   `CryptographicException` fails against real, correct .NET code for the
   wrong reason. Assert the subtype the platform actually throws.

## Toolchain traps

Every one of these was measured by building and running real code, not read
from documentation.

- **bUnit 2.9 still ships an obsolete `Bunit.TestContext`**, which collides
  with xunit.v3's `Xunit.TestContext` (`CS0104`) the moment a test file has
  `using Bunit;` and also touches `TestContext.Current.CancellationToken`.
  Fix: `using TestContext = Xunit.TestContext;`. `blazor/` never hits this
  because it runs xunit 2.x, which has no `TestContext` at all.
- **`Microsoft.Data.Sqlite` 10.0.0 drags in `SQLitePCLRaw.lib.e_sqlite3`
  2.1.11**, which carries **GHSA-2m69-gcr7-jv3q (high severity)** and emits
  `NU1903` on every build. `SQLitePCLRaw.bundle_e_sqlite3` cannot fix it —
  bundle and lib versions are decoupled. Pinned instead to
  **`SQLitePCLRaw.lib.e_sqlite3` 2.1.13**, the smallest move that clears the
  advisory inside the line `Microsoft.Data.Sqlite` 10.0.0 was built against.
- **Do not reference `Microsoft.Extensions.Hosting` or
  `System.Security.Cryptography.ProtectedData`.** Both are already in the
  shared framework for `net10.0-windows` here; referencing either emits
  `NU1510` on every build. (`wpf/` *does* reference `Microsoft.Extensions.Hosting`
  explicitly, because it targets the same TFM but does not carry
  `Microsoft.AspNetCore.App` — do not copy that line into this track.)
- **`Directory.Build.props` redirecting the solutions build's output via
  `UseArtifactsOutput`/`ArtifactsPath` is required, not cosmetic.** Without it
  the two content projects share an `obj/` tree and the build fails
  `CS0579` on duplicate generated assembly-info attributes.
- **The test project uses `Microsoft.NET.Sdk`, not the Razor SDK.** It has no
  `.razor` files of its own — the components under test live in the content
  library.
- **`ImplicitUsings` here is the minimal set**: `System`,
  `System.Collections.Generic`, `System.Linq`, `System.Threading`,
  `System.Threading.Tasks`, `Xunit`, and nothing else — no `System.IO`, no
  `System.Net`, no `System.Net.Http`. A `CS0246` in a test file is a missing
  `using`, not a missing package.
- **`[assembly: Parallelization(Mode = ParallelMode.None)]` needs
  `using Xunit.Sdk; using Xunit.v3;`** — those types are not in the bare
  `Xunit` namespace.
- **bUnit's `BunitContext` pre-registers a `PlaceholderAuthorizationService`
  that throws**, and `AddAuthorizationCore()` cannot displace it because the
  registration is `TryAdd`-based. `BlazorHarness` registers
  `AddSingleton<IAuthorizationService, DefaultAuthorizationService>()`
  explicitly for exactly this reason; any row using `AuthorizeView` or
  `[Authorize]` needs it.
- **`ECDsa.VerifyData` never throws for malformed signature bytes**, in
  either `IeeeP1363` or `Rfc3279DerSequence` format — 36 distinct malformed
  inputs were tried across both formats and none threw. `Ex043` is scoped
  around recognising this: the exercise teaches noticing when a platform
  primitive already provides a safety property, rather than adding a guard
  that would be dead code.
- **The harness's own WPF canary was once tautological.** `ActualWidth > 0`
  after `Arrange` holds regardless of whether a control template resolved,
  because `FrameworkElement` defaults to `HorizontalAlignment.Stretch` and
  fills whatever rect it is given. It now asserts `Template != null` plus
  `DesiredSize > 0` measured **before** `Arrange`. Do not reintroduce the old
  form.

## Per-row warnings

- **`Ex055_ClipboardHygiene` touches the real system clipboard**, which is
  process-global and shared with the developer's own session. The test saves
  prior clipboard content in its constructor and restores it in `Dispose`,
  with a bounded 10-attempt retry against `COMException` — but **running the
  suite will briefly disturb the clipboard** while it runs. The graded
  contract is the triple of registered clipboard formats Microsoft documents
  for this, in the values the documentation gives them:
  `CanIncludeInClipboardHistory` = **false**, `CanUploadToCloudClipboard` =
  **false**, `ExcludeClipboardContentFromMonitorProcessing` = **true**. Read
  the names literally — the two `Can…` formats grant a permission, so denying
  it is `false`, while `Exclude…` asserts an exclusion, so requesting it is
  `true`. An earlier version of this row asserted `false` for the `Exclude…`
  format and omitted `CanUploadToCloudClipboard` altogether, which both
  failed the learner who wrote the documented answer and left the stub
  header's cloud-sync claim graded by nothing at all.
- **`Ex060_FilePickerResultStillUntrusted` has the track's only skipped
  test.** Its symbolic-link attack fact needs elevation or Windows Developer
  Mode for `File.CreateSymbolicLink`; without either, the fact is dynamically
  skipped via `Assert.Skip` with a reason, and shows as `Skipped: 1` in every
  run on this machine. This fails safe rather than false-passing: the
  property left ungraded on an unprivileged run is specifically "a path that
  looks like it's inside the root but resolves outside it" — the other four
  facts in the exercise still run and grade normally.
- **`Ex050` and `Ex052` create real OS objects** (a named pipe, an ACL'd
  file) under per-test temp directories, cleaned up afterwards. `Ex050` uses
  a unique pipe name per test so the serialised suite cannot collide with a
  leftover. **`Ex049` starts no process and creates no OS object at all**:
  `BuildStartInfo` only builds a `ProcessStartInfo` and hands it back for
  inspection, and `Process.Start` appears nowhere in this track.
- **`Ex047_ZipSlipExtraction`'s tests prove containment by watching a
  directory** — listing the extraction root's parent before and after each
  extraction and asserting nothing new appeared, which catches any escape
  path the implementation resolves to rather than only one hand-picked one.
  That parent is a per-test sandbox (`%TEMP%/fewo-sec-<guid>/`) with the
  extraction root one level down (`.../root/`), never `%TEMP%` itself: a
  watcher pointed at `%TEMP%` attributes any file another process happens to
  create in that window to the code under test, and an earlier version of
  this test then deleted it. Cleanup is now by removing the sandbox whole in
  `Dispose`, so the test can never touch a file it did not create. Keep both
  properties if you touch this exercise.
- **`Ex053_PasswordBoxNoPlaintextBinding` is graded by reflection** over the
  view model's public properties and fields, not by observing rendered
  output.
- **`Ex057_EmbeddedBrowserNavigationPolicy` deliberately does not reference
  WebView2.** The runtime cannot be meaningfully exercised headless, and an
  exercise that only pretended to test it would be worse than none; it tests
  the navigation-policy decision surface as a standalone class instead.
- **`Ex006_SqlInjectionParameterization` is the only row needing a
  database.** It runs real in-memory SQLite rather than inspecting command
  text, because a test that merely checks for a parameter placeholder in the
  command string is satisfied by a solution that builds a right-looking
  string and concatenates the attacker input in some other query entirely.

## Catalog structure

Every other track in this repo has 100 rows across four **difficulty tiers**.
This track has **60 rows across four attack-surface blocks** instead, because
"beginner" is not a meaningful axis for security: a path-traversal guard is
not conceptually harder than a CSP header, they are different attack
surfaces entirely. Difficulty rises **within** each block, not across the
track.

| Block folder | Rows | Harness |
|---|---|---|
| `01-web-aspnet` | 001–024 | `TestServer` |
| `02-web-blazor` | 025–036 | bUnit |
| `03-desktop-core` | 037–052 | plain xunit |
| `04-desktop-wpf` | 053–060 | `[WpfFact]` |

`catalog.md` keeps the repo's usual column shape (`# | Slug | Concepts |
Status`) and its `**Status: N ✅ / M ⬜**` line, so the existing "read the
catalog, take the next five ⬜ rows" workflow from the repo's `CLAUDE.md`
still applies — only the row count and the axis the rows are organised by
differ.

## Deliberate exclusions

Recorded so they read as decisions, not oversights: supply-chain / NuGet
package-signature verification, threat-modelling artefacts, the full OAuth
2.0 / OIDC authorization-code-with-PKCE flow (rows 017–018 cover token
*validation* and rotation, not the flow itself), Windows service and
privilege separation, and Kerberos/NTLM. None of the 60 rows attempt these.
