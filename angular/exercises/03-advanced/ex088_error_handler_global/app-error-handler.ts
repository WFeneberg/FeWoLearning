import { ErrorHandler, Injectable, InjectionToken, inject } from "@angular/core";

// Exercise 088 — a custom global ErrorHandler with reporting (advanced).
// Goal:   replace Angular's default "log it to the console and move on" ErrorHandler with one that
//         normalizes whatever was thrown into a typed report and forwards it to a swappable sink.
// Drills: implementing the `ErrorHandler` interface, `InjectionToken`-based DI for the sink so it can
//         be faked in tests, and a de-duplication rule so one bug does not flood the sink.
// Passes: when `npx jest exercises/03-advanced/ex088_error_handler_global` is green.
//
// `ErrorHandler` is Angular's last stop for an exception it could not otherwise route anywhere —
// one thrown from a template expression, a lifecycle hook, an `effect()`, anything the framework's
// own call stack surfaces. Its contract is strict: `handleError` must never itself throw. There is
// nothing left to catch a second exception thrown while handling the first; it would simply crash
// past Angular's own internals. That is why every branch below funnels into `sink.report(...)`
// rather than any operation (JSON.stringify, property access on an unknown shape, etc.) that could
// itself fail — normalize defensively, then report, and nothing in between should be able to throw.
//
// `ERROR_SINK` is an `InjectionToken` rather than a concrete class for the usual reason: production
// wires a sink that calls out to a real telemetry backend, while a test wires an in-memory fake and
// asserts on what it received — the handler itself never needs to know which one it is holding.
//
// The de-duplication rule exists because a single root cause commonly throws more than once in a
// row — a broken `computed()` re-evaluated on every subsequent read, a retried request failing the
// same way each time. Reporting the identical message every time turns one incident into a flood in
// whatever's on the other end of `sink`. Comparing only against the *immediately previous* report
// (not history further back) keeps the rule simple and cheap: two unrelated errors of unrelated
// causes are never silently dropped just because the same message recurred earlier in the session.

export interface ErrorReport {
  readonly message: string;
  readonly stack?: string;
  readonly timestamp: number;
}

export interface ErrorSink {
  report(entry: ErrorReport): void;
}

export const ERROR_SINK = new InjectionToken<ErrorSink>("ERROR_SINK");

@Injectable()
export class AppErrorHandler implements ErrorHandler {
  private readonly sink = inject(ERROR_SINK);
  private lastReported: ErrorReport | null = null;

  /**
   * TODO: implement handleError.
   *   - Build an `ErrorReport`: if `error instanceof Error`, use its `message` and `stack`
   *     (stack may be `undefined`); otherwise there is no Error object at all, so the message is
   *     `String(error)` and `stack` is omitted entirely (do not set it to `undefined` explicitly —
   *     leave the property out, since `ErrorReport.stack` is optional).
   *   - `timestamp` is `Date.now()`.
   *   - Skip reporting (return without calling `sink.report`) when this report's `message` is
   *     identical to `this.lastReported`'s message — the de-duplication rule above. Still update
   *     `this.lastReported` in that case is unnecessary (it is already that report); a genuinely new
   *     report always updates `this.lastReported` to itself after reporting.
   *   - Never let anything here throw back out of `handleError`.
   */
  handleError(error: unknown): void {
    throw new Error("TODO: implement handleError");
  }
}
