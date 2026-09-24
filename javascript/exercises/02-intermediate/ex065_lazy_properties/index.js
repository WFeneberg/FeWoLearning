// Exercise 065 — lazy properties (intermediate).
// Goal:   compute once, on first read, and leave no getter behind.
// Drills: a getter that replaces itself with a data property,
//         defineProperty from inside the getter, keeping the flags right.
// Passes: the expensive computation runs exactly once no matter how many
//         reads, and after the first read the property is a plain value.

/**
 * Defines `key` on `object` as a lazy property: the first read calls
 * `compute()`, and from then on the property is an ordinary data property
 * holding that value — enumerable, writable, configurable.
 *
 * Returns the object.
 *
 * TODO: implement. The getter itself replaces the property, so `compute`
 * cannot run twice even if it returns undefined.
 */
export function defineLazy(_object, _key, _compute) {
  throw new Error("TODO: implement defineLazy");
}

/**
 * An object with a lazy `settings` property built from `load()`, plus a
 * `loadedYet` accessor reporting whether `settings` has been materialised
 * yet — without reading it.
 *
 * TODO: implement using defineLazy and Object.getOwnPropertyDescriptor.
 */
export function makeConfig(_load) {
  throw new Error("TODO: implement makeConfig");
}
