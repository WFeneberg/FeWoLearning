import { describe, expect, it } from "vitest";
import {
  ApiError,
  TimeoutError,
  area,
  describeThrown,
  isCircle,
} from "@ex/02-intermediate/ex053_in_and_instanceof_narrowing/index";

describe("ex053 area", () => {
  it("measures a circle", () => {
    expect(area({ radius: 2 })).toBeCloseTo(Math.PI * 4, 10);
  });

  it("measures a square", () => {
    expect(area({ side: 3 })).toBe(9);
  });
});

describe("ex053 isCircle", () => {
  it("recognises a circle", () => {
    expect(isCircle({ radius: 1 })).toBe(true);
  });

  it("rejects a square", () => {
    expect(isCircle({ side: 1 })).toBe(false);
  });
});

describe("ex053 describeThrown", () => {
  // Most specific first. A TimeoutError is an ApiError is an Error, so an
  // implementation that asks in the wrong order reports "api:408" here.
  it("recognises the most specific subclass first", () => {
    expect(describeThrown(new TimeoutError(500))).toBe("timeout:500");
  });

  it("falls back to the base class", () => {
    expect(describeThrown(new ApiError("nope", 404))).toBe("api:404");
  });

  it("falls back to a plain Error", () => {
    expect(describeThrown(new Error("boom"))).toBe("error:boom");
  });

  it("reports anything else as unknown", () => {
    expect(describeThrown("a string")).toBe("unknown");
    expect(describeThrown(null)).toBe("unknown");
  });
});
