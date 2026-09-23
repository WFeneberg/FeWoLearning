import { describe, expect, it } from "vitest";
import {
  assertEmail,
  describeQuota,
  toEmail,
  toPositiveInt,
} from "@ex/03-advanced/ex077_validated_ids/index";

describe("ex077 toEmail", () => {
  it("accepts a plain address and gives back the same string", () => {
    expect(toEmail("ada@example.com")).toBe("ada@example.com");
  });

  it("rejects a missing @, an empty side, and two @s", () => {
    expect(toEmail("ada.example.com")).toBeUndefined();
    expect(toEmail("@example.com")).toBeUndefined();
    expect(toEmail("ada@")).toBeUndefined();
    expect(toEmail("a@b@c")).toBeUndefined();
  });

  it("rejects whitespace anywhere", () => {
    expect(toEmail("ada @example.com")).toBeUndefined();
    expect(toEmail(" ada@example.com")).toBeUndefined();
  });
});

describe("ex077 toPositiveInt", () => {
  it("accepts a positive integer", () => {
    expect(toPositiveInt(3)).toBe(3);
  });

  it("rejects zero and negatives", () => {
    expect(toPositiveInt(0)).toBeUndefined();
    expect(toPositiveInt(-1)).toBeUndefined();
  });

  it("rejects a fraction", () => {
    expect(toPositiveInt(1.5)).toBeUndefined();
  });

  it("rejects NaN and Infinity", () => {
    expect(toPositiveInt(Number.NaN)).toBeUndefined();
    expect(toPositiveInt(Number.POSITIVE_INFINITY)).toBeUndefined();
  });
});

describe("ex077 assertEmail", () => {
  it("returns quietly for a valid address", () => {
    expect(() => assertEmail("ada@example.com")).not.toThrow();
  });

  it("throws a TypeError otherwise", () => {
    expect(() => assertEmail("nope")).toThrow(TypeError);
  });
});

describe("ex077 describeQuota", () => {
  it("renders the pair", () => {
    const email = toEmail("ada@example.com");
    const count = toPositiveInt(5);
    if (email === undefined || count === undefined) {
      throw new Error("fixture should be valid");
    }
    expect(describeQuota(email, count)).toBe("ada@example.com x5");
  });
});
