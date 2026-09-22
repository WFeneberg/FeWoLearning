import { describe, expect, it } from "vitest";
import { forEachLine } from "@ex/01-beginner/ex008_function_types/index";

describe("ex008 forEachLine", () => {
  it("visits every line in order", () => {
    const seen: string[] = [];
    // Annotated on purpose: while Callback is still `unknown`, an unannotated
    // parameter here would be an implicit any and put a type error in this
    // file rather than a failing fact.
    forEachLine("alpha\nbeta\ngamma", (line: string) => {
      seen.push(line);
    });
    expect(seen).toEqual(["alpha", "beta", "gamma"]);
  });

  it("accepts a callback that returns a value", () => {
    const seen: string[] = [];
    // push returns a number. A void-returning parameter takes it anyway.
    forEachLine("one\ntwo", (line: string) => seen.push(line));
    expect(seen).toEqual(["one", "two"]);
  });

  it("treats a text without newlines as a single line", () => {
    const seen: string[] = [];
    forEachLine("solo", (line: string) => {
      seen.push(line);
    });
    expect(seen).toEqual(["solo"]);
  });
});
