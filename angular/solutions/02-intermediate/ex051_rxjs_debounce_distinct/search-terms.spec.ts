import { fakeAsync, tick } from "@angular/core/testing";
import { Subject } from "rxjs";
import {
  searchTerms,
  searchTermsDistinctFirst,
  searchTermsEager,
  throttledTerms,
} from "./search-terms";

describe("searchTerms", () => {
  let keystrokes: Subject<string>;
  let seen: string[];

  beforeEach(() => {
    keystrokes = new Subject<string>();
    seen = [];
  });

  it("emits nothing until the typing pauses", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("an");
    tick(299);

    expect(seen).toEqual([]);

    tick(1);
    expect(seen).toEqual(["an"]);
  }));

  it("keeps only the last value of a burst", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("a");
    tick(100);
    keystrokes.next("an");
    tick(100);
    keystrokes.next("ang");
    tick(300);

    // Not "emit every 300ms" — one emission, the latest value.
    expect(seen).toEqual(["ang"]);
  }));

  it("emits nothing at all while typing never pauses", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    for (const term of ["a", "an", "ang", "angu", "angul"]) {
      keystrokes.next(term);
      tick(200);
    }

    expect(seen).toEqual([]);

    tick(300);
    expect(seen).toEqual(["angul"]);
  }));

  it("trims the term", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("  ada  ");
    tick(300);

    expect(seen).toEqual(["ada"]);
  }));

  it("drops terms shorter than two characters", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("a");
    tick(300);
    keystrokes.next("");
    tick(300);
    keystrokes.next("ab");
    tick(300);

    expect(seen).toEqual(["ab"]);
  }));

  it("counts length after trimming", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("  a  ");
    tick(300);

    expect(seen).toEqual([]);
  }));

  it("drops a repeat of the previous term", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("ada");
    tick(300);
    keystrokes.next("ada");
    tick(300);

    expect(seen).toEqual(["ada"]);
  }));

  it("treats a term differing only in whitespace as a repeat", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("ada");
    tick(300);
    keystrokes.next("ada  ");
    tick(300);

    // Trimming before the distinct check is what makes this one term rather than two.
    expect(seen).toEqual(["ada"]);
  }));

  it("only compares against the immediately previous term", fakeAsync(() => {
    searchTerms(keystrokes).subscribe((term) => seen.push(term));

    for (const term of ["ada", "bob", "ada"]) {
      keystrokes.next(term);
      tick(300);
    }

    // distinctUntilChanged, not distinct: coming back to an earlier term is a real change.
    expect(seen).toEqual(["ada", "bob", "ada"]);
  }));
});

describe("searchTermsDistinctFirst", () => {
  it("lets a whitespace-only difference through", fakeAsync(() => {
    const keystrokes = new Subject<string>();
    const seen: string[] = [];
    searchTermsDistinctFirst(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("ada");
    tick(300);
    keystrokes.next("ada  ");
    tick(300);

    // Same three operators, different order, one extra pointless request.
    expect(seen).toEqual(["ada", "ada"]);
  }));
});

describe("searchTermsEager", () => {
  it("emits on every qualifying keystroke", () => {
    const keystrokes = new Subject<string>();
    const seen: string[] = [];
    searchTermsEager(keystrokes).subscribe((term) => seen.push(term));

    for (const term of ["a", "an", "ang", "angu"]) {
      keystrokes.next(term);
    }

    // One request per character, which is what debouncing exists to prevent.
    expect(seen).toEqual(["an", "ang", "angu"]);
  });

  it("still drops repeats and short terms", () => {
    const keystrokes = new Subject<string>();
    const seen: string[] = [];
    searchTermsEager(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("a");
    keystrokes.next("ab");
    keystrokes.next("ab");

    expect(seen).toEqual(["ab"]);
  });
});

describe("throttledTerms", () => {
  it("emits the first value immediately", fakeAsync(() => {
    const keystrokes = new Subject<string>();
    const seen: string[] = [];
    throttledTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("a");

    // The opposite of debounce: emit now, then go quiet.
    expect(seen).toEqual(["a"]);
    tick(300);
  }));

  it("ignores values inside the window", fakeAsync(() => {
    const keystrokes = new Subject<string>();
    const seen: string[] = [];
    throttledTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("a");
    tick(100);
    keystrokes.next("b");
    tick(100);
    keystrokes.next("c");
    tick(300);

    expect(seen).toEqual(["a"]);
  }));

  it("accepts a value after the window", fakeAsync(() => {
    const keystrokes = new Subject<string>();
    const seen: string[] = [];
    throttledTerms(keystrokes).subscribe((term) => seen.push(term));

    keystrokes.next("a");
    tick(300);
    keystrokes.next("b");
    tick(300);

    expect(seen).toEqual(["a", "b"]);
  }));
});
