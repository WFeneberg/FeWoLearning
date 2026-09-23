import { beforeEach, describe, expect, it } from "vitest";
import { Widget, counter } from "@ex/03-advanced/ex080_declaration_merging/index";

// Read loosely on purpose. A member that has not been merged in yet does
// not exist, and naming it directly would be a type error in this file
// rather than a failing fact — the same reason ex027 loads its barrel as
// a Record. The .test-d.ts file grades the types.
const loose = counter as unknown as Record<string, unknown>;

function callReset(): void {
  (loose["reset"] as (() => void) | undefined)?.();
}

beforeEach(() => {
  callReset();
});

describe("ex080 counter", () => {
  it("is callable and counts up", () => {
    expect([counter(), counter(), counter()]).toEqual([1, 2, 3]);
  });

  // The namespace's exports are properties of the function itself.
  it("carries its own start value", () => {
    expect(loose["start"]).toBe(1);
  });

  it("carries its own reset", () => {
    counter();
    counter();
    callReset();
    expect(counter()).toBe(1);
  });
});

describe("ex080 Widget", () => {
  it("has the method from its own body", () => {
    expect(new Widget("box").describe()).toBe("<box>");
  });

  // Merging DECLARES the member; something still has to put it there.
  it("has the method the merged interface promised", () => {
    const widget = new Widget("box") as unknown as Record<string, unknown>;
    expect((widget["render"] as () => string)()).toBe("[box]");
  });

  it("finds the merged method on the prototype, not the instance", () => {
    const widget = new Widget("box");
    expect(Object.hasOwn(widget, "render")).toBe(false);
    expect(typeof (widget as unknown as Record<string, unknown>)["render"]).toBe("function");
  });
});
