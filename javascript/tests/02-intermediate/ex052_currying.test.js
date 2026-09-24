import { describe, expect, it } from "vitest";
import {
  curry,
  curry3,
  partial,
  partialRight,
} from "@ex/02-intermediate/ex052_currying/index.js";

const volume = (l, w, h) => l * w * h;
const label = (prefix, body, suffix) => `${prefix}${body}${suffix}`;

describe("ex052 curry3", () => {
  it("takes one argument at a time", () => {
    expect(curry3(volume)(2)(3)(4)).toBe(24);
  });

  it("lets an intermediate stage be reused", () => {
    const fromTwo = curry3(volume)(2);
    expect(fromTwo(3)(4)).toBe(24);
    expect(fromTwo(5)(5)).toBe(50);
  });

  it("returns functions until the last argument", () => {
    expect(typeof curry3(volume)(1)).toBe("function");
    expect(typeof curry3(volume)(1)(1)).toBe("function");
  });
});

describe("ex052 curry", () => {
  it("accepts any grouping of the arguments", () => {
    expect(curry(volume)(2)(3)(4)).toBe(24);
    expect(curry(volume)(2, 3)(4)).toBe(24);
    expect(curry(volume)(2)(3, 4)).toBe(24);
    expect(curry(volume)(2, 3, 4)).toBe(24);
  });

  it("uses the function's declared arity", () => {
    const noArgs = () => "done";
    expect(curry(noArgs)()).toBe("done");
  });

  it("keeps each partially applied branch independent", () => {
    const fromTwo = curry(volume)(2);
    expect(fromTwo(3, 4)).toBe(24);
    expect(fromTwo(1, 1)).toBe(2);
  });

  it("passes the arguments in order, not by name", () => {
    expect(curry(label)("[")("x")("]")).toBe("[x]");
  });
});

describe("ex052 partial", () => {
  it("presets from the left", () => {
    expect(partial(volume, 2)(3, 4)).toBe(24);
    expect(partial(volume, 2, 3)(4)).toBe(24);
  });

  it("reports the remaining arity", () => {
    // bind does this for free; a wrapper arrow would report 0.
    expect(partial(volume, 2).length).toBe(2);
    expect(partial(volume, 2, 3).length).toBe(1);
    expect(partial(volume, 2, 3, 4).length).toBe(0);
  });

  it("does not mutate the original function", () => {
    partial(volume, 1);
    expect(volume.length).toBe(3);
    expect(volume(2, 3, 4)).toBe(24);
  });
});

describe("ex052 partialRight", () => {
  it("presets from the right", () => {
    expect(partialRight(label, "]")("[", "x")).toBe("[x]");
    expect(partialRight(label, "x", "]")("[")).toBe("[x]");
  });

  it("keeps the preset arguments in their given order", () => {
    expect(partialRight(volume, 3, 4)(2)).toBe(24);
  });
});
