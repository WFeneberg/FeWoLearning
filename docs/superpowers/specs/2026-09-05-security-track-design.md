# Security Track — Design

**Date:** 2026-09-05
**Status:** approved
**Track folder:** `security/`

---

## 1. Purpose

A self-contained learning track teaching **application security in C#**, across
the two surfaces the track owner actually ships: **web (ASP.NET Core + Blazor)**
and **Windows desktop (.NET class libraries + WPF)**.

It follows the repo's universal exercise pattern — a stub that fails red before
implementation and passes green once it matches its reference solution — but
deviates from the repo's 100-row / four-difficulty-tier scheme. See §5.

"Beginner" is not a meaningful axis here. A path-traversal guard is not
conceptually harder than a CSP header; they are different *attack surfaces*.
Difficulty therefore rises **within** each surface block rather than across the
track, and the blocks are the organising principle.

Language: all track artifacts (catalog, README, stub header comments, test
names) are written in **English**, matching every other track in this repo.

## 2. Toolchain

### 2.1 Verified on this machine (2026-09-05)

Everything below was measured by building and running real code in a throwaway
scratchpad project, not read from documentation.

- .NET SDK **10.0.400** (10.0.303, 9, 8, 7, 6 also installed).
- **A single test project on `net10.0-windows` with `UseWPF=true` hosts all
  three harnesses at once.** A probe with one `[WpfFact]` (real `PasswordBox`,
  measured and arranged), one bUnit render of a `.razor` component from a
  referenced library, and one ASP.NET Core `TestServer` round-trip asserting a
  response header ran **3 passed / 0 failed**. This is why the track is three
  projects like `wpf/`, not a web pair plus a desktop pair.
- **bUnit 2.9.0 compiles and runs against xunit.v3 4.0.0.** The feared version
  split — `blazor/` runs bUnit on xunit 2.x, `wpf/` runs `Xunit.StaFact` 4.x on
  xunit.v3 4.0.0 — does not force two test projects. One xunit generation covers
  the track.
- `Microsoft.IdentityModel.JsonWebTokens` **8.16.0** resolves and its
  `JsonWebTokenHandler` binds.
- `AesGcm`, `RandomNumberGenerator`, `ProtectedData` and
  `System.Windows.Markup.XamlReader` all bind from the shared framework on
  `net10.0-windows`.

### 2.2 Pinned versions

Content libraries (`exercises/`, `solutions/`) — **kept byte-identical to each
other**, as in `wpf/`:

| Package | Version | Why |
|---|---|---|
| *(FrameworkReference)* `Microsoft.AspNetCore.App` | — | Required, or the Razor source generator cannot resolve `Microsoft.AspNetCore.Components` and every `.razor` fails CS0234 (same trap as `blazor/`). |
| `Microsoft.Data.Sqlite` | 10.0.0 | Row 006 only — see §4.3. |
| `SQLitePCLRaw.lib.e_sqlite3` | **2.1.13** | Explicit pin, see §2.3. |
| `Microsoft.IdentityModel.JsonWebTokens` | 8.16.0 | Rows 017, 018. |

Test project:

| Package | Version |
|---|---|
| `xunit.v3` | 4.0.0 |
| `xunit.runner.visualstudio` | 4.0.0 |
| `Xunit.StaFact` | 4.0.23 |
| `bunit` | 2.9.0 |
| `Microsoft.AspNetCore.TestHost` | 10.0.0 |
| `Microsoft.NET.Test.Sdk` | 17.14.1 |

**Do not add packages for `Microsoft.Extensions.Hosting` or
`System.Security.Cryptography.ProtectedData`.** Both are already in the shared
framework for `net10.0-windows`; referencing them emits `NU1510` on every build
and would break the zero-warnings rule for `solutions/`. (`wpf/` *does*
reference `Microsoft.Extensions.Hosting` explicitly, because it targets the
same TFM but does not carry `Microsoft.AspNetCore.App` — do not copy that line
into this track.)

