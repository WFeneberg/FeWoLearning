import { describe, expect, it } from "vitest";
import { compose, pipe } from "@ex/03-advanced/ex090_pipe_and_compose/index";

const double = (n: number): number => n * 2;
const toText = (n: number): string => `n=${n}`;
const lengthOf = (s: string): number => s.length;

describe("ex090 pipe", () => {
  it("applies a single function", () => {
    expect((pipe(double as never) as (n: number) => number)(3)).toBe(6);
  });

  it("applies two, left to right", () => {
    expect((pipe(double as never, toText as never) as (n: number) => string)(3)).toBe("n=6");
  });

  it("applies three, left to right", () => {
    expect(
      (pipe(double as never, toText as never, lengthOf as never) as (n: number) => number)(3),
    ).toBe(3);
  });

  it("returns a reusable function, not a one-shot result", () => {
    const run = pipe(double as never, toText as never) as (n: number) => string;
    expect([run(1), run(2)]).toEqual(["n=2", "n=4"]);
  });
});

describe("ex090 compose", () => {
  it("applies a single function", () => {
    expect((compose(double as never) as (n: number) => number)(3)).toBe(6);
  });

  // The other way round: the LAST argument runs first.
  it("applies two, right to left", () => {
    expect((compose(toText as never, double as never) as (n: number) => string)(3)).toBe("n=6");
  });

  it("applies three, right to left", () => {
    expect(
      (compose(lengthOf as never, toText as never, double as never) as (n: number) => number)(3),
    ).toBe(3);
  });

  it("is pipe reversed", () => {
    const piped = pipe(double as never, toText as never) as (n: number) => string;
    const composed = compose(toText as never, double as never) as (n: number) => string;
    expect(piped(5)).toBe(composed(5));
  });
});
