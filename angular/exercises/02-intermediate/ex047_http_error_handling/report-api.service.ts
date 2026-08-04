import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 047 — HTTP error handling (intermediate).
// Goal:   decide, per call, whether a failure is a fallback, a domain error, or worth retrying.
// Drills: catchError, throwError, reading HttpErrorResponse (status, error, message), translating
//         it into a domain error, retry({count}), and what status 0 actually means.
// Passes: when `npx jest exercises/02-intermediate/ex047_http_error_handling` is green.
//
// HttpClient signals failure by erroring the observable with an HttpErrorResponse. The field that
// matters most is `status`, and its most important value is the surprising one: **0 means the
// request never got a response at all** — offline, DNS failure, CORS rejection, connection
// refused. It is not a server error and must not be reported as one, because the only useful
// advice for it is "check your connection".
//
// There is no single right response to a failure, which is the point of this exercise. Three
// strategies, each right somewhere:
//
//   fallback — swallow it and carry on with a default. Right for something optional, and wrong
//              for anything the user is waiting on, because it hides the problem.
//   translate — turn it into a domain error the rest of the app understands, so nothing above
//              this layer has to know HttpErrorResponse exists.
//   retry    — for a transient failure. Only ever safe on an idempotent request: retrying a POST
//              can create two records.
//
// `retry({count: 2})` re-subscribes, and re-subscribing an HttpClient observable sends the
// request again — which is why a test sees a fresh request per attempt.

/** A domain error, so nothing above this service needs to know about HTTP. */
export class ReportError extends Error {
  constructor(
    readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "ReportError";
  }
}

export interface Report {
  readonly id: number;
  readonly title: string;
  readonly total: number;
}

export const EMPTY_REPORT: Report = { id: 0, title: "unavailable", total: 0 };

@Injectable({ providedIn: "root" })
export class ReportApi {
  static readonly base = "/api/reports";

  /** TODO: inject HttpClient. */

  /**
   * Classify a failure.
   *
   * "offline" for status 0, "client" for 400–499, "server" for 500 and above, "unknown"
   * otherwise. Note status 0 is the one that is *not* a server problem.
   */
  classify(error: HttpErrorResponse): string {
    throw new Error("TODO: implement classify");
  }

  /** A readable message: `"<classification>: <status>"`, e.g. "server: 500", "offline: 0". */
  describe(error: HttpErrorResponse): string {
    throw new Error("TODO: implement describe");
  }

  /**
   * GET /api/reports/<id>, falling back to EMPTY_REPORT on any failure.
   *
   * The observable must complete normally — a caller of this never sees an error.
   */
  fetchOrDefault(id: number): Observable<Report> {
    throw new Error("TODO: implement fetchOrDefault");
  }

  /**
   * GET /api/reports/<id>, translating any failure into a ReportError.
   *
   * The error carries the status and `describe()`'s message. Callers above this layer never
   * see an HttpErrorResponse.
   */
  fetchOrThrow(id: number): Observable<Report> {
    throw new Error("TODO: implement fetchOrThrow");
  }

  /**
   * GET /api/reports/<id>, retrying twice before giving up.
   *
   * Three attempts in total. A failure that survives all three becomes a ReportError, as in
   * fetchOrThrow. Safe here only because a GET is idempotent.
   */
  fetchWithRetry(id: number): Observable<Report> {
    throw new Error("TODO: implement fetchWithRetry");
  }

  /**
   * How many requests the last fetchWithRetry made.
   *
   * Counted by the service so the spec can assert the attempts without inspecting internals.
   */
  attempts = 0;
}
