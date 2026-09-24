import { describe, expect, it } from "vitest";
import { deepClone } from "@ex/02-intermediate/ex057_deep_clone/index.js";

describe("ex057 deepClone", () => {
  it("returns primitives unchanged", () => {
    expect(deepClone(5)).toBe(5);
    expect(deepClone("s")).toBe("s");
    expect(deepClone(null)).toBeNull();
    expect(deepClone(undefined)).toBeUndefined();
  });

  it("copies nested objects and arrays", () => {
    const original = { a: 1, nested: { list: [1, { deep: true }] } };
    const copy = deepClone(original);
    expect(copy).toEqual(original);
    expect(copy).not.toBe(original);
    expect(copy.nested).not.toBe(original.nested);
    expect(copy.nested.list).not.toBe(original.nested.list);
    expect(copy.nested.list[1]).not.toBe(original.nested.list[1]);
  });

  it("leaves the original alone when the copy is mutated", () => {
    const original = { list: [1, 2] };
    deepClone(original).list.push(3);
    expect(original.list).toEqual([1, 2]);
  });

  it("copies Date, Map and Set as their own types", () => {
    const original = {
      when: new Date("2024-01-31T00:00:00.000Z"),
      map: new Map([["k", { v: 1 }]]),
      set: new Set([1, 2]),
    };
    const copy = deepClone(original);
    expect(copy.when).toBeInstanceOf(Date);
    expect(copy.when.getTime()).toBe(original.when.getTime());
    expect(copy.when).not.toBe(original.when);
    expect(copy.map).toBeInstanceOf(Map);
    expect(copy.map.get("k")).toEqual({ v: 1 });
    expect(copy.map.get("k")).not.toBe(original.map.get("k"));
    expect(copy.set).toBeInstanceOf(Set);
    expect([...copy.set]).toEqual([1, 2]);
  });

  it("reuses functions rather than trying to clone them", () => {
    const fn = () => "called";
    const copy = deepClone({ fn });
    expect(copy.fn).toBe(fn);
  });

  it("preserves a cycle, pointing at the copy", () => {
    const original = { name: "self" };
    original.self = original;
    const copy = deepClone(original);
    expect(copy.self).toBe(copy);
    expect(copy.self).not.toBe(original);
  });

  it("keeps a shared reference shared — one copy, two places", () => {
    // The fact a naive recursive clone gets wrong: it produces two separate
    // copies, and a later mutation shows up in only one of them.
    const shared = { count: 0 };
    const copy = deepClone({ left: shared, right: shared });
    expect(copy.left).toBe(copy.right);
    copy.left.count = 1;
    expect(copy.right.count).toBe(1);
  });

  it("survives a deeply nested structure", () => {
    let original = { value: 0 };
    for (let i = 1; i <= 100; i++) original = { value: i, child: original };
    const copy = deepClone(original);
    expect(copy.child.child.value).toBe(98);
    expect(copy.child).not.toBe(original.child);
  });
});
