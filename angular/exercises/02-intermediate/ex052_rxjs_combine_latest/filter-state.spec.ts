import { of, Subject } from "rxjs";
import {
  combinedFilters,
  combinedFiltersWithDefaults,
  filterSummary,
  Filters,
  filtersOnSearch,
  joinAll,
} from "./filter-state";

describe("combinedFilters", () => {
  let term: Subject<string>;
  let category: Subject<string>;
  let inStock: Subject<boolean>;
  let seen: Filters[];

  beforeEach(() => {
    term = new Subject<string>();
    category = new Subject<string>();
    inStock = new Subject<boolean>();
    seen = [];
  });

  it("emits nothing until every source has emitted", () => {
    combinedFilters(term, category, inStock).subscribe((filters) => seen.push(filters));

    term.next("chair");
    category.next("furniture");

    // The rule that makes a filter panel look broken: two of three is not enough.
    expect(seen).toEqual([]);

    inStock.next(true);

    expect(seen).toEqual([{ term: "chair", category: "furniture", inStock: true }]);
  });

  it("emits again on any single change", () => {
    combinedFilters(term, category, inStock).subscribe((filters) => seen.push(filters));

    term.next("chair");
    category.next("furniture");
    inStock.next(false);
    term.next("table");

    expect(seen).toEqual([
      { term: "chair", category: "furniture", inStock: false },
      { term: "table", category: "furniture", inStock: false },
    ]);
  });

  it("carries the latest of each, not a matched pair", () => {
    combinedFilters(term, category, inStock).subscribe((filters) => seen.push(filters));

    term.next("a");
    category.next("x");
    inStock.next(true);
    term.next("b");
    term.next("c");

    expect(seen.map((f) => f.term)).toEqual(["a", "b", "c"]);
    expect(seen.every((f) => f.category === "x")).toBe(true);
  });
});

describe("combinedFiltersWithDefaults", () => {
  it("emits immediately", () => {
    const seen: Filters[] = [];
    combinedFiltersWithDefaults(new Subject(), new Subject(), new Subject()).subscribe(
      (filters) => seen.push(filters),
    );

    // startWith gives every source a value, so there is nothing to wait for.
    expect(seen).toEqual([{ term: "", category: "all", inStock: false }]);
  });

  it("then follows each change", () => {
    const term = new Subject<string>();
    const seen: Filters[] = [];
    combinedFiltersWithDefaults(term, new Subject(), new Subject()).subscribe((filters) =>
      seen.push(filters),
    );

    term.next("chair");

    expect(seen).toEqual([
      { term: "", category: "all", inStock: false },
      { term: "chair", category: "all", inStock: false },
    ]);
  });
});

describe("filterSummary", () => {
  it("describes the defaults", () => {
    const seen: string[] = [];
    filterSummary(new Subject(), new Subject(), new Subject()).subscribe((s) => seen.push(s));

    expect(seen).toEqual(["anything in all"]);
  });

  it("uses the term when there is one", () => {
    const term = new Subject<string>();
    const seen: string[] = [];
    filterSummary(term, new Subject(), new Subject()).subscribe((s) => seen.push(s));

    term.next("chair");

    expect(seen[seen.length - 1]).toBe("chair in all");
  });

  it("adds the stock note", () => {
    const inStock = new Subject<boolean>();
    const seen: string[] = [];
    filterSummary(new Subject(), new Subject(), inStock).subscribe((s) => seen.push(s));

    inStock.next(true);

    expect(seen[seen.length - 1]).toBe("anything in all (in stock)");
  });

  it("combines everything", () => {
    const term = new Subject<string>();
    const category = new Subject<string>();
    const inStock = new Subject<boolean>();
    const seen: string[] = [];
    filterSummary(term, category, inStock).subscribe((s) => seen.push(s));

    term.next("chair");
    category.next("furniture");
    inStock.next(true);

    expect(seen[seen.length - 1]).toBe("chair in furniture (in stock)");
  });
});

describe("filtersOnSearch", () => {
  let searches: Subject<void>;
  let term: Subject<string>;
  let category: Subject<string>;
  let inStock: Subject<boolean>;
  let seen: Filters[];

  beforeEach(() => {
    searches = new Subject<void>();
    term = new Subject<string>();
    category = new Subject<string>();
    inStock = new Subject<boolean>();
    seen = [];
    filtersOnSearch(searches, term, category, inStock).subscribe((f) => seen.push(f));
  });

  it("emits nothing when only a filter changes", () => {
    term.next("chair");
    category.next("furniture");
    inStock.next(true);

    // The asymmetry is the whole point: filters do not drive.
    expect(seen).toEqual([]);
  });

  it("emits a snapshot when the search fires", () => {
    term.next("chair");
    category.next("furniture");
    inStock.next(true);

    searches.next();

    expect(seen).toEqual([{ term: "chair", category: "furniture", inStock: true }]);
  });

  it("emits once per search", () => {
    term.next("chair");
    category.next("furniture");
    inStock.next(false);

    searches.next();
    searches.next();

    expect(seen).toHaveLength(2);
  });

  it("takes the filters as they are at that moment", () => {
    term.next("chair");
    category.next("furniture");
    inStock.next(false);
    searches.next();

    term.next("table");
    searches.next();

    expect(seen.map((f) => f.term)).toEqual(["chair", "table"]);
  });

  it("emits nothing while a source has never produced a value", () => {
    // withLatestFrom has the same all-sources-needed rule as combineLatest.
    searches.next();

    expect(seen).toEqual([]);
  });
});

describe("joinAll", () => {
  it("joins completed sources", () => {
    const seen: string[] = [];
    joinAll(of("a"), of("b"), of("c")).subscribe((joined) => seen.push(joined));

    expect(seen).toEqual(["a, b, c"]);
  });

  it("takes the last value of each", () => {
    const seen: string[] = [];
    joinAll(of("a", "z"), of("b"), of("c")).subscribe((joined) => seen.push(joined));

    expect(seen).toEqual(["z, b, c"]);
  });

  it("emits once, not per change", () => {
    const seen: string[] = [];
    joinAll(of("a"), of("b"), of("c")).subscribe((joined) => seen.push(joined));

    expect(seen).toHaveLength(1);
  });

  it("never emits when a source does not complete", () => {
    const open = new Subject<string>();
    const seen: string[] = [];
    joinAll(open, of("b"), of("c")).subscribe((joined) => seen.push(joined));

    open.next("a");

    // forkJoin waits for completion, so a long-lived stream makes it useless.
    expect(seen).toEqual([]);

    open.complete();
    expect(seen).toEqual(["a, b, c"]);
  });
});
