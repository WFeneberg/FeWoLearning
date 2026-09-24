import { describe, expect, it } from "vitest";
import { inherit, Point, Point3D } from "@ex/02-intermediate/ex046_constructor_functions/index.js";

describe("ex046 Point", () => {
  it("constructs", () => {
    const point = new Point(3, 4);
    expect(point.x).toBe(3);
    expect(point.y).toBe(4);
    expect(point).toBeInstanceOf(Point);
  });

  it("formats and measures through prototype methods", () => {
    expect(String(new Point(1, 2))).toBe("(1, 2)");
    expect(new Point(0, 0).distanceTo(new Point(3, 4))).toBe(5);
  });

  it("shares one function object per method", () => {
    expect(new Point(0, 0).toString).toBe(new Point(1, 1).toString);
    expect(Object.hasOwn(Point.prototype, "distanceTo")).toBe(true);
  });

  it("refuses a call without new", () => {
    expect(() => Point(1, 2)).toThrow(TypeError);
    expect(() => Point(1, 2)).toThrow("Point requires new");
  });

  it("has ENUMERABLE prototype methods, unlike a class", () => {
    // The one visible difference: for..in over an instance picks these up,
    // where a class's methods are non-enumerable and stay hidden.
    const keys = [];
    for (const key in new Point(1, 2)) keys.push(key);
    expect(keys).toEqual(["x", "y", "toString", "distanceTo"]);
  });
});

describe("ex046 inherit", () => {
  it("links the prototypes and restores constructor", () => {
    function Parent() {}
    function Child() {}
    inherit(Child, Parent);
    expect(Object.getPrototypeOf(Child.prototype)).toBe(Parent.prototype);
    expect(Child.prototype.constructor).toBe(Child);
  });

  it("keeps constructor non-enumerable, as the built-in one is", () => {
    function Parent() {}
    function Child() {}
    inherit(Child, Parent);
    expect(Object.keys(Child.prototype)).toEqual([]);
  });

  it("does not make Child.prototype an instance of Parent by calling it", () => {
    let called = false;
    function Parent() {
      called = true;
    }
    function Child() {}
    inherit(Child, Parent);
    expect(called).toBe(false);
  });
});

describe("ex046 Point3D", () => {
  it("reuses Point's constructor for x and y", () => {
    const point = new Point3D(1, 2, 3);
    expect([point.x, point.y, point.z]).toEqual([1, 2, 3]);
  });

  it("inherits Point's methods", () => {
    expect(new Point3D(0, 0, 9).distanceTo(new Point(3, 4))).toBe(5);
  });

  it("overrides toString without touching Point's", () => {
    expect(String(new Point3D(1, 2, 3))).toBe("(1, 2, 3)");
    expect(String(new Point(1, 2))).toBe("(1, 2)");
  });

  it("is an instance of both", () => {
    const point = new Point3D(1, 2, 3);
    expect(point).toBeInstanceOf(Point3D);
    expect(point).toBeInstanceOf(Point);
  });
});
