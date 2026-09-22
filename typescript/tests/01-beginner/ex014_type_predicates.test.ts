import { describe, expect, it } from "vitest";
import { isStringArray, isUser } from "@ex/01-beginner/ex014_type_predicates/index";

describe("ex014 isUser", () => {
  it("accepts an object with both string fields", () => {
    expect(isUser({ id: "1", email: "ada@example.com" })).toBe(true);
  });

  it("accepts an object carrying extra fields", () => {
    expect(isUser({ id: "1", email: "ada@example.com", admin: true })).toBe(true);
  });

  it("rejects a missing field", () => {
    expect(isUser({ id: "1" })).toBe(false);
  });

  it("rejects a field of the wrong type", () => {
    expect(isUser({ id: 1, email: "ada@example.com" })).toBe(false);
  });

  it("rejects null and primitives", () => {
    expect(isUser(null)).toBe(false);
    expect(isUser("ada")).toBe(false);
  });
});

describe("ex014 isStringArray", () => {
  it("accepts an array of strings", () => {
    expect(isStringArray(["a", "b"])).toBe(true);
  });

  it("accepts the empty array", () => {
    expect(isStringArray([])).toBe(true);
  });

  it("rejects a mixed array", () => {
    expect(isStringArray(["a", 2])).toBe(false);
  });

  it("rejects a non-array", () => {
    expect(isStringArray("ab")).toBe(false);
  });
});
