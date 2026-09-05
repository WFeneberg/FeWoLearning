# Security (C#) — Exercise Catalog (60)

Attack-surface blocks: **web-aspnet** 001–024 · **web-blazor** 025–036 ·
**desktop-core** 037–052 · **desktop-wpf** 053–060.

Legend: ✅ seeded (stub + test + solution present, red and green both verified) ·
⬜ planned.

This track deliberately departs from the repo's 100-row / four-difficulty-tier
scheme. "Beginner" is not a meaningful axis for security: a path-traversal guard
is not conceptually harder than a CSP header, they are different attack surfaces.
Difficulty rises *within* each block. See
`docs/superpowers/specs/2026-09-05-security-track-design.md` §5.

Stubs live in `exercises/<block>/ExNNN_<Slug>.cs` (or `.razor` for block 02),
their xUnit tests in `tests/<block>/ExNNN_<Slug>Tests.cs`, and reference
implementations in `solutions/<block>/` at the same relative path.

**Status: 15 ✅ / 45 ⬜**

## web-aspnet (001–024) — the server-side attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | SecurityHeaders | middleware pipeline, Response.OnStarting, header lifetime | ✅ |
| 002 | HttpsRedirectAndHsts | HSTS, transport downgrade, redirect status codes | ✅ |
| 003 | ContentSecurityPolicy | CSP directives, per-request nonce, inline-script blocking | ✅ |
| 004 | PathTraversalGuard | canonicalisation, root containment, safe static file serving | ✅ |
| 005 | ModelBindingOverposting | mass assignment, BindNever, explicit DTO projection | ✅ |
| 006 | SqlInjectionParameterization | parameterised commands, real SQLite, tautology payloads | ✅ |
| 007 | ContextualOutputEncoding | HtmlEncoder vs JavaScriptEncoder vs UrlEncoder, sink context | ✅ |
| 008 | AntiforgeryCsrf | antiforgery tokens, cross-origin POST, safe vs unsafe methods | ✅ |
| 009 | CorsPolicy | origin allowlists, credentials, why wildcard plus credentials fails | ✅ |
| 010 | CookieSecurityFlags | HttpOnly, Secure, SameSite, cookie scope | ✅ |
| 011 | SessionFixation | identifier regeneration on privilege change | ✅ |
| 012 | PasswordHashingPbkdf2 | Rfc2898DeriveBytes, per-user salt, iteration count, fixed-time verify | ✅ |
| 013 | AuthenticationHandler | AuthenticationHandler, ClaimsPrincipal construction, scheme selection | ✅ |
| 014 | AuthorizationPolicies | policy-based authorization, requirements, handler registration | ✅ |
| 015 | ResourceBasedAuthorization | IAuthorizationService on a resource instance, ownership checks | ✅ |
| 016 | InsecureDirectObjectReference | ownership enforcement, opaque identifiers, enumeration | ⬜ |
| 017 | JwtValidation | issuer, audience, lifetime and signature validation, alg confusion | ⬜ |
| 018 | RefreshTokenRotation | single-use refresh tokens, reuse detection, family revocation | ⬜ |
| 019 | RateLimiting | rate limiter partitions, 429 responses, per-principal keys | ⬜ |
| 020 | JsonDepthAndUnknownMembers | MaxDepth, unmapped member handling, deserialisation resource limits | ⬜ |
| 021 | SsrfOutboundGuard | outbound URL validation, scheme allowlists, private address ranges | ⬜ |
| 022 | OpenRedirectGuard | local-redirect checks, absolute URL rejection, return-URL allowlists | ⬜ |
| 023 | FileUploadValidation | content sniffing, extension allowlists, size limits, safe storage names | ⬜ |
| 024 | ErrorHandlingWithoutLeakage | ProblemDetails, exception middleware, suppressing internals | ⬜ |

