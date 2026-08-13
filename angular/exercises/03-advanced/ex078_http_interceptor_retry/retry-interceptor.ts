import { InjectionToken } from "@angular/core";
import { HttpInterceptorFn } from "@angular/common/http";

// Exercise 078 — an HTTP interceptor that retries transient failures with backoff (advanced).
// Goal:   stop a flaky backend from turning a single dropped request into a failed page load,
//         without hammering a backend that is genuinely down.
// Drills: HttpInterceptorFn, rxjs `retry({ count, delay })`, and telling a transient failure
//         (worth retrying) apart from a permanent one (not worth retrying).
// Passes: when `npx jest exercises/03-advanced/ex078_http_interceptor_retry` is green.
//
// `retry()`'s `delay` option is not just a pause — it is how the operator decides whether to
// retry at all. Returning an observable that eventually emits means "try again"; returning (or
// throwing into) an observable that errors means "stop, and let this error reach the caller."
// That is the lever this interceptor pulls: a 503 gets a `timer(...)` so the retry happens after
// a backoff, while a 404 gets `throwError(...)` so it fails immediately on the first attempt.
//
// The backoff itself doubles on every attempt (`baseDelayMs`, then `baseDelayMs * 2`, then
// `baseDelayMs * 4`, ...) — a fixed delay would retry a struggling backend at the exact same rate
// that got it into trouble in the first place; a growing delay gives it room to recover instead
// of adding to the pile-up.
//
// Retrying is only ever correct for failures that might not repeat: a 5xx from an overloaded or
// momentarily-unavailable server, or a network drop. A 4xx means the request itself was wrong —
// retrying it identically will fail identically every time, so `retry` must give up immediately
// instead of spending attempts (and time) on a request that cannot succeed.

/** Tunable via DI so tests can use a near-zero delay instead of waiting on real backoff timing. */
export interface RetryConfig {
  /** Total attempts, including the first — not the number of retries. */
  readonly maxAttempts: number;
  readonly baseDelayMs: number;
}

export const RETRY_CONFIG = new InjectionToken<RetryConfig>("RETRY_CONFIG", {
  factory: (): RetryConfig => ({ maxAttempts: 3, baseDelayMs: 200 }),
});

/** attempt is 1-based: attempt 1 waits baseDelayMs, attempt 2 waits baseDelayMs * 2, ... */
export function computeBackoffDelayMs(attempt: number, baseDelayMs: number): number {
  return baseDelayMs * 2 ** (attempt - 1);
}

/**
 * TODO: implement retryInterceptor.
 *
 * Forward the request. If it fails with an HttpErrorResponse whose status is >= 500, retry with
 * an exponentially growing delay (computeBackoffDelayMs), up to RETRY_CONFIG.maxAttempts total
 * attempts. Any other failure (a 4xx, or anything that is not an HttpErrorResponse) must not be
 * retried at all — let it fail on the first attempt.
 */
export const retryInterceptor: HttpInterceptorFn = (req, next) => {
  throw new Error("TODO: implement retryInterceptor");
};