### 2.3 The vulnerable transitive dependency

`Microsoft.Data.Sqlite` 10.0.0 pulls `SQLitePCLRaw.lib.e_sqlite3` **2.1.11**,
which carries **GHSA-2m69-gcr7-jv3q (high severity)** and makes every build emit
`NU1903`. A security-training track must not ship a known-vulnerable dependency,
and the warning alone would break the zero-warnings rule.

`SQLitePCLRaw.bundle_e_sqlite3` cannot fix it — bundle and lib versions are
decoupled, and the bundle's newest is 3.0.5 while the lib is at 3.53.3. The fix
is an explicit `PackageReference` on **`SQLitePCLRaw.lib.e_sqlite3` 2.1.13**.
Verified: `NU1903` disappears, and SQLite still genuinely executes the injection
row 006 depends on (the classic `or 1=1` payload returned 2 rows where a
parameterised query returns 0). 3.53.3 also works and also clears the warning;
2.1.13 is chosen as the smallest move inside the line `Microsoft.Data.Sqlite`
10.0.0 was built against.

### 2.4 Give `dotnet test` an explicit `--solution` or `--project`

**Corrected twice — 2026-09-05, and again in the final review.** Read this as a
rule to follow if a symptom appears, not as a defect this environment has.

`security/global.json` opts into the `Microsoft.Testing.Platform` runner, whose
`dotnet test` front-end is built around `--project` / `--solution` rather than a
bare, argument-less invocation. Use the explicit form:

| Command | Result |
|---|---|
| `dotnet test --solution FeWoLearning.Security.slnx` | 333 total, 329 failed, 3 passed, 1 skipped, exit 2 |
| `dotnet test --solution FeWoLearning.Security.slnx -p:UseSolutions=true` | 333 total, 0 failed, 332 passed, 1 skipped |
| `dotnet test --project tests/FeWoLearning.Security.Tests.csproj` | identical to the first row |
| `… --filter-class "*Ex001*"` | 5 tests — filtering works |

**The conditional rule:** *if* a bare, argument-less `dotnet test` in this
directory ever reports zero tests (`Es wurden keine Tests ausgeführt`, exit code
5), the fix is the explicit `--solution` / `--project` form above, whatever the
cause. Do not assume it is necessary before you have seen it.

This entry originally recorded that zero-tests result as a measured, reproducible
property of the MTP opt-in. It is **not** established as one. It was observed
once; five separate later invocations — two in Bash and one in PowerShell against
the already-built tree, and one each for `security/` and the pre-existing `wpf/`
track in a throwaway `git worktree` that had never been built at all — every one
of them completed correctly with the full totals, and no party since has
reproduced the failure. Whatever produced the original observation was not a
property of this track or of the runner opt-in as such.

Building and running
`tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe` directly also
works, is how every batch in this track was verified, sidesteps the `dotnet test`
front-end entirely, and remains a useful fallback — but it is not the only way,
and the earlier claim that it was is exactly the error this entry has now been
corrected for twice.

### 2.5 Consequences of the platform choice

The whole track is **Windows-only** and needs an **interactive desktop session**
for the `04-desktop-wpf` block, because WPF is. `net10.0-windows` is the single
TFM for all three projects even though blocks 01–03 would run cross-platform;
splitting the TFM would mean splitting the projects, which §2.1 showed is
unnecessary.

## 3. Project layout

```
security/
  FeWoLearning.Security.slnx
  Directory.Build.props          # ArtifactsPath redirect under UseSolutions
  global.json                    # {"test":{"runner":"Microsoft.Testing.Platform"}}
  README.md
  catalog.md                     # 60-row ledger
  exercises/                     # Microsoft.NET.Sdk.Razor
    _support/
    01-web-aspnet/  02-web-blazor/  03-desktop-core/  04-desktop-wpf/
  solutions/                     # identical csproj, identical namespaces
    _support/
    01-web-aspnet/  02-web-blazor/  03-desktop-core/  04-desktop-wpf/
  tests/                         # Microsoft.NET.Sdk (NOT Razor)
    _harness/
    01-web-aspnet/  02-web-blazor/  03-desktop-core/  04-desktop-wpf/
```

