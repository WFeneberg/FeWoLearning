import { Observable } from "rxjs";

// Exercise 052 — combineLatest and withLatestFrom (intermediate).
// Goal:   derive one value from several independent streams.
// Drills: combineLatest, the fact that it emits nothing until every source has, startWith as the
//         fix for that, withLatestFrom for "one stream drives, the others are consulted", and
//         forkJoin for "wait for all of them to finish".
// Passes: when `npx jest exercises/02-intermediate/ex052_rxjs_combine_latest` is green.
//
// combineLatest takes the most recent value from each source and emits a combination whenever
// *any* of them changes. Its one surprising rule: it emits nothing at all until every source has
// produced at least one value. A filter panel wired to three controls where one has never been
// touched shows nothing, and the stream looks broken. `startWith` on each source is the fix, and
// a BehaviorSubject — which always has a current value — sidesteps the problem entirely.
//
// withLatestFrom is asymmetric on purpose. Only the source stream causes an emission; the others
// are just read at that moment. That is the difference between "recompute whenever anything
// changes" (combineLatest) and "when the user hits search, take whatever the filters currently
// say" (withLatestFrom) — and using the first where you meant the second is how a search fires on
// every filter change instead of on the button press.
//
// forkJoin is the odd one out: it waits for every source to *complete* and emits their final
// values once. Perfect for parallel HTTP calls, useless for anything long-lived — a forkJoin over
// a Subject never emits at all.

export interface Filters {
  readonly term: string;
  readonly category: string;
  readonly inStock: boolean;
}

/**
 * TODO: combine three streams into a Filters object.
 *
 * Emits whenever any source changes, and — as combineLatest requires — nothing until all three
 * have emitted at least once.
 */
export function combinedFilters(
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<Filters> {
  throw new Error("TODO: implement combinedFilters");
}

/**
 * TODO: the same, but each source gets a starting value so the result emits immediately.
 *
 * Defaults: term "", category "all", inStock false.
 */
export function combinedFiltersWithDefaults(
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<Filters> {
  throw new Error("TODO: implement combinedFiltersWithDefaults");
}

/**
 * TODO: a human summary of the filters, recomputed on every change.
 *
 * `"<term or 'anything'> in <category>"`, plus " (in stock)" when inStock. Built on
 * combinedFiltersWithDefaults, so it emits straight away.
 */
export function filterSummary(
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<string> {
  throw new Error("TODO: implement filterSummary");
}

/**
 * TODO: emit a Filters snapshot only when `searches` fires.
 *
 * withLatestFrom: the button drives, the filters are consulted. A filter change on its own must
 * emit nothing.
 */
export function filtersOnSearch(
  searches: Observable<void>,
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<Filters> {
  throw new Error("TODO: implement filtersOnSearch");
}

/**
 * TODO: wait for three one-shot sources to complete, then emit their values joined with ", ".
 *
 * forkJoin. Note it emits once, at the end, and never at all if a source does not complete.
 */
export function joinAll(
  first: Observable<string>,
  second: Observable<string>,
  third: Observable<string>,
): Observable<string> {
  throw new Error("TODO: implement joinAll");
}
