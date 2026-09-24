import { describe, expect, it } from "vitest";
import {
  parseWithDates,
  redact,
  roundTrip,
  stringifyStable,
} from "@ex/01-beginner/ex020_json_roundtrip/index.js";

describe("ex020 roundTrip", () => {
  it("preserves the JSON data types", () => {
    const value = { s: "a", n: 1, b: true, nil: null, list: [1, "2", false] };
    expect(roundTrip(value)).toEqual(value);
  });

  it("drops what JSON has no syntax for", () => {
    const result = roundTrip({ fn: () => {}, un: undefined, sym: Symbol("s"), keep: 1 });
    expect(result).toEqual({ keep: 1 });
  });

  it("turns those same values into null inside an ARRAY, rather than dropping them", () => {
    // An array has to keep its length, so there is nowhere to drop a slot.
    expect(roundTrip([1, undefined, () => {}, 2])).toEqual([1, null, null, 2]);
  });

  it("flattens the types JSON does not know", () => {
    const result = roundTrip({ when: new Date("2024-01-31T12:00:00.000Z"), map: new Map([["a", 1]]) });
    expect(typeof result.when).toBe("string");
    expect(result.map).toEqual({}); // a Map has no own enumerable properties
  });

  it("turns Infinity and NaN into null", () => {
    expect(roundTrip({ a: Infinity, b: NaN })).toEqual({ a: null, b: null });
  });
});

describe("ex020 redact", () => {
  it("removes the named keys at the top level", () => {
    const json = redact({ user: "ada", password: "x", token: "y" }, ["password", "token"]);
    expect(JSON.parse(json)).toEqual({ user: "ada" });
  });

  it("removes them at any depth", () => {
    const json = redact({ a: { b: { password: "x", keep: 1 } } }, ["password"]);
    expect(json).not.toContain("password");
    expect(JSON.parse(json)).toEqual({ a: { b: { keep: 1 } } });
  });

  it("leaves the input object untouched", () => {
    const input = { password: "x" };
    redact(input, ["password"]);
    expect(input).toEqual({ password: "x" });
  });

  it("returns a string, not an object", () => {
    expect(typeof redact({ a: 1 }, [])).toBe("string");
  });
});

describe("ex020 parseWithDates", () => {
  it("rebuilds a Date from an ISO string", () => {
    const result = parseWithDates('{"when":"2024-01-31T12:00:00.000Z"}');
    expect(result.when).toBeInstanceOf(Date);
    expect(result.when.toISOString()).toBe("2024-01-31T12:00:00.000Z");
  });

  it("reaches into nested objects and arrays", () => {
    const result = parseWithDates('{"a":{"b":["2024-01-31T12:00:00.000Z"]}}');
    expect(result.a.b[0]).toBeInstanceOf(Date);
  });

  it("leaves other strings alone", () => {
    const result = parseWithDates('{"a":"2024-01-31","b":"hello","c":5}');
    expect(result).toEqual({ a: "2024-01-31", b: "hello", c: 5 });
  });
});

describe("ex020 stringifyStable", () => {
  it("sorts the keys", () => {
    expect(stringifyStable({ b: 1, a: 2 })).toBe('{"a":2,"b":1}');
  });

  it("gives equal objects equal strings regardless of insertion order", () => {
    expect(stringifyStable({ x: 1, y: { q: 2, p: 3 } })).toBe(
      stringifyStable({ y: { p: 3, q: 2 }, x: 1 }),
    );
  });

  it("sorts nested objects too", () => {
    expect(stringifyStable({ b: { d: 1, c: 2 }, a: 3 })).toBe('{"a":3,"b":{"c":2,"d":1}}');
  });

  it("keeps array order, which is data rather than layout", () => {
    expect(stringifyStable({ list: [3, 1, 2] })).toBe('{"list":[3,1,2]}');
  });
});