Three projects, `UseSolutions` mechanism identical to `wpf/`, `blazor/`, `uno/`,
`caliburn/` and `avalonia/`: `exercises/` and `solutions/` compile the same type
names into the same namespaces, `tests/` references **exactly one** of them via
the `UseSolutions` MSBuild property. `dotnet build` + test exe is the red run,
`dotnet build -p:UseSolutions=true` + test exe the green run. `solutions/` is
therefore compile-checked on every green run and cannot drift silently — the
deliberate, documented waiver of the repo-wide "solutions are outside the build"
convention.

`Directory.Build.props` redirecting the solutions build through
`UseArtifactsOutput`/`ArtifactsPath` is **required, not cosmetic**: without it
the two content projects share an `obj/` tree and the build fails `CS0579` on
duplicate generated assembly-info attributes. It must live in
`Directory.Build.props`, not in the `.csproj` body, where it is read after the
SDK props import and therefore too late.

The **test project must use `Microsoft.NET.Sdk`, not `Microsoft.NET.Sdk.Razor`**.
It has no `.razor` files of its own; the components under test live in the
content library. (The Razor SDK was tried in the test project while chasing the
zero-tests symptom §2.4 describes and made no difference to it, but the plain SDK
is the correct minimal choice and matches `wpf/`.)

### 3.1 Namespaces

Folder names like `01-web-aspnet` are not valid C# identifiers, so namespaces are
pinned per **block**, not per folder:

| Folder | Namespace |
|---|---|
| `01-web-aspnet/` | `FeWoLearning.Security.Exercises.WebAspNet` |
| `02-web-blazor/` | `FeWoLearning.Security.Exercises.WebBlazor` |
| `03-desktop-core/` | `FeWoLearning.Security.Exercises.DesktopCore` |
| `04-desktop-wpf/` | `FeWoLearning.Security.Exercises.DesktopWpf` |

Blazor components get their namespace from a folder-level `_Imports.razor`
(`@namespace FeWoLearning.Security.Exercises.WebBlazor`), as in `blazor/`. A
Razor component's type name **is its file name**.

Test namespaces mirror with `FeWoLearning.Security.Tests.<Block>`.

### 3.2 `_support/` and `_harness/`

`exercises/_support/` and `solutions/_support/` are **identical** and hold shared
fixtures several exercises depend on: the in-memory SQLite seed, RSA/ECDSA key
generation, a recording logger, and the shared attack-payload corpus. Like
`blazor/_support/`, they are never a TODO and never get a `catalog.md` row.

`tests/_harness/` holds the three harness entry points:

- `WebHarness` — builds an `IHost` with `UseTestServer()` around a pipeline
  configuration supplied by the exercise, and returns an `HttpClient`.
  **`UseTestServer` lives here and only here**; the content library must not
  reference `Microsoft.AspNetCore.TestHost`, or the exercise could host itself
  and the separation of "learner configures / harness drives" collapses.
- `BlazorHarness` — thin wrapper over `BunitContext` with the services the block
  needs (auth state, navigation).
- WPF uses `[WpfFact]` from `Xunit.StaFact` directly, plus a `Pump()` helper
  borrowed in spirit from `wpf/`.

Plus `HarnessSmokeTests` — three facts, one per harness, which must stay green on
the untouched stub tree. They are the canary for a package bump breaking a
harness, exactly as `uno/`'s `HarnessSmokeTests` is. They are the **only** tests
green on a red run.

Tests are serialised with `[assembly: Parallelization(Mode = ParallelMode.None)]`,
as in `wpf/` — `CollectionBehavior(DisableTestParallelization = true)` is
`Obsolete(error: true)` in xunit.v3 4.0.0 and does not compile. Serialisation is
not optional here: rows 055 (clipboard) and 050 (named pipes) touch
process-global and machine-global state.

