import { describe, expect, it } from "vitest";
import {
  makeConfig,
  makeUser,
  orderedKeys,
  tag,
} from "@ex/01-beginner/ex011_object_literals/index.js";

describe("ex011 makeUser", () => {
  it("builds the shape", () => {
    expect(makeUser("Ada", 36, "import")).toEqual({
      name: "Ada",
      age: 36,
      meta: { source: "import" },
    });
  });

  it("names the properties after the parameters", () => {
    expect(Object.keys(makeUser("x", 1, "y"))).toEqual(["name", "age", "meta"]);
  });
});

describe("ex011 tag", () => {
  it("builds the key at runtime", () => {
    expect(tag("env", "prod", 1)).toEqual({ "env:prod": 1 });
    expect(tag("user", 7, "admin")).toEqual({ "user:7": "admin" });
  });

  it("has exactly one property", () => {
    expect(Object.keys(tag("a", "b", null))).toHaveLength(1);
  });
});

describe("ex011 orderedKeys", () => {
  it("puts integer-like keys first, in ascending numeric order", () => {
    // Insertion order was b, 2, a, 1 — and the runtime ignores that for the
    // two keys that look like array indices.
    expect(orderedKeys({ b: 1, 2: 2, a: 3, 1: 4 })).toEqual(["1", "2", "b", "a"]);
  });

  it("keeps insertion order among the string keys", () => {
    expect(orderedKeys({ zeta: 1, alpha: 2 })).toEqual(["zeta", "alpha"]);
  });

  it("skips symbol keys, which Object.keys never reports", () => {
    expect(orderedKeys({ [Symbol("hidden")]: 1, visible: 2 })).toEqual(["visible"]);
  });

  it("is empty for an empty object", () => {
    expect(orderedKeys({})).toEqual([]);
  });
});

describe("ex011 makeConfig", () => {
  it("nests", () => {
    expect(makeConfig("dev", 8080)).toEqual({
      env: "dev",
      server: { port: 8080, host: "localhost" },
      features: [],
    });
  });

  it("gives every call its own arrays and objects", () => {
    const first = makeConfig("dev", 1);
    const second = makeConfig("dev", 1);
    expect(first.features).not.toBe(second.features);
    expect(first.server).not.toBe(second.server);
    first.features.push("x");
    expect(second.features).toEqual([]);
  });
});
