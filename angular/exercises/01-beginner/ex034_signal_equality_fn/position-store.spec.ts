import { TestBed } from "@angular/core/testing";
import { PositionStore } from "./position-store";

describe("PositionStore", () => {
  let store: PositionStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(PositionStore);
  });

  it("starts at the origin and moves off it", () => {
    expect(store.position()).toEqual({ x: 0, y: 0 });
    expect(store.distance()).toBe(0);

    store.moveTo(5, 12);

    expect(store.distance()).toBe(13);
  });

  it("moves", () => {
    store.moveTo(3, 4);

    expect(store.position()).toEqual({ x: 3, y: 4 });
    expect(store.distance()).toBe(5);
  });

  it("recomputes for a real move", () => {
    expect(store.distance()).toBe(0);
    const before = store.recomputes;

    store.moveTo(3, 4);

    expect(store.distance()).toBe(5);
    expect(store.recomputes).toBe(before + 1);
  });

  it("ignores a new object holding the same values", () => {
    store.moveTo(3, 4);
    expect(store.distance()).toBe(5);
    const before = store.recomputes;

    store.moveTo(3, 4);

    // A different object, equal contents. `equal` said "same", so nobody was told.
    expect(store.distance()).toBe(5);
    expect(store.recomputes).toBe(before);
  });

  it("keeps the object it already had", () => {
    store.moveTo(3, 4);
    const kept = store.position();

    store.moveTo(3, 4);

    // The new object was discarded, not swapped in — worth knowing if anything downstream
    // compares references.
    expect(store.position()).toBe(kept);
  });

  it("notices a change in either coordinate", () => {
    store.moveTo(3, 4);
    expect(store.distance()).toBe(5);
    const before = store.recomputes;

    // The read is what runs the body. A computed is lazy, so setting the signal only marks
    // it dirty — without reading, `recomputes` would not move whatever changed.
    store.moveTo(3, 5);
    store.distance();
    expect(store.recomputes).toBe(before + 1);

    store.moveTo(4, 5);
    store.distance();
    expect(store.recomputes).toBe(before + 2);
  });

  it("recomputes every time without a custom equality", () => {
    expect(store.naiveDistance()).toBe(0);
    const naiveBefore = store.naiveRecomputes;
    expect(store.distance()).toBe(0);
    const equalBefore = store.recomputes;

    // Reading between each set is deliberate. Laziness means several notifications collapse
    // into one recompute if nothing reads in between, so batching them would hide the very
    // difference this test is about.
    for (const _ of [1, 2, 3]) {
      store.moveNaivelyTo(3, 4);
      store.naiveDistance();
      store.moveTo(3, 4);
      store.distance();
    }

    expect(store.naiveDistance()).toBe(5);
    expect(store.distance()).toBe(5);

    // Three identical positions: Object.is compares references, so each new object counted
    // as a change and cost a recompute...
    expect(store.naiveRecomputes).toBe(naiveBefore + 3);

    // ...while the custom `equal` recognised the first one as a real move and the other two
    // as nothing at all.
    expect(store.recomputes).toBe(equalBefore + 1);
  });

  it("agrees with the naive version on the value", () => {
    store.moveTo(6, 8);
    store.moveNaivelyTo(6, 8);

    expect(store.distance()).toBe(10);
    expect(store.naiveDistance()).toBe(10);
  });

  it("stores tags", () => {
    store.setTags(["alpha", "beta"]);

    expect(store.tags()).toEqual(["alpha", "beta"]);
  });

  it("notices a change in length", () => {
    store.setTags(["alpha"]);
    store.setTags(["alpha", "beta"]);

    expect(store.tags()).toEqual(["alpha", "beta"]);
  });

  it("silently drops a same-length change", () => {
    store.setTags(["alpha", "beta"]);

    store.setTags(["gamma", "delta"]);

    // Completely different contents, same length. A too-loose `equal` reported "same" and
    // the update vanished — no error, no warning, just the old value. This is the failure
    // mode to fear: `equal` must cover every field a consumer can observe.
    expect(store.tags()).toEqual(["alpha", "beta"]);
  });

  it("cannot recover the dropped value except by changing length", () => {
    store.setTags(["alpha", "beta"]);
    store.setTags(["gamma", "delta"]);
    expect(store.tags()).toEqual(["alpha", "beta"]);

    store.setTags(["gamma", "delta", "epsilon"]);

    expect(store.tags()).toEqual(["gamma", "delta", "epsilon"]);
  });
});