### 3.3 The `CS0104` trap

bUnit 2.9 still ships an obsolete `Bunit.TestContext`, which collides with
xunit.v3's `Xunit.TestContext`. Any test file that has `using Bunit;` and also
touches `TestContext.Current.CancellationToken` fails **`CS0104`**. Measured, not
predicted. Every affected test file needs:

```csharp
using TestContext = Xunit.TestContext;
```

This is worth a prominent README entry: `blazor/` never hit it because it runs
xunit 2.x, which has no `TestContext`.

## 4. The exercise shape

### 4.1 Attack facts and use facts

Each exercise is vulnerable or missing code the learner hardens. Its test class
carries **two kinds of facts**:

- **Attack facts** — the exploit must fail. Traversal rejected, forged token
  refused, untrusted markup not rendered as markup.
- **Use facts** — the legitimate path must still work. The real file is served,
  the valid token is accepted, safe markup still renders.

Stubs throw `NotImplementedException`, so both kinds go red; the solution turns
both green. This preserves the repo invariant.

### 4.2 The rule this track lives or dies by

> **An attack fact alone is worthless. Every attack fact must be paired with a
> use fact.**

A test that only asserts "the attack was rejected" is satisfied by a stub — or a
lazy solution — that rejects *everything*. A path validator returning a constant
`false` passes every traversal payload ever written. A sanitiser that returns the
empty string defeats every XSS vector. Without the paired use fact, such an
exercise grades nothing.

This is the security-track member of the family of "tests that lie" that this
repo records per track: `avalonia/`'s rendered-geometry-cannot-prove-mechanism,
`wpf/`'s CLR-wrapper-cannot-prove-the-property-system, `kotlin/`'s
passes-without-`supervisorScope`.

Three further traps, all to be written into `security/README.md`:

1. **Do not test crypto against hard-coded expected values.** It teaches
   copy-paste and breaks on any legitimate parameter change. Assert *properties*
   instead: a different salt yields a different hash, `Verify` round-trips its own
   `Hash`, a single flipped ciphertext byte is detected.
2. **Never assert wall-clock timing.** A constant-time-comparison exercise must
   assert the mechanism and its behaviour, never that one comparison took as long
   as another — that test is flaky by construction. Same stance `wpf/` takes on
   its performance rows.
3. **`Assert.Throws` on a stub that already throws is a false green.** The repo
   already records this; it bites harder here, because so many security
   behaviours are naturally expressed as "this must be rejected". Assert the
   *rejection outcome* (status code, returned `false`, unchanged state), not that
   *an* exception occurred — and where an exception genuinely is the contract,
   assert a locally defined exception type that `NotImplementedException` cannot
   satisfy.

### 4.3 Row 006 uses real SQLite deliberately

SQL injection cannot be honestly demonstrated against a string-comparing fake: a
test asserting "the command text contains a parameter placeholder" is satisfied
by a solution that builds the right-looking string and still concatenates
elsewhere. Row 006 therefore runs `Microsoft.Data.Sqlite` in-memory, seeds two
rows, and its attack fact asserts the classic tautology payload returns **zero**
rows. Verified in the probe that the vulnerable form really does return 2. Its
paired use fact asserts the legitimate lookup still finds its row.

This is the only row needing a database, and the only reason the SQLite packages
are in the content libraries at all.

## 5. Catalog structure — the deviation from the repo scheme

Every other track has 100 rows in four difficulty tiers (`01-beginner` 001–035,
`02-intermediate` 036–070, `03-advanced` 071–090, `04-expert` 091–100). This
track has **60 rows in four surface blocks**:

| Block folder | Rows | Harness |
|---|---|---|
| `01-web-aspnet` | 001–024 | `TestServer` |
| `02-web-blazor` | 025–036 | bUnit |
| `03-desktop-core` | 037–052 | plain xunit |
| `04-desktop-wpf` | 053–060 | `[WpfFact]` |

