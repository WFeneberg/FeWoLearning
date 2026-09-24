import { describe, expect, it } from "vitest";
import {
  getIn,
  removeIn,
  setIn,
  updateIn,
} from "@ex/03-advanced/ex085_immutable_updates/index.js";

const state = () => ({
  user: { name: "ada", address: { city: "Bern", zip: "3000" } },
  items: [{ id: 1, qty: 1 }, { id: 2, qty: 5 }],
  untouched: { deep: { value: 1 } },
});

describe("ex085 setIn", () => {
  it("sets a nested value", () => {
    const next = setIn(state(), ["user", "address", "city"], "Zurich");
    expect(next.user.address.city).toBe("Zurich");
    expect(next.user.address.zip).toBe("3000");
  });

  it("leaves the original alone", () => {
    const before = state();
    setIn(before, ["user", "name"], "grace");
    expect(before.user.name).toBe("ada");
  });

  it("copies every object on the path", () => {
    const before = state();
    const next = setIn(before, ["user", "address", "city"], "Zurich");
    expect(next).not.toBe(before);
    expect(next.user).not.toBe(before.user);
    expect(next.user.address).not.toBe(before.user.address);
  });

  it("SHARES every branch off the path", () => {
    // The whole point: an untouched subtree keeps its identity, so a
    // reference check is enough to know it did not change.
    const before = state();
    const next = setIn(before, ["user", "name"], "grace");
    expect(next.untouched).toBe(before.untouched);
    expect(next.items).toBe(before.items);
    expect(next.user.address).toBe(before.user.address);
  });

  it("keeps an array an array", () => {
    const before = state();
    const next = setIn(before, ["items", 0, "qty"], 99);
    expect(Array.isArray(next.items)).toBe(true);
    expect(next.items[0].qty).toBe(99);
    expect(next.items[1]).toBe(before.items[1]);
    expect(before.items[0].qty).toBe(1);
  });

  it("creates missing intermediate objects", () => {
    expect(setIn({}, ["a", "b", "c"], 1)).toEqual({ a: { b: { c: 1 } } });
  });

  it("replaces the whole value for an empty path", () => {
    expect(setIn({ a: 1 }, [], "replaced")).toBe("replaced");
  });
});

describe("ex085 updateIn", () => {
  it("computes from the old value", () => {
    const next = updateIn(state(), ["items", 1, "qty"], (qty) => qty + 1);
    expect(next.items[1].qty).toBe(6);
  });

  it("shares the untouched branches too", () => {
    const before = state();
    const next = updateIn(before, ["user", "name"], (name) => name.toUpperCase());
    expect(next.user.name).toBe("ADA");
    expect(next.items).toBe(before.items);
  });

  it("passes undefined for a value that is not there", () => {
    expect(updateIn({}, ["count"], (value) => (value ?? 0) + 1)).toEqual({ count: 1 });
  });
});

describe("ex085 getIn", () => {
  it("reads a nested value", () => {
    expect(getIn(state(), ["user", "address", "city"], "?")).toBe("Bern");
    expect(getIn(state(), ["items", 1, "id"], "?")).toBe(2);
  });

  it("falls back at any missing step", () => {
    expect(getIn(state(), ["user", "nope", "deeper"], "fallback")).toBe("fallback");
    expect(getIn({}, ["a"], "fallback")).toBe("fallback");
    expect(getIn(null, ["a"], "fallback")).toBe("fallback");
  });

  it("returns the whole object for an empty path", () => {
    const object = { a: 1 };
    expect(getIn(object, [], "fallback")).toBe(object);
  });
});

describe("ex085 removeIn", () => {
  it("deletes an object key", () => {
    const next = removeIn(state(), ["user", "address", "zip"]);
    expect(Object.hasOwn(next.user.address, "zip")).toBe(false);
    expect(next.user.address.city).toBe("Bern");
  });

  it("shortens an array", () => {
    const next = removeIn(state(), ["items", 0]);
    expect(next.items).toHaveLength(1);
    expect(next.items[0].id).toBe(2);
  });

  it("leaves the original alone and shares the rest", () => {
    const before = state();
    const next = removeIn(before, ["user", "name"]);
    expect(before.user.name).toBe("ada");
    expect(next.items).toBe(before.items);
  });
});
