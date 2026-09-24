import { describe, expect, it } from "vitest";
import {
  firstAndRest,
  firstTitle,
  parseOptions,
  pickCoords,
  swap,
} from "@ex/01-beginner/ex012_destructuring/index.js";

describe("ex012 firstAndRest", () => {
  it("splits head from tail", () => {
    expect(firstAndRest([1, 2, 3])).toEqual({ first: 1, rest: [2, 3] });
    expect(firstAndRest(["only"])).toEqual({ first: "only", rest: [] });
  });

  it("handles the empty list", () => {
    expect(firstAndRest([])).toEqual({ first: undefined, rest: [] });
  });

  it("does not consume the input", () => {
    const input = [1, 2];
    firstAndRest(input);
    expect(input).toEqual([1, 2]);
  });
});

describe("ex012 swap", () => {
  it("swaps", () => {
    expect(swap([1, 2])).toEqual([2, 1]);
    expect(swap(["a", "b"])).toEqual(["b", "a"]);
  });

  it("returns a new array", () => {
    const input = [1, 2];
    expect(swap(input)).not.toBe(input);
    expect(input).toEqual([1, 2]);
  });
});

describe("ex012 pickCoords", () => {
  it("renames", () => {
    expect(pickCoords({ x: 3, y: 4 })).toEqual({ left: 3, top: 4 });
  });

  it("defaults what is missing", () => {
    expect(pickCoords({ x: 3 })).toEqual({ left: 3, top: 0 });
    expect(pickCoords({})).toEqual({ left: 0, top: 0 });
  });

  it("defaults on undefined but not on null — the rule that catches people", () => {
    expect(pickCoords({ x: undefined })).toEqual({ left: 0, top: 0 });
    expect(pickCoords({ x: null })).toEqual({ left: null, top: 0 });
  });

  it("keeps a legitimate zero", () => {
    expect(pickCoords({ x: 0, y: 0 })).toEqual({ left: 0, top: 0 });
  });
});

describe("ex012 parseOptions", () => {
  it("works with no argument at all", () => {
    expect(parseOptions()).toEqual({ retries: 3, label: "job" });
  });

  it("works with a partial object", () => {
    expect(parseOptions({ tag: "sync" })).toEqual({ retries: 3, label: "sync" });
    expect(parseOptions({ retries: 0 })).toEqual({ retries: 0, label: "job" });
  });

  it("ignores keys it does not know", () => {
    expect(parseOptions({ nonsense: true })).toEqual({ retries: 3, label: "job" });
  });
});

describe("ex012 firstTitle", () => {
  it("digs the title out", () => {
    expect(firstTitle({ data: { items: [{ title: "Hello" }, { title: "Second" }] } })).toBe(
      "Hello",
    );
  });

  it("falls back at every level", () => {
    expect(firstTitle({ data: { items: [] } })).toBe("untitled");
    expect(firstTitle({ data: {} })).toBe("untitled");
    expect(firstTitle({})).toBe("untitled");
    expect(firstTitle({ data: { items: [{}] } })).toBe("untitled");
  });
});
