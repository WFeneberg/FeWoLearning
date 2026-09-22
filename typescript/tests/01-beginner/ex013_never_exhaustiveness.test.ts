import { describe, expect, it } from "vitest";
import { area, assertNever } from "@ex/01-beginner/ex013_never_exhaustiveness/index";

describe("ex013 area", () => {
  it("measures a circle", () => {
    expect(area({ kind: "circle", radius: 2 })).toBeCloseTo(Math.PI * 4, 10);
  });

  it("measures a square", () => {
    expect(area({ kind: "square", side: 3 })).toBe(9);
  });
});

describe("ex013 assertNever", () => {
  it("throws, naming the value it was given", () => {
    const unreachable = { kind: "triangle" } as never;
    expect(() => assertNever(unreachable)).toThrow(/^Unexpected value: /);
  });

  it("includes the value as JSON", () => {
    const unreachable = { kind: "triangle" } as never;
    expect(() => assertNever(unreachable)).toThrow(/\{"kind":"triangle"\}/);
  });
});
