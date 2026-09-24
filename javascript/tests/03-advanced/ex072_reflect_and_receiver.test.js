import { describe, expect, it } from "vitest";
import {
  allOwnKeys,
  logReads,
  logReadsWithoutReceiver,
  withDefault,
} from "@ex/03-advanced/ex072_reflect_and_receiver/index.js";

const target = () => ({
  first: "a",
  second: "b",
  get combined() {
    return `${this.first}${this.second}`;
  },
});

describe("ex072 logReads", () => {
  it("logs a plain read", () => {
    const log = [];
    void logReads(target(), log).first;
    expect(log).toEqual(["first"]);
  });

  it("logs the reads a getter makes, because the receiver is the proxy", () => {
    // This is the whole exercise: "combined" runs with `this` bound to the
    // proxy, so its own two reads are trapped as well.
    const log = [];
    expect(logReads(target(), log).combined).toBe("ab");
    expect(log).toEqual(["combined", "first", "second"]);
  });

  it("returns the real values", () => {
    expect(logReads({ a: 1 }, []).a).toBe(1);
  });
});

describe("ex072 logReadsWithoutReceiver", () => {
  it("logs the outer read only", () => {
    const log = [];
    expect(logReadsWithoutReceiver(target(), log).combined).toBe("ab");
    expect(log).toEqual(["combined"]);
  });

  it("differs from the correct version by exactly that", () => {
    const withReceiver = [];
    const without = [];
    logReads(target(), withReceiver).combined;
    logReadsWithoutReceiver(target(), without).combined;
    expect(withReceiver).toHaveLength(3);
    expect(without).toHaveLength(1);
  });
});

describe("ex072 withDefault", () => {
  it("returns the fallback for a missing key", () => {
    expect(withDefault({}, "none").whatever).toBe("none");
  });

  it("returns a real value, including an undefined one", () => {
    expect(withDefault({ a: 1 }, "none").a).toBe(1);
    expect(withDefault({ a: undefined }, "none").a).toBeUndefined();
  });

  it("counts inherited keys as present", () => {
    expect(typeof withDefault({}, "none").toString).toBe("function");
  });

  it("does not add the key to the target", () => {
    const object = {};
    void withDefault(object, "none").missing;
    expect(Object.keys(object)).toEqual([]);
  });
});

describe("ex072 allOwnKeys", () => {
  it("reports string and symbol keys", () => {
    const key = Symbol("s");
    expect(allOwnKeys({ a: 1, [key]: 2 })).toEqual(["a", key]);
  });

  it("reports non-enumerable keys, unlike Object.keys", () => {
    const object = Object.defineProperty({ shown: 1 }, "hidden", { value: 2 });
    expect(allOwnKeys(object)).toEqual(["shown", "hidden"]);
    expect(Object.keys(object)).toEqual(["shown"]);
  });

  it("uses the runtime's own ordering", () => {
    expect(allOwnKeys({ b: 1, 2: 2, a: 3, 1: 4 })).toEqual(["1", "2", "b", "a"]);
  });
});