Chosen by the track owner over both a 50-row cut and a difficulty-tier layout.
Difficulty rises inside each block. `catalog.md` keeps the repo's column shape
(`# | Slug | Concepts | Status`) and its `**Status: N ✅ / M ⬜**` line, so the
existing workflow in CLAUDE.md ("read the catalog, take the next five ⬜ rows")
applies unchanged.

`docs/exercise-format.md` and `CLAUDE.md` must both be updated to record this
deviation, or the next person to read them will assume 100 rows and four
difficulty tiers.

### 5.1 Block 01 — `web-aspnet` (001–024)

| # | Slug |
|---|---|
| 001 | SecurityHeaders |
| 002 | HttpsRedirectAndHsts |
| 003 | ContentSecurityPolicy |
| 004 | PathTraversalGuard |
| 005 | ModelBindingOverposting |
| 006 | SqlInjectionParameterization |
| 007 | ContextualOutputEncoding |
| 008 | AntiforgeryCsrf |
| 009 | CorsPolicy |
| 010 | CookieSecurityFlags |
| 011 | SessionFixation |
| 012 | PasswordHashingPbkdf2 |
| 013 | AuthenticationHandler |
| 014 | AuthorizationPolicies |
| 015 | ResourceBasedAuthorization |
| 016 | InsecureDirectObjectReference |
| 017 | JwtValidation |
| 018 | RefreshTokenRotation |
| 019 | RateLimiting |
| 020 | JsonDepthAndUnknownMembers |
| 021 | SsrfOutboundGuard |
| 022 | OpenRedirectGuard |
| 023 | FileUploadValidation |
| 024 | ErrorHandlingWithoutLeakage |

Row 019 asserts that the limiter *rejected* the surplus request (429), never how
fast — per §4.2 trap 2.

### 5.2 Block 02 — `web-blazor` (025–036)

| # | Slug |
|---|---|
| 025 | MarkupStringXss |
| 026 | RenderTreeEncodingDefaults |
| 027 | CspNonceFlow |
| 028 | AuthorizeViewAndAuthState |
| 029 | ClientAuthIsNotEnforcement |
| 030 | AntiforgeryInEditForm |
| 031 | SecretsNeverReachClient |
| 032 | JsInteropInjection |
| 033 | NavigationManagerOpenRedirect |
| 034 | PersistentStateLeak |
| 035 | ErrorBoundaryLeakage |
| 036 | SanitizingComponent |

Row 029 is the block's centrepiece: `AuthorizeView` hides the button, and the
test then calls the service **directly, bypassing the UI**, to prove the server
side still refuses. A solution that only hides the button fails.

Row 034 depends on `AddBunitPersistentComponentState()`; `blazor/README.md`
already records that `PersistentComponentState` is not in bUnit's default
services and that hand-building it out of `ComponentStatePersistenceManager` is
~40 lines of fixture for nothing. Reuse that finding.

### 5.3 Block 03 — `desktop-core` (037–052)

| # | Slug |
|---|---|
| 037 | DpapiProtectedData |
| 038 | CredentialStorage |
| 039 | AesGcmAuthenticatedEncryption |
| 040 | KeyDerivationAndRotation |
| 041 | FixedTimeComparison |
| 042 | CryptographicRandomness |
| 043 | SignatureVerification |
| 044 | UpdateIntegrityAndRollback |
| 045 | UnsafeDeserialization |
| 046 | XmlExternalEntity |
| 047 | ZipSlipExtraction |
| 048 | PathCanonicalization |
| 049 | ProcessArgumentInjection |
| 050 | NamedPipeAccessControl |
| 051 | SecretRedactionInLogs |
| 052 | RestrictiveFileAcl |

Row 042's use fact must not be "the token is random" (untestable in one draw);
it asserts distribution-independent properties instead — correct length, no
repeat across many draws, and that a seeded `System.Random` cannot reproduce the
output.