## web-blazor (025–036) — the component attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 025 | MarkupStringXss | MarkupString as a sink, sanitisation, when raw HTML is never safe | ⬜ |
| 026 | RenderTreeEncodingDefaults | automatic encoding of text and attributes, attribute injection | ⬜ |
| 027 | CspNonceFlow | nonce propagation to components, eliminating inline handlers | ⬜ |
| 028 | AuthorizeViewAndAuthState | AuthenticationStateProvider, AuthorizeView, cascading auth state | ⬜ |
| 029 | ClientAuthIsNotEnforcement | UI trimming is not authorization, server-side enforcement | ⬜ |
| 030 | AntiforgeryInEditForm | EditForm, antiforgery in interactive and static rendering | ⬜ |
| 031 | SecretsNeverReachClient | configuration surface, what a component may receive | ⬜ |
| 032 | JsInteropInjection | passing untrusted data across JS interop, avoiding eval-shaped calls | ⬜ |
| 033 | NavigationManagerOpenRedirect | client-side redirect validation, external URI rejection | ⬜ |
| 034 | PersistentStateLeak | PersistentComponentState, what must never survive prerendering | ⬜ |
| 035 | ErrorBoundaryLeakage | ErrorBoundary, suppressing exception detail in the render tree | ⬜ |
| 036 | SanitizingComponent | reusable sanitising component, allowlist over denylist | ⬜ |

## desktop-core (037–052) — the local attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 037 | DpapiProtectedData | ProtectedData, DataProtectionScope, optional entropy | ⬜ |
| 038 | CredentialStorage | never plaintext at rest, round-tripping, scope of protection | ⬜ |
| 039 | AesGcmAuthenticatedEncryption | AES-GCM, nonce uniqueness, tag verification, tamper detection | ⬜ |
| 040 | KeyDerivationAndRotation | key derivation, versioned key material, decrypting older versions | ⬜ |
| 041 | FixedTimeComparison | CryptographicOperations.FixedTimeEquals, why length-first exits leak | ⬜ |
| 042 | CryptographicRandomness | RandomNumberGenerator over System.Random, token generation | ⬜ |
| 043 | SignatureVerification | detached signatures, public-key verification, rejecting tampered data | ⬜ |
| 044 | UpdateIntegrityAndRollback | hash manifests, signed manifests, monotonic version enforcement | ⬜ |
| 045 | UnsafeDeserialization | polymorphic type handling, type allowlists, rejecting arbitrary types | ⬜ |
| 046 | XmlExternalEntity | XmlReaderSettings, DtdProcessing, XmlResolver, entity expansion | ⬜ |
| 047 | ZipSlipExtraction | archive entry path containment, absolute and relative escapes | ⬜ |
| 048 | PathCanonicalization | full-path containment, UNC and device-name traps, alternate streams | ⬜ |
| 049 | ProcessArgumentInjection | ProcessStartInfo.ArgumentList over a joined Arguments string | ⬜ |
| 050 | NamedPipeAccessControl | PipeSecurity, ACLs, rejecting unauthorised peers | ⬜ |
| 051 | SecretRedactionInLogs | structured logging, redaction of sensitive values, log injection | ⬜ |
| 052 | RestrictiveFileAcl | file ACLs at creation, inherited permissions, least privilege | ⬜ |

## desktop-wpf (053–060) — the WPF attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 053 | PasswordBoxNoPlaintextBinding | PasswordBox, why Password is not a DependencyProperty | ⬜ |
| 054 | SensitiveBufferLifetime | clearing sensitive buffers, bounded lifetime of plaintext | ⬜ |
| 055 | ClipboardHygiene | clipboard as shared state, excluding data from history | ⬜ |
| 056 | DragDropUntrustedPayload | validating dropped formats and paths before acting | ⬜ |
| 057 | EmbeddedBrowserNavigationPolicy | navigation allowlists, scheme restrictions, host object exposure | ⬜ |
| 058 | XamlReaderUntrustedMarkup | XamlReader.Parse as code execution, restricting parsed markup | ⬜ |
| 059 | BindingErrorLeakage | binding failure surfaces, tooltips and traces as leak channels | ⬜ |
| 060 | FilePickerResultStillUntrusted | dialog results are user input, post-dialog validation | ⬜ |
