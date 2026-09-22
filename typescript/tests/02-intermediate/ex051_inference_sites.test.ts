import { describe, expect, it } from "vitest";
import { mapEach, pipe2 } from "@ex/02-intermediate/ex051_inference_sites/index";

describe("ex051 mapEach", () => {
  it("applies the callback to every item", () => {
    expect(mapEach([1, 2, 3] as never, ((n: number) => n * 2) as never)).toEqual([2, 4, 6]);
  });

  it("returns an empty list for an empty input", () => {
    expect(mapEach([] as never, ((n: number) => n) as never)).toEqual([]);
  });
});

describe("ex051 pipe2", () => {
  it("runs the first function, then the second", () => {
    const run = pipe2(
      ((n: number) => String(n)) as never,
      ((s: string) => s.length) as never,
    ) as (input: number) => number;
    expect(run(1234)).toBe(4);
  });
});
