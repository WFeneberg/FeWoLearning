import { describe, expect, it } from "vitest";
import {
  double,
  makeCart,
  makeRecord,
  makeTagger,
} from "@ex/01-beginner/ex015_arrow_functions/index.js";

describe("ex015 double", () => {
  it("doubles", () => {
    expect(double(4)).toBe(8);
    expect(double(-1.5)).toBe(-3);
  });

  it("is an arrow, so it has no prototype and cannot be constructed", () => {
    expect(double(3)).toBe(6); // anchor: the stub is already an arrow
    expect(double.prototype).toBeUndefined();
    expect(() => new double(1)).toThrow(TypeError);
  });
});

describe("ex015 makeTagger", () => {
  it("closes over the tag", () => {
    const dev = makeTagger("dev");
    expect(dev("api")).toBe("[dev] api");
    expect(dev("db")).toBe("[dev] db");
  });

  it("gives each tagger its own capture", () => {
    expect(makeTagger("a")("x")).toBe("[a] x");
    expect(makeTagger("b")("x")).toBe("[b] x");
  });
});

describe("ex015 makeRecord", () => {
  it("returns the object rather than undefined", () => {
    expect(makeRecord(1)).toEqual({ id: 1, ok: true });
  });

  it("returns a fresh object each call", () => {
    expect(makeRecord(1)).not.toBe(makeRecord(1));
  });
});

describe("ex015 makeCart", () => {
  it("chains adds", () => {
    const cart = makeCart();
    expect(cart.add(10).add(5)).toBe(cart);
    expect(cart.items).toEqual([10, 5]);
  });

  it("totals through the cart's own rate", () => {
    expect(makeCart().add(10).add(5).total()).toBe(15);
    expect(makeCart(2).add(10).add(5).total()).toBe(30);
  });

  it("is 0 for an empty cart", () => {
    expect(makeCart(3).total()).toBe(0);
  });

  it("gives each cart its own items", () => {
    const first = makeCart();
    const second = makeCart();
    first.add(1);
    expect(second.items).toEqual([]);
  });

  it("loses its receiver when the method is detached", () => {
    // Not a defect — the point of the row. A method is a property holding a
    // function, and `this` comes from the call, not from the definition.
    const detached = makeCart().total;
    expect(() => detached()).toThrow(TypeError);
  });
});
