// Exercise 046 — constructor functions (intermediate).
// Goal:   build what `class` compiles down to, by hand.
// Drills: a function used with `new`, .prototype, wiring a subclass with
//         Object.create, new.target, and the one visible difference from a
//         class — prototype methods written this way are ENUMERABLE.
// Passes: Point3D inherits Point's methods through the prototype chain, and
//         Point refuses to be called without `new`.

/**
 * A constructor function taking x and y onto `this`. Called without `new`
 * it throws a TypeError("Point requires new") — a class does this for you,
 * a function has to check for itself.
 *
 * Check `this instanceof Point`, which is the ES5 idiom, and NOT
 * new.target: Point3D below reuses this constructor with Point.call(this,
 * …), where new.target is undefined even though the call is legitimate.
 * That is measured, not theoretical — the guard written with new.target
 * makes every Point3D construction throw.
 *
 * TODO: implement.
 */
export function Point(_x, _y) {
  throw new Error("TODO: implement Point");
}

// TODO: give Point.prototype two methods:
//   - toString() -> "(x, y)"
//   - distanceTo(other) -> Euclidean distance

/**
 * Wires `Child` to inherit from `Parent` the ES5 way: Child.prototype
 * becomes a fresh object whose prototype is Parent.prototype, and its
 * `constructor` property points back at Child (non-enumerable, as the
 * built-in one is).
 *
 * TODO: implement with Object.create + Object.defineProperty.
 */
export function inherit(_Child, _Parent) {
  throw new Error("TODO: implement inherit");
}

/**
 * A three-dimensional Point that reuses Point's constructor for x and y.
 *
 * TODO: implement — call Point with this as the receiver, add z, and wire
 * the prototypes up with inherit() below.
 */
export function Point3D(_x, _y, _z) {
  throw new Error("TODO: implement Point3D");
}

// TODO: call inherit(Point3D, Point) here, and add a Point3D.prototype
// toString() returning "(x, y, z)".
