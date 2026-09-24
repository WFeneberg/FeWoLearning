import { describe, expect, it } from "vitest";
import {
  allActive,
  containsValue,
  findUser,
  hasAdmin,
  lastError,
} from "@ex/01-beginner/ex009_array_search/index.js";

const users = () => [
  { id: "u1", role: "user", active: true },
  { id: "u2", role: "admin", active: true },
  { id: "u3", role: "user", active: false },
];

const events = () => [
  { seq: 1, level: "info" },
  { seq: 2, level: "error" },
  { seq: 3, level: "warn" },
  { seq: 4, level: "error" },
  { seq: 5, level: "info" },
];

describe("ex009 findUser", () => {
  it("returns the matching object itself, not a copy", () => {
    const list = users();
    expect(findUser(list, "u2")).toBe(list[1]);
  });

  it("returns undefined when nothing matches", () => {
    expect(findUser(users(), "nope")).toBeUndefined();
    expect(findUser([], "u1")).toBeUndefined();
  });
});

describe("ex009 lastError", () => {
  it("finds the last one, not the first", () => {
    expect(lastError(events()).seq).toBe(4);
  });

  it("returns undefined when there is none", () => {
    expect(lastError([{ seq: 1, level: "info" }])).toBeUndefined();
  });

  it("does not reorder the input", () => {
    const list = events();
    lastError(list);
    expect(list.map((event) => event.seq)).toEqual([1, 2, 3, 4, 5]);
  });
});

describe("ex009 hasAdmin / allActive", () => {
  it("answer over a populated list", () => {
    expect(hasAdmin(users())).toBe(true);
    expect(allActive(users())).toBe(false);
    expect(allActive(users().slice(0, 2))).toBe(true);
    expect(hasAdmin(users().slice(2))).toBe(false);
  });

  it("answer the vacuous way over an empty list", () => {
    // every() is true over nothing; some() is false over nothing.
    expect(allActive([])).toBe(true);
    expect(hasAdmin([])).toBe(false);
  });
});

describe("ex009 containsValue", () => {
  it("finds ordinary values", () => {
    expect(containsValue([1, 2, 3], 2)).toBe(true);
    expect(containsValue([1, 2, 3], 9)).toBe(false);
    expect(containsValue([], 1)).toBe(false);
  });

  it("finds a NaN, which indexOf cannot", () => {
    expect([NaN].indexOf(NaN)).toBe(-1);
    expect(containsValue([NaN], NaN)).toBe(true);
  });

  it("does not coerce", () => {
    expect(containsValue([1, 2], "2")).toBe(false);
  });

  it("finds undefined in a real slot", () => {
    expect(containsValue([undefined], undefined)).toBe(true);
  });
});
