// Exercise 075 — well-known symbols (advanced).
// Goal:   hook into operators and built-in methods.
// Drills: Symbol.hasInstance (instanceof), Symbol.species (what a
//         subclass's methods return), Symbol.isConcatSpreadable.
// Passes: a plain object answers `instanceof`, and a subclass of Array
//         hands back plain Arrays from map() without overriding map().

/**
 * An object usable on the right of `instanceof`, answering `predicate`.
 *
 *   const Even = makeTypeGuard(n => n % 2 === 0);
 *   4 instanceof Even   // true
 *
 * TODO: implement with Symbol.hasInstance.
 */
export function makeTypeGuard(_predicate) {
  throw new Error("TODO: implement makeTypeGuard");
}

/**
 * A subclass of Array whose derived results — map, filter, slice — are
 * plain Arrays rather than instances of the subclass.
 *
 * It must also carry a `first` getter returning its element 0, so the
 * subclass is worth something at all.
 *
 * TODO: implement with a static Symbol.species getter. Do not override
 * map/filter/slice.
 */
export function makeDetachedArrayClass() {
  throw new Error("TODO: implement makeDetachedArrayClass");
}

/**
 * Returns { spreadable, notSpreadable }:
 *   spreadable    — [1].concat(<an array-like with isConcatSpreadable true>)
 *   notSpreadable — [1].concat(<a real array with isConcatSpreadable false>)
 *
 * The array-like is { length: 2, 0: "a", 1: "b" }; the real array is
 * ["a", "b"]. The flag reverses concat's default behaviour in both
 * directions.
 *
 * TODO: implement with Symbol.isConcatSpreadable.
 */
export function concatSpreading() {
  throw new Error("TODO: implement concatSpreading");
}
