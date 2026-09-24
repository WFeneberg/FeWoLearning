import { describe, expect, it } from "vitest";
import {
  errorFromTouchingThisFirst,
  Rectangle,
  Shape,
  Square,
} from "@ex/01-beginner/ex026_inheritance/index.js";

describe("ex026 Shape and Rectangle", () => {
  it("names itself through super()", () => {
    expect(new Rectangle(2, 3).name).toBe("rectangle");
  });

  it("computes its own area", () => {
    expect(new Shape("blob").area()).toBe(0);
    expect(new Rectangle(2, 3).area()).toBe(6);
  });

  it("calls the subclass's area from the base class's describe", () => {
    // describe() is written once, in Shape, and still gets 6 — dynamic
    // dispatch, not a copy of the method.
    expect(new Rectangle(2, 3).describe()).toBe("rectangle has area 6");
  });
});

describe("ex026 Square", () => {
  it("reaches two levels up through one super() call", () => {
    const square = new Square(3);
    expect(square.width).toBe(3);
    expect(square.height).toBe(3);
    expect(square.area()).toBe(9);
    expect(square.name).toBe("square");
  });

  it("wraps the inherited describe with super.describe()", () => {
    expect(new Square(3).describe()).toBe("[sq] square has area 9");
  });

  it("is an instance of every class in its chain", () => {
    const square = new Square(1);
    expect(square).toBeInstanceOf(Square);
    expect(square).toBeInstanceOf(Rectangle);
    expect(square).toBeInstanceOf(Shape);
  });

  it("links the prototypes rather than copying members", () => {
    // Anchored: `extends` links the prototypes before anyone implements a
    // method, so on its own this fact is green against the untouched stub.
    expect(new Square(2).area()).toBe(4);
    expect(Object.getPrototypeOf(Square.prototype)).toBe(Rectangle.prototype);
    expect(Object.getPrototypeOf(Rectangle.prototype)).toBe(Shape.prototype);
    expect(Object.hasOwn(Square.prototype, "area")).toBe(false);
  });
});

describe("ex026 statics", () => {
  it("are inherited by the subclasses", () => {
    expect(Square.describeAll).toBe(Shape.describeAll);
    expect(Square.describeAll([new Square(2), new Rectangle(1, 4)])).toEqual([
      "[sq] square has area 4",
      "rectangle has area 4",
    ]);
  });

  it("accept any iterable", () => {
    expect(Shape.describeAll(new Set([new Shape("dot")]))).toEqual(["dot has area 0"]);
  });
});

describe("ex026 this before super()", () => {
  it("is a ReferenceError, not a silent undefined", () => {
    expect(errorFromTouchingThisFirst()).toBe("ReferenceError");
  });
});
