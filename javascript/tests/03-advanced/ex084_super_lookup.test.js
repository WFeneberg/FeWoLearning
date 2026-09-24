import { describe, expect, it } from "vitest";
import {
  describeAfterCopying,
  describeWithNewParent,
  makeAccessorChild,
  makeChild,
} from "@ex/03-advanced/ex084_super_lookup/index.js";

const parent = () => ({ describe: () => "parent" });

describe("ex084 makeChild", () => {
  it("wraps the inherited answer", () => {
    expect(makeChild(parent()).describe()).toBe("child(parent)");
  });

  it("really inherits rather than copying", () => {
    const base = parent();
    const child = makeChild(base);
    expect(Object.getPrototypeOf(child)).toBe(base);
    expect(Object.hasOwn(child, "describe")).toBe(true);
    expect(Object.keys(child)).toEqual(["describe"]);
  });
});

describe("ex084 describeWithNewParent", () => {
  it("follows the home object's CURRENT prototype", () => {
    // super is resolved at call time against the home object's prototype,
    // not captured when the method was written.
    const child = makeChild(parent());
    expect(describeWithNewParent(child, { describe: () => "replacement" })).toBe(
      "child(replacement)",
    );
  });

  it("changes the child permanently", () => {
    const child = makeChild(parent());
    describeWithNewParent(child, { describe: () => "replacement" });
    expect(child.describe()).toBe("child(replacement)");
  });
});

describe("ex084 describeAfterCopying", () => {
  it("keeps the ORIGINAL home object's super, even on a new receiver", () => {
    // The method's home object travels with the function. `this` is the new
    // object; `super` is still the literal's prototype.
    expect(describeAfterCopying(makeChild(parent()), { describe: () => "other" })).toBe(
      "child(parent)",
    );
  });
});

describe("ex084 makeAccessorChild", () => {
  it("adds to the inherited getter", () => {
    expect(makeAccessorChild({ get total() { return 10; } }, 5).total).toBe(15);
  });

  it("reads the base getter live", () => {
    let backing = 10;
    const base = {
      get total() {
        return backing;
      },
    };
    const child = makeAccessorChild(base, 1);
    expect(child.total).toBe(11);
    backing = 100;
    expect(child.total).toBe(101);
  });

  it("is itself an accessor on the child", () => {
    const child = makeAccessorChild({ get total() { return 1; } }, 1);
    expect(typeof Object.getOwnPropertyDescriptor(child, "total").get).toBe("function");
  });

  it("uses `this` for its own part, so the extra can be changed", () => {
    const child = makeAccessorChild({ get total() { return 10; } }, 5);
    child.extra = 20;
    expect(child.total).toBe(30);
  });
});
