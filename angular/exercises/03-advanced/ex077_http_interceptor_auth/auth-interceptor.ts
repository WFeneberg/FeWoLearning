import { Injectable, Signal, signal } from "@angular/core";
import { HttpInterceptorFn } from "@angular/common/http";

// Exercise 077 — a functional HTTP interceptor that attaches an auth header (advanced).
// Goal:   stop every call site from having to remember to add an Authorization header itself.
// Drills: HttpInterceptorFn, req.clone() (HttpRequest is immutable), and forwarding to `next`.
// Passes: when `npx jest exercises/03-advanced/ex077_http_interceptor_auth` is green.
//
// A functional interceptor is a plain function registered once, in `provideHttpClient(withInterceptors([...]))`
// — no class, no NgModule, nothing else needs to know it exists. Angular calls it for every
// outgoing request with two things: the request, and a `next` function that continues the chain to
// whatever interceptor (or the real backend) comes after this one. Every interceptor either calls
// `next(req)` to let the request through, or `next(modifiedReq)` to let a changed version through
// instead — there is no third option, because not calling `next` at all just means the request
// never goes anywhere.
//
// `HttpRequest` objects are immutable on purpose — the same request instance might be replayed
// (retries, multiple subscribers), so mutating it in place would be visible to code that has no
// idea an interceptor touched it. `req.clone({ ... })` is how you attach a header instead: it
// returns a new request with everything copied over except the fields you override, leaving the
// original untouched.
//
// The other half of the exercise is restraint: a request made while logged out should be passed
// through unchanged, not sent with an empty or fabricated Authorization header. An interceptor
// that always calls `.clone()` regardless of whether there is a token to attach would leak "this
// header exists but is meaningless" into every request instead of just leaving it off.

@Injectable({ providedIn: "root" })
export class TokenStore {
  private readonly token = signal<string | null>(null);

  /** Read-only — the interceptor only ever needs to read the current value. */
  readonly current: Signal<string | null> = this.token.asReadonly();

  set(value: string | null): void {
    this.token.set(value);
  }
}

/**
 * TODO: implement the interceptor.
 *
 * If TokenStore has a token, forward a clone of the request with an
 * `Authorization: Bearer <token>` header attached. Otherwise, forward the request unchanged.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  throw new Error("TODO: implement authInterceptor");
};
