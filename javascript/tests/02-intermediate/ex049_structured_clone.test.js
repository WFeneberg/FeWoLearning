import { describe, expect, it } from "vitest";
import {
  clone,
  cloneLosesPrototype,
  cloneUncloneable,
  cloneWithCycle,
} from "@ex/02-intermediate/ex049_structured_clone/index.js";

describe("ex049 clone", () => {
  it("copies deeply, sharing nothing", () => {
    const original = { a: { b: [1, 2] } };
    const copy = clone(original);
    expect(copy).toEqual(original);
    expect(copy).not.toBe(original);
    expect(copy.a).not.toBe(original.a);
    copy.a.b.push(3);
    expect(original.a.b).toEqual([1, 2]);
  });

  it("keeps the types JSON destroys", () => {
    const original = {
      when: new Date("2024-01-31T00:00:00.000Z"),
      pattern: /ab+c/gi,
      map: new Map([["k", { deep: true }]]),
      set: new Set([1, 2]),
      big: 10n ** 20n,
      bytes: new Uint8Array([1, 2, 3]),
    };
    const copy = clone(original);
    expect(copy.when).toBeInstanceOf(Date);
    expect(copy.when.getTime()).toBe(original.when.getTime());
    expect(copy.pattern).toBeInstanceOf(RegExp);
    expect(copy.pattern.flags).toBe("gi");
    expect(copy.map).toBeInstanceOf(Map);
    expect(copy.map.get("k")).toEqual({ deep: true });
    expect(copy.map.get("k")).not.toBe(original.map.get("k"));
    expect(copy.set).toBeInstanceOf(Set);
    expect(copy.big).toBe(10n ** 20n);
    expect(copy.bytes).toBeInstanceOf(Uint8Array);
  });

  it("keeps undefined-valued properties, which JSON drops", () => {
    const copy = clone({ a: undefined });
    expect(Object.hasOwn(copy, "a")).toBe(true);
  });

  it("flattens a getter to the value it returned", () => {
    const copy = clone({
      get computed() {
        return 42;
      },
    });
    expect(copy.computed).toBe(42);
    expect(Object.getOwnPropertyDescriptor(copy, "computed").get).toBeUndefined();
  });
});

describe("ex049 cloneWithCycle", () => {
  it("clones the cycle", () => {
    const copy = cloneWithCycle();
    expect(copy.name).toBe("self");
    expect(copy.self).toBe(copy);
  });

  it("points the cycle at the copy, not at the original", () => {
    const first = cloneWithCycle();
    const second = cloneWithCycle();
    expect(first.self).not.toBe(second);
  });
});

describe("ex049 cloneUncloneable", () => {
  it("refuses a function", () => {
    expect(cloneUncloneable(() => {})).toBe("DataCloneError");
    expect(cloneUncloneable({ nested: () => {} })).toBe("DataCloneError");
  });

  it("refuses a symbol", () => {
    expect(cloneUncloneable(Symbol("s"))).toBe("DataCloneError");
  });

  it("accepts what it can clone", () => {
    expect(cloneUncloneable({ fine: 1 })).toBe("no error");
  });
});

describe("ex049 cloneLosesPrototype", () => {
  it("copies the data and drops the class", () => {
    expect(cloneLosesPrototype()).toEqual({ isTagged: false, hasDescribe: false, value: 7 });
  });
});
