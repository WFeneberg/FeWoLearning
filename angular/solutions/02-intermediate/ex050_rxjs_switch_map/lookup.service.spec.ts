import { TestBed } from "@angular/core/testing";
import { Subject } from "rxjs";
import {
  concatLookup,
  exhaustLookup,
  LookupBackend,
  mergeLookup,
  switchLookup,
} from "./lookup.service";

describe("flattening operators", () => {
  let backend: LookupBackend;
  let terms: Subject<string>;
  let seen: string[];

  beforeEach(() => {
    TestBed.configureTestingModule({});
    backend = TestBed.inject(LookupBackend);
    terms = new Subject<string>();
    seen = [];
  });

  describe("switchLookup", () => {
    beforeEach(() => {
      switchLookup(terms, backend).subscribe((result) => seen.push(result));
    });

    it("emits the result of a single lookup", () => {
      terms.next("a");
      backend.respond("a", "A");

      expect(seen).toEqual(["A"]);
    });

    it("cancels the previous lookup", () => {
      terms.next("a");
      terms.next("ab");

      expect(backend.queries).toEqual(["a", "ab"]);
      expect(backend.cancelled).toEqual(["a"]);
    });

    it("ignores a cancelled lookup that answers late", () => {
      terms.next("a");
      terms.next("ab");
      backend.respond("ab", "AB");

      // "a" was unsubscribed, so its answer can never arrive — the stale-result bug is
      // structurally impossible rather than merely unlikely.
      expect(seen).toEqual(["AB"]);
      expect(backend.isPending("a")).toBe(true);
    });

    it("emits each result when the terms are spaced out", () => {
      terms.next("a");
      backend.respond("a", "A");
      terms.next("b");
      backend.respond("b", "B");

      expect(seen).toEqual(["A", "B"]);
      expect(backend.cancelled).toEqual([]);
    });
  });

  describe("mergeLookup", () => {
    beforeEach(() => {
      mergeLookup(terms, backend).subscribe((result) => seen.push(result));
    });

    it("keeps every lookup running", () => {
      terms.next("a");
      terms.next("ab");

      expect(backend.cancelled).toEqual([]);
      expect(backend.isPending("a")).toBe(true);
      expect(backend.isPending("ab")).toBe(true);
    });

    it("emits in completion order, not request order", () => {
      terms.next("a");
      terms.next("ab");

      backend.respond("ab", "AB");
      backend.respond("a", "A");

      // "a" answered last and so lands last. In a search box this is the stale result
      // overwriting the fresh one.
      expect(seen).toEqual(["AB", "A"]);
    });
  });

  describe("concatLookup", () => {
    beforeEach(() => {
      concatLookup(terms, backend).subscribe((result) => seen.push(result));
    });

    it("does not start the second until the first finishes", () => {
      terms.next("a");
      terms.next("b");

      expect(backend.queries).toEqual(["a"]);
    });

    it("starts the queued lookup once the first completes", () => {
      terms.next("a");
      terms.next("b");
      backend.respond("a", "A");

      expect(backend.queries).toEqual(["a", "b"]);
      backend.respond("b", "B");

      // Always request order, whatever the timings.
      expect(seen).toEqual(["A", "B"]);
    });
  });

  describe("exhaustLookup", () => {
    beforeEach(() => {
      exhaustLookup(terms, backend).subscribe((result) => seen.push(result));
    });

    it("ignores terms arriving while one is in flight", () => {
      terms.next("a");
      terms.next("b");
      terms.next("c");

      // The double-click guard: the extra values are dropped, not queued.
      expect(backend.queries).toEqual(["a"]);
    });

    it("accepts a new term once the previous one is done", () => {
      terms.next("a");
      terms.next("ignored");
      backend.respond("a", "A");

      terms.next("b");
      backend.respond("b", "B");

      expect(backend.queries).toEqual(["a", "b"]);
      expect(seen).toEqual(["A", "B"]);
    });
  });

  describe("choosing between them", () => {
    it("differs only when values overlap", () => {
      const switchSeen: string[] = [];
      const switchTerms = new Subject<string>();
      switchLookup(switchTerms, backend).subscribe((r) => switchSeen.push(r));

      switchTerms.next("solo");
      backend.respond("solo", "SOLO");

      // With no overlap all four operators behave identically; the choice only matters under
      // concurrency, which is exactly why the wrong one survives code review.
      expect(switchSeen).toEqual(["SOLO"]);
      expect(backend.cancelled).toEqual([]);
    });
  });
});
