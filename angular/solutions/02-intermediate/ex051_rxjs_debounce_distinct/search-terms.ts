import {
  debounceTime,
  distinctUntilChanged,
  filter,
  map,
  Observable,
  throttleTime,
} from "rxjs";

// Exercise 051 — debounceTime and distinctUntilChanged (reference solution).

export function searchTerms(source: Observable<string>): Observable<string> {
  return source.pipe(
    // Waits for a gap and emits the latest value. A user who never pauses gets nothing.
    debounceTime(300),
    map((term) => term.trim()),
    filter((term) => term.length >= 2),
    // After the trim, so "ada" and "ada " count as the same term.
    distinctUntilChanged(),
  );
}

export function searchTermsDistinctFirst(source: Observable<string>): Observable<string> {
  return source.pipe(
    debounceTime(300),
    // Before the trim, so "ada" and "ada " are different values and both get through. Same
    // operators, one line moved, one extra pointless request.
    distinctUntilChanged(),
    map((term) => term.trim()),
    filter((term) => term.length >= 2),
  );
}

export function searchTermsEager(source: Observable<string>): Observable<string> {
  return source.pipe(
    map((term) => term.trim()),
    filter((term) => term.length >= 2),
    distinctUntilChanged(),
  );
}

export function throttledTerms(source: Observable<string>): Observable<string> {
  // Emits immediately then ignores values for the window — the opposite trade-off to debounce.
  return source.pipe(throttleTime(300));
}
