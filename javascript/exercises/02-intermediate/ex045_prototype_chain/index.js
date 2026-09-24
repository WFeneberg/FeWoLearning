// Exercise 045 — the prototype chain (intermediate).
// Goal:   see the delegation `class` is built on.
// Drills: Object.create, getPrototypeOf, shadowing, hasOwn vs `in`,
//         a null-prototype object.
// Passes: shadow() leaves the prototype untouched, and bareObject() has no
//         toString at all.

/**
 * An object whose prototype is `proto`, carrying the own properties in
 * `ownProps`.
 *
 * TODO: implement with Object.create.
 */
export function createWith(_proto, _ownProps) {
  throw new Error("TODO: implement createWith");
}

/**
 * The prototype chain above `value`, as an array, ending with null.
 * chainOf({}) -> [Object.prototype, null]
 *
 * TODO: implement with Object.getPrototypeOf in a loop.
 */
export function chainOf(_value) {
  throw new Error("TODO: implement chainOf");
}

/**
 * Where a property comes from:
 *   { own: boolean, inChain: boolean, value }
 * `own` is true only for the object's own property; `inChain` is true when
 * anything in the chain has it (what `in` answers).
 *
 * TODO: implement with Object.hasOwn and `in`.
 */
export function describeProperty(_object, _key) {
  throw new Error("TODO: implement describeProperty");
}

/**
 * Sets an OWN property on `object`, shadowing whatever the prototype had,
 * and returns { objectValue, protoValue } read afterwards — the prototype's
 * value must be unchanged.
 *
 * TODO: implement.
 */
export function shadow(_object, _key, _value) {
  throw new Error("TODO: implement shadow");
}

/**
 * An object with NO prototype at all, carrying { safe: true }.
 *
 * Nothing is inherited: no toString, no hasOwnProperty, and `"toString" in
 * it` is false. This is what a lookup table should be, so that a key called
 * "constructor" cannot collide with anything.
 *
 * TODO: implement.
 */
export function bareObject() {
  throw new Error("TODO: implement bareObject");
}
