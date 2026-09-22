import { describe, expect, it } from "vitest";
import { pathsOf } from "@ex/03-advanced/ex074_dotted_paths/index";

describe("ex074 pathsOf", () => {
  it("lists a flat object's keys", () => {
    expect(pathsOf({ a: 1, b: "x" })).toEqual(["a", "b"]);
  });

  // The branch AND everything beneath it, branch first.
  it("lists a branch before the paths under it", () => {
    expect(pathsOf({ a: { b: 1 } })).toEqual(["a", "a.b"]);
  });

  it("goes as deep as the object does", () => {
    expect(pathsOf({ a: { b: { c: 1 } } })).toEqual(["a", "a.b", "a.b.c"]);
  });

  it("treats an array as a leaf", () => {
    expect(pathsOf({ tags: ["x", "y"], a: 1 })).toEqual(["tags", "a"]);
  });

  it("treats null as a leaf rather than recursing into it", () => {
    expect(pathsOf({ a: null })).toEqual(["a"]);
  });

  it("returns nothing for an empty object", () => {
    expect(pathsOf({})).toEqual([]);
  });
});
