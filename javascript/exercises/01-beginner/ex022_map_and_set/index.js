// Exercise 022 — Map and Set (beginner).
// Goal:   reach for a Map when the keys are data.
// Drills: Map over a plain object, key identity, insertion order, Set for
//         uniqueness, iterating both.
// Passes: countWords() returns a real Map, and objectKeyCollision() shows
//         why a plain object cannot do the job.

/**
 * Counts each word: countWords(["a", "b", "a"]) -> Map { "a" => 2, "b" => 1 },
 * in first-seen order.
 *
 * TODO: implement with a Map.
 */
export function countWords(_words) {
  throw new Error("TODO: implement countWords");
}

/**
 * Keeps the FIRST item for each key produced by keyFn, in order.
 * uniqueBy([{id:1},{id:1},{id:2}], x => x.id) -> the first and the third.
 *
 * TODO: implement with a Set.
 */
export function uniqueBy(_items, _keyFn) {
  throw new Error("TODO: implement uniqueBy");
}

/**
 * Groups items by key into a Map of arrays, in first-seen key order.
 *
 * TODO: implement.
 */
export function groupToMap(_items, _keyFn) {
  throw new Error("TODO: implement groupToMap");
}

/**
 * Stores the number 1 under the key `1` and the string "one" under the key
 * "1" — in a plain object AND in a Map — then reads both keys back from
 * each, returning:
 *   { objectAtNumber, objectAtString, mapAtNumber, mapAtString }
 *
 * A plain object turns every key into a string, so its two writes collide.
 * A Map compares keys with SameValueZero and keeps them apart.
 *
 * TODO: implement.
 */
export function objectKeyCollision() {
  throw new Error("TODO: implement objectKeyCollision");
}
