import { describe, expect, it } from "vitest";
import { card, interpolate, label } from "@ex/01-beginner/ex006_template_literals/index.js";

describe("ex006 label", () => {
  it("pluralizes on the count", () => {
    expect(label("apple", 1)).toBe("apple: 1 item");
    expect(label("apple", 3)).toBe("apple: 3 items");
    expect(label("apple", 0)).toBe("apple: 0 items");
  });
});

describe("ex006 card", () => {
  it("underlines the title to its own length", () => {
    expect(card("Hi", [])).toBe("Hi\n--");
    expect(card("Report", [])).toBe("Report\n------");
  });

  it("bullets each line", () => {
    expect(card("Todo", ["wash", "dry"])).toBe("Todo\n----\n- wash\n- dry");
  });

  it("has no trailing newline", () => {
    expect(card("X", ["only"]).endsWith("\n")).toBe(false);
  });
});

describe("ex006 interpolate", () => {
  it("uses the value's string conversion", () => {
    expect(interpolate(42)).toBe("value: 42");
    expect(interpolate({ toString: () => "custom" })).toBe("value: custom");
    expect(interpolate([1, 2])).toBe("value: 1,2");
  });

  it("spells null and undefined out rather than skipping them", () => {
    expect(interpolate(null)).toBe("value: null");
    expect(interpolate(undefined)).toBe("value: undefined");
  });

  it("lets a symbol throw, because symbols have no string conversion", () => {
    // Asserting the TYPE matters: a stub's own Error would satisfy a bare
    // toThrow().
    expect(() => interpolate(Symbol("s"))).toThrow(TypeError);
  });
});
