// Exercise 066 — mixins (intermediate).
// Goal:   share behaviour without a base class.
// Drills: the class-factory mixin, composing several, and the difference
//         between Object.assign and copying DESCRIPTORS.
// Passes: copyMembers() keeps a getter a getter, where Object.assign would
//         freeze the value it happened to return.

/**
 * A mixin: given a base class, returns a subclass adding
 *   - toJSON(): an object of the instance's own enumerable properties
 *   - a `serialized` getter: JSON.stringify(this)
 *
 * TODO: implement as a function returning `class extends Base`.
 */
export function withSerializable(_Base) {
  throw new Error("TODO: implement withSerializable");
}

/**
 * A second mixin adding a `stamp(now)` method that records `now` on
 * `this.updatedAt` and returns `this`.
 *
 * TODO: implement.
 */
export function withStamp(_Base) {
  throw new Error("TODO: implement withStamp");
}

/**
 * Applies several mixins to a base class, left to right:
 * mix(Base, a, b) === b(a(Base)).
 *
 * TODO: implement.
 */
export function mix(_Base, ..._mixins) {
  throw new Error("TODO: implement mix");
}

/**
 * Copies every own member of `source` onto `target` — including
 * non-enumerable ones — as the SAME KIND of property: a getter stays a
 * getter rather than becoming the value it returned once.
 *
 * Returns target.
 *
 * TODO: implement with Object.defineProperties and
 * Object.getOwnPropertyDescriptors, not Object.assign.
 */
export function copyMembers(_target, _source) {
  throw new Error("TODO: implement copyMembers");
}
