import { describe, expect, it } from "vitest";
import {
  makeMarker,
  makeSideTable,
  makeWeakHolder,
  useAsWeakKey,
} from "@ex/02-intermediate/ex050_weak_collections/index.js";

describe("ex050 makeSideTable", () => {
  it("stores data against an object without touching it", () => {
    const table = makeSideTable();
    const key = { id: 1 };
    table.set(key, "secret");
    expect(table.get(key)).toBe("secret");
    expect(Object.keys(key)).toEqual(["id"]);
    expect(JSON.stringify(key)).toBe('{"id":1}');
  });

  it("keys on identity, not on shape", () => {
    const table = makeSideTable();
    table.set({ id: 1 }, "first");
    expect(table.get({ id: 1 })).toBeUndefined();
  });

  it("keeps two instances apart", () => {
    const table = makeSideTable();
    const a = {};
    const b = {};
    table.set(a, "a-data");
    table.set(b, "b-data");
    expect(table.get(a)).toBe("a-data");
    expect(table.get(b)).toBe("b-data");
  });

  it("reports absence", () => {
    const table = makeSideTable();
    expect(table.has({})).toBe(false);
    expect(table.get({})).toBeUndefined();
  });

  it("exposes no way to enumerate or count what it holds", () => {
    // A WeakMap has no size, no keys() and no iterator — by design, since
    // exposing them would let you observe garbage collection.
    const table = makeSideTable();
    table.set({}, 1);
    expect(Object.values(table).every((value) => typeof value === "function")).toBe(true);
    expect(table.size).toBeUndefined();
    expect(table[Symbol.iterator]).toBeUndefined();
  });

  it("returns the value it stored", () => {
    expect(makeSideTable().set({}, 42)).toBe(42);
  });
});

describe("ex050 makeMarker", () => {
  it("marks and reports", () => {
    const marker = makeMarker();
    const object = {};
    expect(marker.wasMarked(object)).toBe(false);
    marker.mark(object);
    expect(marker.wasMarked(object)).toBe(true);
  });

  it("is idempotent", () => {
    const marker = makeMarker();
    const object = {};
    marker.mark(object);
    marker.mark(object);
    expect(marker.wasMarked(object)).toBe(true);
  });

  it("keeps separate markers separate", () => {
    const first = makeMarker();
    const second = makeMarker();
    const object = {};
    first.mark(object);
    expect(second.wasMarked(object)).toBe(false);
  });
});

describe("ex050 useAsWeakKey", () => {
  it("accepts objects, arrays and functions", () => {
    expect(useAsWeakKey({})).toBe("ok");
    expect(useAsWeakKey([])).toBe("ok");
    expect(useAsWeakKey(() => {})).toBe("ok");
  });

  it("refuses primitives", () => {
    expect(useAsWeakKey("string")).toBe("TypeError");
    expect(useAsWeakKey(42)).toBe("TypeError");
    expect(useAsWeakKey(null)).toBe("TypeError");
  });

  it("accepts a unique symbol but not a registered one", () => {
    // Symbol.for() lives in a global registry, so it can never be collected
    // and is therefore not allowed to be a weak key.
    expect(useAsWeakKey(Symbol("unique"))).toBe("ok");
    expect(useAsWeakKey(Symbol.for("registered"))).toBe("TypeError");
  });
});

describe("ex050 makeWeakHolder", () => {
  it("dereferences to the object while it is alive", () => {
    const object = { alive: true };
    expect(makeWeakHolder(object).get()).toBe(object);
  });

  it("holds the object without a strong property to it", () => {
    const holder = makeWeakHolder({});
    expect(Object.values(holder).every((value) => typeof value === "function")).toBe(true);
  });
});
