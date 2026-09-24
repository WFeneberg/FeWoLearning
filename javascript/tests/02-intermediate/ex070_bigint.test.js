import { describe, expect, it } from "vitest";
import {
  comparisons,
  divide,
  factorial,
  mixingError,
  serializeBig,
} from "@ex/02-intermediate/ex070_bigint/index.js";

describe("ex070 factorial", () => {
  it("handles the small cases", () => {
    expect(factorial(0n)).toBe(1n);
    expect(factorial(1n)).toBe(1n);
    expect(factorial(5n)).toBe(120n);
  });

  it("stays exact past 2^53, where Number has already drifted", () => {
    expect(factorial(23n)).toBe(25_852_016_738_884_976_640_000n);
    // The float64 answer to the same question, for contrast. Measured: the
    // two agree up to 22! and part company at 23!.
    let asNumber = 1;
    for (let i = 2; i <= 23; i++) asNumber *= i;
    expect(BigInt(asNumber)).toBe(25_852_016_738_884_978_212_864n);
  });

  it("has no upper limit worth reaching", () => {
    expect(factorial(50n).toString()).toHaveLength(65);
  });
});

describe("ex070 divide", () => {
  it("truncates towards zero", () => {
    expect(divide(7n, 2n)).toBe(3n);
    expect(divide(-7n, 2n)).toBe(-3n);
    expect(divide(6n, 3n)).toBe(2n);
  });

  it("returns a BigInt, not a Number", () => {
    expect(typeof divide(4n, 2n)).toBe("bigint");
  });

  it("throws on division by zero rather than giving Infinity", () => {
    expect(() => divide(1n, 0n)).toThrow(RangeError);
  });
});

describe("ex070 mixingError", () => {
  it("refuses arithmetic between BigInt and Number", () => {
    expect(mixingError()).toBe("TypeError");
  });
});

describe("ex070 comparisons", () => {
  it("converts for comparison but not for identity", () => {
    expect(comparisons()).toEqual({ loose: true, strict: false, greater: true });
  });
});

describe("ex070 serializeBig", () => {
  it("writes a BigInt as a string", () => {
    expect(serializeBig({ id: 1n })).toBe('{"id":"1"}');
    expect(serializeBig({ id: 2n ** 70n })).toBe('{"id":"1180591620717411303424"}');
  });

  it("leaves everything else as JSON would", () => {
    expect(serializeBig({ n: 1, s: "a", b: true })).toBe('{"n":1,"s":"a","b":true}');
  });

  it("reaches into nested values", () => {
    expect(serializeBig({ list: [1n, 2n] })).toBe('{"list":["1","2"]}');
  });

  it("is needed because plain JSON.stringify throws", () => {
    // Anchored: the platform throws with or without this exercise.
    expect(serializeBig({ id: 3n })).toBe('{"id":"3"}');
    expect(() => JSON.stringify({ id: 1n })).toThrow(TypeError);
  });
});
