// Exercise 026 — inheritance (beginner).
// Goal:   extend a class and call back into it.
// Drills: extends, super() in a constructor, super.method() in a method,
//         dynamic dispatch, inherited statics, the ReferenceError you get
//         for touching `this` before super().
// Passes: describe() written once in Shape calls the subclass's area(),
//         and Square reaches two levels up.

export class Shape {
  /** Stores `name`. TODO: implement. */
  constructor(_name) {
    throw new Error("TODO: implement the Shape constructor");
  }

  /** 0 for a plain shape — subclasses override this. TODO: implement. */
  area() {
    throw new Error("TODO: implement Shape#area");
  }

  /**
   * `<name> has area <area>` — written ONCE here, and must pick up the
   * subclass's area().
   *
   * TODO: implement.
   */
  describe() {
    throw new Error("TODO: implement Shape#describe");
  }

  /** Maps describe() over an iterable of shapes. TODO: implement. */
  static describeAll(_shapes) {
    throw new Error("TODO: implement Shape.describeAll");
  }
}

export class Rectangle extends Shape {
  /**
   * Names itself "rectangle" through super() and stores width and height.
   *
   * TODO: implement.
   */
  constructor(_width, _height) {
    super("rectangle");
    throw new Error("TODO: implement the Rectangle constructor");
  }

  /** width * height. TODO: implement. */
  area() {
    throw new Error("TODO: implement Rectangle#area");
  }
}

export class Square extends Rectangle {
  /**
   * A rectangle with equal sides, named "square".
   *
   * TODO: implement — one super() call with both sides.
   */
  constructor(_side) {
    super(0, 0);
    throw new Error("TODO: implement the Square constructor");
  }

  /**
   * `[sq] ` followed by whatever the inherited describe() produces.
   *
   * TODO: implement with super.describe().
   */
  describe() {
    throw new Error("TODO: implement Square#describe");
  }
}

/**
 * Defines a subclass of Shape whose constructor touches `this` BEFORE
 * calling super(), constructs it inside a try/catch, and returns the
 * caught error's name — "ReferenceError". A derived constructor has no
 * `this` until super() has run.
 *
 * TODO: implement.
 */
export function errorFromTouchingThisFirst() {
  throw new Error("TODO: implement errorFromTouchingThisFirst");
}
