import { describe, expect, it } from "vitest";
import { Counter } from "@ex/01-beginner/ex027_static_and_private/index.js";

describe("ex027 Counter", () => {
  it("counts", () => {
    const counter = new Counter();
    expect(counter.value).toBe(0);
    expect(counter.increment()).toBe(1);
    expect(counter.increment(5)).toBe(6);
    expect(counter.value).toBe(6);
  });

  it("gives each instance its own private state", () => {
    const first = new Counter();
    const second = new Counter();
    first.increment(3);
    expect(second.value).toBe(0);
  });

  it("formats through a private method", () => {
    const counter = new Counter();
    counter.increment(2);
    expect(counter.toString()).toBe("Counter(2)");
    expect(`${counter}`).toBe("Counter(2)");
  });
});

describe("ex027 what private actually hides", () => {
  it("shows nothing to the ordinary reflection routes", () => {
    const counter = new Counter();
    counter.increment(7);
    expect(Object.keys(counter)).toEqual([]);
    expect(Object.getOwnPropertyNames(counter)).toEqual([]);
    expect(JSON.stringify(counter)).toBe("{}");
    expect(structuredClone({ ...counter })).toEqual({});
  });

  it("is not reachable under a lookalike name", () => {
    const counter = new Counter();
    expect(counter["#count"]).toBeUndefined();
    expect(Object.hasOwn(counter, "#count")).toBe(false);
  });
});

describe("ex027 statics", () => {
  it("counts constructions across every instance", () => {
    const before = Counter.created;
    new Counter();
    new Counter();
    expect(Counter.created - before).toBe(2);
  });

  it("keeps the static counter off the instances", () => {
    expect(new Counter().created).toBeUndefined();
  });
});

describe("ex027 the brand check", () => {
  it("recognises a real Counter", () => {
    expect(Counter.isCounter(new Counter())).toBe(true);
  });

  it("rejects a shape-compatible impostor", () => {
    // instanceof can be fooled by Object.create(Counter.prototype); a
    // private-field check cannot, because the field is installed by the
    // constructor and by nothing else.
    const impostor = Object.create(Counter.prototype);
    expect(impostor).toBeInstanceOf(Counter);
    expect(Counter.isCounter(impostor)).toBe(false);
    expect(Counter.isCounter({ value: 0, increment: () => 1 })).toBe(false);
  });

  it("survives primitives and null without throwing", () => {
    expect(Counter.isCounter(null)).toBe(false);
    expect(Counter.isCounter(undefined)).toBe(false);
    expect(Counter.isCounter(42)).toBe(false);
    expect(Counter.isCounter("Counter(1)")).toBe(false);
  });
});
