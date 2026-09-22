import { describe, expect, it } from "vitest";
import {
  assertDefined,
  assertNonEmpty,
  assertString,
} from "@ex/02-intermediate/ex052_assertion_functions/index";

describe("ex052 assertString", () => {
  it("returns quietly for a string", () => {
    expect(() => assertString("ok")).not.toThrow();
  });

  it("throws a TypeError for anything else", () => {
    expect(() => assertString(42)).toThrow(TypeError);
    expect(() => assertString(null)).toThrow(TypeError);
  });
});

describe("ex052 assertDefined", () => {
  it("accepts a value, including a falsy one", () => {
    expect(() => assertDefined(0)).not.toThrow();
    expect(() => assertDefined("")).not.toThrow();
    expect(() => assertDefined(false)).not.toThrow();
  });

  it("throws for null and undefined", () => {
    expect(() => assertDefined(null)).toThrow(TypeError);
    expect(() => assertDefined(undefined)).toThrow(TypeError);
  });
});

describe("ex052 assertNonEmpty", () => {
  it("accepts a list with something in it", () => {
    expect(() => assertNonEmpty([1])).not.toThrow();
  });

  it("throws a RangeError for an empty list", () => {
    expect(() => assertNonEmpty([])).toThrow(RangeError);
  });
});
