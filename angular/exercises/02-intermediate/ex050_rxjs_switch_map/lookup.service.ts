import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

// Exercise 050 — switchMap and the flattening operators (intermediate).
// Goal:   start an inner stream per outer value, and choose what happens to the one already running.
// Drills: switchMap cancelling the previous inner stream, mergeMap letting them race, concatMap
//         queueing them, exhaustMap ignoring new ones, and the out-of-order bug this all prevents.
// Passes: when `npx jest exercises/02-intermediate/ex050_rxjs_switch_map` is green.
//
// Four operators, one question: a new outer value arrives while the previous inner stream is still
// running — what now?
//
//   switchMap  — unsubscribe from the old one and start fresh. The right default for a lookup:
//                a stale answer is worthless and, worse, might arrive last and overwrite the
//                good one.
//   mergeMap   — let them all run. Fine when order does not matter and every result counts.
//   concatMap  — wait, then start the next. Use when order matters or the request is not
//                idempotent.
//   exhaustMap — ignore new values while one is in flight. The double-click guard.
//
// The bug switchMap exists to prevent is worth stating plainly. Type "a", then "ab": two requests
// go out, and nothing guarantees they come back in order. With mergeMap, if "a" answers second,
// the user sees results for "a" while the box reads "ab". switchMap makes that impossible, because
// the "a" subscription is gone before "ab" starts.
//
// The service below hands out inner streams the spec controls, so each operator's behaviour is
// observable rather than a matter of timing luck.

@Injectable({ providedIn: "root" })
export class LookupBackend {
  /** Every term asked about, in order. */
  readonly queries: string[] = [];

  /** Terms whose inner stream was unsubscribed before completing. */
  readonly cancelled: string[] = [];

  private readonly pending = new Map<string, Subject<string>>();

  /** An inner stream for `term`, which emits only when the spec says so. */
  lookup(term: string): Observable<string> {
    this.queries.push(term);
    const subject = new Subject<string>();
    // Tracked explicitly rather than read off the Subject: `closed` is set by unsubscribe(),
    // *not* by complete(), so a normally finished stream would otherwise look cancelled.
    let finished = false;
    this.pending.set(term, subject);
    return new Observable<string>((subscriber) => {
      const subscription = subject.subscribe({
        next: (value) => subscriber.next(value),
        error: (error: unknown) => subscriber.error(error),
        complete: () => {
          finished = true;
          subscriber.complete();
        },
      });
      // Teardown runs on both paths; `finished` is what tells them apart.
      return () => {
        if (!finished) {
          this.cancelled.push(term);
        }
        subscription.unsubscribe();
      };
    });
  }

  /** Answer one outstanding lookup and complete it. */
  respond(term: string, result: string): void {
    const subject = this.pending.get(term);
    if (subject === undefined) {
      throw new Error(`no outstanding lookup for "${term}"`);
    }
    subject.next(result);
    subject.complete();
    this.pending.delete(term);
  }

  /** Whether a lookup is still waiting. */
  isPending(term: string): boolean {
    return this.pending.has(term);
  }
}

/**
 * TODO: map each term to a lookup, cancelling any lookup already in flight.
 *
 * The operator that makes a stale answer impossible.
 */
export function switchLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  throw new Error("TODO: implement switchLookup");
}

/** TODO: the same, but letting every lookup run and emit in whatever order it finishes. */
export function mergeLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  throw new Error("TODO: implement mergeLookup");
}

/** TODO: the same, but running them strictly one after another, in order. */
export function concatLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  throw new Error("TODO: implement concatLookup");
}

/** TODO: the same, but ignoring new terms while a lookup is in flight. */
export function exhaustLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  throw new Error("TODO: implement exhaustLookup");
}
