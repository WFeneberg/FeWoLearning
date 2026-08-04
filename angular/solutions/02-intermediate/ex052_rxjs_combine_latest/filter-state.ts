import { combineLatest, forkJoin, map, Observable, startWith, withLatestFrom } from "rxjs";

// Exercise 052 — combineLatest and withLatestFrom (reference solution).

export interface Filters {
  readonly term: string;
  readonly category: string;
  readonly inStock: boolean;
}

export function combinedFilters(
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<Filters> {
  // Nothing is emitted until all three have produced a value — the rule that makes an
  // untouched filter panel look broken.
  return combineLatest([term, category, inStock]).pipe(
    map(([termValue, categoryValue, inStockValue]) => ({
      term: termValue,
      category: categoryValue,
      inStock: inStockValue,
    })),
  );
}

export function combinedFiltersWithDefaults(
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<Filters> {
  // startWith gives each source a value up front, so there is nothing to wait for. A
  // BehaviorSubject would have the same effect without the operator.
  return combinedFilters(
    term.pipe(startWith("")),
    category.pipe(startWith("all")),
    inStock.pipe(startWith(false)),
  );
}

export function filterSummary(
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<string> {
  return combinedFiltersWithDefaults(term, category, inStock).pipe(
    map((filters) => {
      const what = filters.term === "" ? "anything" : filters.term;
      const stock = filters.inStock ? " (in stock)" : "";
      return `${what} in ${filters.category}${stock}`;
    }),
  );
}

export function filtersOnSearch(
  searches: Observable<void>,
  term: Observable<string>,
  category: Observable<string>,
  inStock: Observable<boolean>,
): Observable<Filters> {
  // Asymmetric: only `searches` triggers an emission, the rest are read at that moment. Using
  // combineLatest here would fire a search on every filter change instead.
  return searches.pipe(
    withLatestFrom(term, category, inStock),
    map(([, termValue, categoryValue, inStockValue]) => ({
      term: termValue,
      category: categoryValue,
      inStock: inStockValue,
    })),
  );
}

export function joinAll(
  first: Observable<string>,
  second: Observable<string>,
  third: Observable<string>,
): Observable<string> {
  // Waits for completion, not for values — so this never emits over a long-lived stream.
  return forkJoin([first, second, third]).pipe(map((values) => values.join(", ")));
}
