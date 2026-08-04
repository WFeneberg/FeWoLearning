import { Observable } from "rxjs";

// Exercise 051 — debounceTime and distinctUntilChanged (intermediate).
// Goal:   turn a stream of keystrokes into a stream of search-worthy terms.
// Drills: debounceTime, distinctUntilChanged, why the order of operators changes the result, and
//         the difference between debouncing and throttling.
// Passes: when `npx jest exercises/02-intermediate/ex051_rxjs_debounce_distinct` is green.
//
// A search box that fires on every keystroke sends one request per character, most of them for
// terms the user never intended to search. Two operators fix that, and they fix *different*
// problems — using one and expecting the other's effect is the usual mistake.
//
// debounceTime(300) waits for a 300ms gap and then emits the *latest* value. Everything typed
// during a burst is dropped except the last. Note the word latest: this is not "emit every 300ms"
// (that is throttleTime), and a user typing continuously for ten seconds produces nothing at all
// until they pause.
//
// distinctUntilChanged drops a value equal to the one *immediately before* it. Not equal to
// anything seen before — only the previous one, so a, b, a emits all three. That is what you want
// for "the term did not really change" and not what you want for deduplication.
//
// Operator order is load-bearing here, and this is the part worth internalising. Trimming before
// distinctUntilChanged means "ada" and "ada " are the same term and the second is dropped;
// trimming after means they differ and a pointless second request goes out. Same three operators,
// different behaviour.

/**
 * TODO: the pipeline a search box should use.
 *
 * In order: wait for a 300ms pause, trim, drop terms shorter than two characters, then drop a
 * term identical to the previous one.
 *
 * Trimming before the distinct check is deliberate — see the note above.
 */
export function searchTerms(source: Observable<string>): Observable<string> {
  throw new Error("TODO: implement searchTerms");
}

/**
 * TODO: the same operators with the distinct check *before* the trim.
 *
 * Kept so the spec can show the difference: "ada" followed by "ada " now produces two terms.
 */
export function searchTermsDistinctFirst(source: Observable<string>): Observable<string> {
  throw new Error("TODO: implement searchTermsDistinctFirst");
}

/**
 * TODO: no debounce at all, for contrast.
 *
 * Trim, drop short terms, drop repeats — every keystroke that qualifies gets through, which is
 * one request per character.
 */
export function searchTermsEager(source: Observable<string>): Observable<string> {
  throw new Error("TODO: implement searchTermsEager");
}

/**
 * TODO: emit at most one value per 300ms window, starting with the first.
 *
 * throttleTime, not debounceTime: this emits immediately and then ignores values for a while,
 * which is the opposite trade-off. Right for a scroll handler, wrong for a search box.
 */
export function throttledTerms(source: Observable<string>): Observable<string> {
  throw new Error("TODO: implement throttledTerms");
}
