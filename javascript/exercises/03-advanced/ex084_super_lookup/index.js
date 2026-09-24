// Exercise 084 — super and the home object (advanced).
// Goal:   see what `super` is actually bound to.
// Drills: `super` in an object literal's method, the home object, changing
//         a prototype at runtime, and inherited accessors.
// Passes: a method's `super` follows its HOME OBJECT's prototype — so
//         reassigning the prototype changes what super reaches, while
//         copying the method elsewhere does not.

/**
 * An object literal with a `describe()` method that calls
 * `super.describe()` and wraps the result as `child(<parent's answer>)`.
 *
 * Its prototype is `parent`, set with Object.setPrototypeOf.
 *
 * TODO: implement. Note that `super` is only available in a method written
 * with the shorthand syntax — `describe: function () {}` has no home object
 * and is a syntax error with super in it.
 */
export function makeChild(_parent) {
  throw new Error("TODO: implement makeChild");
}

/**
 * Re-points an existing child's prototype at `newParent` and returns the
 * result of calling child.describe() afterwards.
 *
 * TODO: implement — `super` resolves against the home object's CURRENT
 * prototype, looked up at call time.
 */
export function describeWithNewParent(_child, _newParent) {
  throw new Error("TODO: implement describeWithNewParent");
}

/**
 * Copies `child.describe` onto a brand-new object with a DIFFERENT
 * prototype, calls it there, and returns the result.
 *
 * The method keeps the home object it was written in, so super still
 * reaches the original prototype — even though `this` is the new object.
 *
 * TODO: implement.
 */
export function describeAfterCopying(_child, _otherParent) {
  throw new Error("TODO: implement describeAfterCopying");
}

/**
 * An object whose `total` getter adds `this.extra` to the inherited
 * `super.total`, over a prototype providing `total` as a getter of its own.
 *
 * Returns the object; its prototype is `base`.
 *
 * TODO: implement — `get total() { return super.total + this.extra }`.
 */
export function makeAccessorChild(_base, _extra) {
  throw new Error("TODO: implement makeAccessorChild");
}
