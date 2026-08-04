import { Injectable } from "@angular/core";
import { concatMap, exhaustMap, mergeMap, Observable, Subject, switchMap } from "rxjs";

// Exercise 050 — switchMap and the flattening operators (reference solution).

@Injectable({ providedIn: "root" })
export class LookupBackend {
  readonly queries: string[] = [];

  readonly cancelled: string[] = [];

  private readonly pending = new Map<string, Subject<string>>();

  lookup(term: string): Observable<string> {
    this.queries.push(term);
    const subject = new Subject<string>();
    // A Subject's `closed` is set by unsubscribe(), not by complete(), so it cannot be used to
    // tell the two apart. Track completion explicitly instead.
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
      // Teardown runs on both paths; `finished` is what distinguishes a cancellation from a
      // normal finish.
      return () => {
        if (!finished) {
          this.cancelled.push(term);
        }
        subscription.unsubscribe();
      };
    });
  }

  respond(term: string, result: string): void {
    const subject = this.pending.get(term);
    if (subject === undefined) {
      throw new Error(`no outstanding lookup for "${term}"`);
    }
    subject.next(result);
    subject.complete();
    this.pending.delete(term);
  }

  isPending(term: string): boolean {
    return this.pending.has(term);
  }
}

export function switchLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  // Unsubscribes from the in-flight lookup before starting the new one, which makes a stale
  // result structurally impossible rather than merely unlikely.
  return terms.pipe(switchMap((term) => backend.lookup(term)));
}

export function mergeLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  // Everything runs; results arrive in completion order, which need not be request order.
  return terms.pipe(mergeMap((term) => backend.lookup(term)));
}

export function concatLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  // Queued: the next inner stream is not even created until the previous one completes.
  return terms.pipe(concatMap((term) => backend.lookup(term)));
}

export function exhaustLookup(
  terms: Observable<string>,
  backend: LookupBackend,
): Observable<string> {
  // Drops outer values while an inner stream is running — the double-click guard.
  return terms.pipe(exhaustMap((term) => backend.lookup(term)));
}
