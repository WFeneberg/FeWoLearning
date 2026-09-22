import { describe, expect, it } from "vitest";
import { fold, groupBy } from "@ex/02-intermediate/ex064_typed_reduce/index";

describe("ex064 fold", () => {
  it("sums with a numeric seed", () => {
    expect(fold([1, 2, 3] as never, 0 as never, ((a: number, b: number) => a + b) as never)).toBe(6);
  });

  it("returns the seed for an empty input", () => {
    expect(fold([] as never, 42 as never, ((a: number, b: number) => a + b) as never)).toBe(42);
  });

  it("can fold into a different type entirely", () => {
    expect(
      fold(["a", "bb"] as never, "" as never, ((acc: string, s: string) => acc + s) as never),
    ).toBe("abb");
  });

  it("visits items in order", () => {
    expect(
      fold([1, 2, 3] as never, [] as never, ((acc: number[], n: number) => [...acc, n * 2]) as never),
    ).toEqual([2, 4, 6]);
  });
});

describe("ex064 groupBy", () => {
  it("groups by a computed key", () => {
    const words = ["ant", "bee", "aunt", "bat"];
    expect(groupBy(words as never, ((w: string) => w[0]) as never)).toEqual({
      a: ["ant", "aunt"],
      b: ["bee", "bat"],
    });
  });

  it("keeps input order inside each group", () => {
    expect(groupBy([3, 1, 4, 2] as never, ((n: number) => (n % 2 === 0 ? "even" : "odd")) as never)).toEqual({
      odd: [3, 1],
      even: [4, 2],
    });
  });

  it("returns an empty record for no items", () => {
    expect(groupBy([] as never, ((n: number) => n) as never)).toEqual({});
  });
});