Rows 049, 050 and 052 touch real OS objects. They create everything they need
under a per-test temp directory and delete it, and 050 uses a per-test unique
pipe name so the serialised suite cannot collide with a leftover.

### 5.4 Block 04 — `desktop-wpf` (053–060)

| # | Slug |
|---|---|
| 053 | PasswordBoxNoPlaintextBinding |
| 054 | SensitiveBufferLifetime |
| 055 | ClipboardHygiene |
| 056 | DragDropUntrustedPayload |
| 057 | EmbeddedBrowserNavigationPolicy |
| 058 | XamlReaderUntrustedMarkup |
| 059 | BindingErrorLeakage |
| 060 | FilePickerResultStillUntrusted |

Row 055 touches the real system clipboard, which is process-global and shared
with the developer's own session. It saves and restores prior content, and the
README warns that running the suite will briefly disturb the clipboard.

## 6. Deliberate exclusions

Recorded so they are visible decisions, not oversights:

- **WebView2 is not referenced.** Row 057 tests the navigation-policy decision
  surface as a standalone class. The WebView2 runtime cannot be meaningfully
  exercised headless, and an exercise that only pretends to test it would be
  worse than none.
- **Not covered by these 60:** supply-chain / NuGet package-signature
  verification, threat-modelling artefacts, the full OAuth 2.0 / OIDC
  authorization-code-with-PKCE flow (rows 017–018 cover token *validation* and
  rotation, not the flow), Windows service and privilege separation, and
  Kerberos/NTLM. Any of these would push the track past 60 rows, which the owner
  has pre-authorised should it prove necessary.

## 7. Testing strategy

Per batch of five exercises, matching the repo workflow in CLAUDE.md:

1. **Red check** — `dotnet build`, then run the test exe filtered to the five.
   Every fact *of those five* must fail, and each failure must trace to that
   exercise's `NotImplementedException`, not to a compile or fixture error. A
   stub that fails to build is a bug. The three `HarnessSmokeTests` are the sole
   deliberate exception to "nothing is green on a red run" (§3.2) and are
   excluded by the filter anyway.
2. **Green check** — `dotnet build -p:UseSolutions=true`, then the same filtered
   run. No overlay copying is needed; that is what the `UseSolutions` mechanism
   buys.
3. **Pairing audit** — for each of the five, confirm §4.2: does every attack fact
   have a use fact, and would a reject-everything implementation fail?
4. Flip exactly those five `catalog.md` rows and update its `**Status:**` line.
5. Commit as `security: exNNN–exNNN`, staging explicit paths.

The full 60-row suite runs once per completed block.

Stubs come in the same two shapes `blazor/` uses. **Shape A** throws from the
member the test calls, and is the default. **Shape B** declares a field or event
the learner must wire up and throws from a lifecycle method or handler instead —
needed wherever a `throw` cannot sit at the call site (Razor markup, an event
declaration). Shape B is why `exercises/` is expected to emit
`CS0169`/`CS0414`/`CS0649` warnings for those never-assigned members; these stay
unsuppressed, so the learner sees what is left to wire. **`solutions/` must build
with zero warnings**, the same stance `blazor/` and `caliburn/` take — and here
that rule is what forces the §2.2 and §2.3 package decisions.

## 8. Build order

1. Scaffolding: `.slnx`, three `.csproj`, `Directory.Build.props`, `global.json`,
   `_Imports.razor`, empty `catalog.md` and `README.md`.
2. `tests/_harness/` plus `HarnessSmokeTests` — three facts, one per harness,
   green on an otherwise empty tree. Nothing else starts until this is green.
3. `catalog.md` seeded with all 60 rows ⬜.
4. Exercises in batches of five, block by block, 001 → 060.
5. `README.md` written as findings accumulate, not at the end.
6. `CLAUDE.md` and `docs/exercise-format.md` updated for the 60-row / four-block
   deviation and for the `security/` commands.
