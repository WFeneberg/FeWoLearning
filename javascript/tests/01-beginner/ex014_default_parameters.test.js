import { describe, expect, it, vi } from "vitest";
import {
  appendTo,
  greet,
  span,
  withId,
} from "@ex/01-beginner/ex014_default_parameters/index.js";

describe("ex014 appendTo", () => {
  it("uses the list it is given, in place", () => {
    const list = ["a"];
    expect(appendTo("b", list)).toBe(list);
    expect(list).toEqual(["a", "b"]);
  });

  it("starts a fresh list on every defaulted call", () => {
    expect(appendTo("x")).toEqual(["x"]);
    expect(appendTo("y")).toEqual(["y"]);
    expect(appendTo("z")).not.toBe(appendTo("z"));
  });

  it("declares the default in the signature, so its length is 1", () => {
    // Function.length counts parameters before the first default. A body-side
    // `list = list || []` would report 2 — which is how this fact tells the
    // two implementations apart.
    expect(appendTo.length).toBe(1);
  });
});

describe("ex014 span", () => {
  it("computes the default from the parameter to its left", () => {
    expect(span(5)).toEqual({ start: 5, end: 15 });
    expect(span(0)).toEqual({ start: 0, end: 10 });
  });

  it("uses an explicit end", () => {
    expect(span(5, 7)).toEqual({ start: 5, end: 7 });
  });

  it("declares one required parameter", () => {
    expect(span.length).toBe(1);
  });
});

describe("ex014 greet", () => {
  it("defaults on nothing and on undefined", () => {
    expect(greet()).toBe("Hello, guest!");
    expect(greet(undefined)).toBe("Hello, guest!");
  });

  it("does not default on null, or on any other falsy value", () => {
    expect(greet(null)).toBe("Hello, null!");
    expect(greet("")).toBe("Hello, !");
    expect(greet(0)).toBe("Hello, 0!");
  });

  it("declares no required parameters", () => {
    expect(greet.length).toBe(0);
  });
});

describe("ex014 withId", () => {
  it("calls the factory only when no id was supplied", () => {
    const make = vi.fn(() => "generated");
    expect(withId(make)).toBe("generated");
    expect(make).toHaveBeenCalledTimes(1);
  });

  it("never calls the factory for a supplied id", () => {
    const make = vi.fn(() => "generated");
    expect(withId(make, "given")).toBe("given");
    expect(make).not.toHaveBeenCalled();
  });

  it("re-runs the factory per defaulted call", () => {
    let counter = 0;
    const make = () => ++counter;
    expect(withId(make)).toBe(1);
    expect(withId(make)).toBe(2);
  });
});
